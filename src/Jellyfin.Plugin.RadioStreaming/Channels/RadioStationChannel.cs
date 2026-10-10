using Jellyfin.Plugin.RadioStreaming.Models;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.RadioStreaming.Channels;

/// <summary>
/// Exposes configured radio stations as playable Jellyfin channel items.
/// </summary>
public class RadioStationChannel : IChannel, IHasCacheKey
{
    private readonly ILogger<RadioStationChannel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioStationChannel"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public RadioStationChannel(ILogger<RadioStationChannel> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "Radio Stations";

    /// <inheritdoc />
    public string Description => "Your configured internet radio stations.";

    /// <inheritdoc />
    public string DataVersion => "1";

    /// <inheritdoc />
    public string HomePageUrl => string.Empty;

    /// <inheritdoc />
    public ChannelParentalRating ParentalRating => ChannelParentalRating.GeneralAudience;

    /// <inheritdoc />
    public InternalChannelFeatures GetChannelFeatures()
    {
        return new InternalChannelFeatures
        {
            MediaTypes = [ChannelMediaType.Audio],
            ContentTypes = [ChannelMediaContentType.Song],
            SupportsContentDownloading = false
        };
    }

    /// <inheritdoc />
    public bool IsEnabledFor(string userId) => true;

    /// <inheritdoc />
    public string? GetCacheKey(string? userId)
    {
        return Plugin.Instance?.Configuration.StationsUpdatedUtc.Ticks.ToString(
            System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        cancellationToken.ThrowIfCancellationRequested();

        var config = Plugin.Instance?.Configuration;
        var stations = config?.Stations ?? [];
        var items = new List<ChannelItemInfo>(stations.Count);
        var updatedUtc = config?.StationsUpdatedUtc ?? DateTime.UtcNow;

        foreach (var station in stations)
        {
            if (!TryCreateItem(station, updatedUtc, items))
            {
                _logger.LogWarning("Skipping invalid radio station configuration with ID {StationId}", station.Id);
            }
        }

        return Task.FromResult(new ChannelItemResult
        {
            Items = items,
            TotalRecordCount = items.Count
        });
    }

    /// <inheritdoc />
    public Task<DynamicImageResponse> GetChannelImage(ImageType type, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new DynamicImageResponse { HasImage = false });
    }

    /// <inheritdoc />
    public IEnumerable<ImageType> GetSupportedChannelImages() => [];

    private static bool TryCreateItem(RadioStation station, DateTime updatedUtc, ICollection<ChannelItemInfo> items)
    {
        if (station.Id == Guid.Empty
            || string.IsNullOrWhiteSpace(station.Name)
            || !Uri.TryCreate(station.StreamUrl?.Trim(), UriKind.Absolute, out var streamUri)
            || (streamUri.Scheme != Uri.UriSchemeHttp && streamUri.Scheme != Uri.UriSchemeHttps))
        {
            return false;
        }

        var name = station.Name.Trim();
        var item = new ChannelItemInfo
        {
            Id = "station_" + station.Id.ToString("N"),
            Name = name,
            Type = ChannelItemType.Media,
            MediaType = ChannelMediaType.Audio,
            ContentType = ChannelMediaContentType.Song,
            IsLiveStream = true,
            Overview = station.Description?.Trim(),
            ImageUrl = GetImagePath(station.Logo),
            DateModified = updatedUtc
        };

        item.MediaSources.Add(new MediaSourceInfo
        {
            Id = "radio_" + station.Id.ToString("N"),
            Name = name,
            Path = streamUri.ToString(),
            Protocol = MediaProtocol.Http,
            Type = MediaSourceType.Default,
            IsRemote = true,
            IsInfiniteStream = true,
            Container = "mp3",
            SupportsDirectPlay = true,
            SupportsDirectStream = false,
            SupportsTranscoding = false,
            MediaStreams =
            [
                new MediaStream
                {
                    Type = MediaStreamType.Audio,
                    Index = -1,
                    Codec = "mp3",
                    IsDefault = true
                }
            ]
        });

        items.Add(item);
        return true;
    }

    private static string? GetImagePath(string? logo)
    {
        if (string.IsNullOrWhiteSpace(logo))
        {
            return null;
        }

        var value = logo.Trim();
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return uri.ToString();
        }

        return Path.IsPathFullyQualified(value) && File.Exists(value) ? value : null;
    }
}
