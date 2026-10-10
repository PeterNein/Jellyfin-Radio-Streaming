# Jellyfin Radio Streaming

Jellyfin Radio Streaming is a server plugin for adding your own internet radio stations to Jellyfin. Configure each station once in the Jellyfin Dashboard, then browse and play the stations from Jellyfin clients that support audio channels.

## Features

- Add, edit, and remove stations from the plugin configuration page.
- Set a station name, an HTTP or HTTPS MP3-compatible stream URL, an optional description, and a logo.
- Use a web-hosted logo or a local image file readable by the Jellyfin server.
- Browse configured stations in Jellyfin's **Radio Stations** channel.
- Use Jellyfin's built-in favorite action to favorite a station for the signed-in user. Favorites are managed by Jellyfin per user.
- Keep station configuration, station models, and playback integration separate, providing a foundation for future features such as playlist imports, station categories, and additional stream formats.

The plugin exposes streams for direct playback; it does not relay or transcode audio. Playback requires a compatible client and a stream reachable during playback.

## Compatibility and requirements

- Jellyfin Server **10.10.7**.
- .NET 8 SDK to build from source.
- An HTTP or HTTPS MP3-compatible radio stream.
- Network access to the stream from the Jellyfin playback path and to web-hosted logos from the Jellyfin server or client.

This plugin is built against Jellyfin 10.10.7. Other Jellyfin versions are not currently certified.

## Install

The project currently provides a build-from-source/manual installation; it is not yet published in the Jellyfin plugin catalog.

### 1. Build the plugin

Install the .NET 8 SDK, clone this repository, and run the following from the repository root:

```powershell
dotnet build .\Jellyfin.Plugin.RadioStreaming.sln --configuration Release
```

The plugin assembly is:

```text
src\Jellyfin.Plugin.RadioStreaming\bin\Release\net8.0\Jellyfin.Plugin.RadioStreaming.dll
```

### 2. Copy it to Jellyfin's plugin directory

Stop Jellyfin, create a `RadioStreaming` folder under the server's `plugins` directory, and copy `Jellyfin.Plugin.RadioStreaming.dll` into that folder. The plugin directory depends on how Jellyfin was installed. Common locations include:

| Installation | Plugin directory |
| --- | --- |
| Windows installer (tray) | `%ProgramData%\Jellyfin\Server\plugins\RadioStreaming` |
| Windows direct installation | `%UserProfile%\AppData\Local\jellyfin\plugins\RadioStreaming` |
| Debian/Ubuntu Linux | `/var/lib/jellyfin/plugins/RadioStreaming` |
| Docker | `<host-config-directory>/plugins/RadioStreaming` (mounted as `/config/plugins/RadioStreaming` in the container) |

If your server uses a custom data directory, use its `plugins` folder instead. Make sure the Jellyfin service account can read the plugin folder and DLL. Start or restart Jellyfin after copying the file.

### 3. Confirm the plugin loaded

Sign in with a Jellyfin administrator account and open **Dashboard > Plugins > My Plugins**. Confirm that **Radio Streaming** is listed. If it is not listed, check the Jellyfin server log for plugin-loading or file-permission errors.

## Configure and use

1. In the Jellyfin Dashboard, open **Plugins > Radio Streaming**.
2. Select **Add station** and enter a station name and its full MP3 stream URL, for example `https://radio.example.org/live.mp3`.
3. Optionally add a description and logo, then select **Save stations**.
4. Open **Radio Stations** in a Jellyfin client to browse the configured stations and start playback.
5. To save a station for later, use the client's built-in favorite action while signed in. Each Jellyfin user has their own favorites.

For a local logo, enter an absolute file path on the **Jellyfin server**, such as `D:\Media\Radio\station.png` on Windows or `/srv/radio/station.png` on Linux. The Jellyfin service account must be able to read the image. A path on your desktop or playback device is not a server-local path.

Stream URLs must use HTTP or HTTPS and be reachable when playback starts. The plugin does not download or proxy station audio for offline playback. Browser or app support can vary with the client's support for the station's particular MP3 stream and redirect behavior.

## Update or remove

To update a manual installation, stop Jellyfin, replace the plugin DLL with the newly built version, and restart Jellyfin. Keep the plugin's configuration files under Jellyfin's `plugins\configurations` directory to retain saved stations.

To remove the plugin, stop Jellyfin, delete the `RadioStreaming` plugin folder, and restart Jellyfin. Remove its configuration file separately only if you also want to delete saved station settings.

## Project structure

- `src/Jellyfin.Plugin.RadioStreaming/Configuration`: persisted settings and the Dashboard station editor.
- `src/Jellyfin.Plugin.RadioStreaming/Models`: station data independent of Jellyfin playback types.
- `src/Jellyfin.Plugin.RadioStreaming/Channels`: maps configured stations to Jellyfin audio channel items.
- `src/Jellyfin.Plugin.RadioStreaming/PluginServiceRegistrator.cs`: registers the channel with Jellyfin.

Future station sources/importers can live in a separate `Sources` area, with shared playback helpers in `Services`. Station metadata and categories can grow in `Models` without coupling those features to the configuration page.

## Source

Repository: [PeterNein/Jellyfin-Radio-Streaming](https://github.com/PeterNein/Jellyfin-Radio-Streaming)
