namespace Jellyfin.Plugin.RadioStreaming.Models;

/// <summary>
/// A user-configured internet radio station.
/// </summary>
public class RadioStation
{
    /// <summary>
    /// Gets or sets the station's stable identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the station name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP or HTTPS audio stream URL.
    /// </summary>
    public string StreamUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an HTTP(S) logo URL or a filesystem path readable by Jellyfin.
    /// </summary>
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the station description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
