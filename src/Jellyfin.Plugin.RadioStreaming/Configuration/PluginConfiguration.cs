using MediaBrowser.Model.Plugins;
using Jellyfin.Plugin.RadioStreaming.Models;

namespace Jellyfin.Plugin.RadioStreaming.Configuration;

/// <summary>
/// Settings for the Radio Streaming plugin.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets configured radio stations.
    /// </summary>
    public List<RadioStation> Stations { get; set; } = [];

    /// <summary>
    /// Gets or sets the time the station list was last saved.
    /// </summary>
    public DateTime StationsUpdatedUtc { get; set; } = DateTime.UtcNow;
}
