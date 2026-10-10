# Jellyfin Radio Streaming

A Jellyfin server plugin that presents configured MP3-compatible internet radio streams as playable audio stations.

## Features

- Manage stations in the Jellyfin Dashboard plugin configuration page.
- Configure each station's name, HTTP(S) stream URL, optional description, and logo.
- Use a web logo URL or an absolute local image path readable by the Jellyfin server.
- Browse stations from Jellyfin's **Radio Stations** channel.
- Favorite stations with Jellyfin's built-in favorite control. Favorites are saved per Jellyfin user.
- Keep the code ready for additional station sources and playback features.

## Requirements

- Jellyfin Server 10.10.7
- .NET 8 SDK to build
- An MP3-compatible stream accessible to the Jellyfin server and playback clients

The plugin passes the stream URL through for direct playback; it does not relay or transcode the audio. Some clients may not support every MP3 stream configuration.

## Build

From the repository root:

```powershell
dotnet build .\Jellyfin.Plugin.RadioStreaming.sln --configuration Release
```

The plugin assembly is produced at `src\Jellyfin.Plugin.RadioStreaming\bin\Release\net8.0\Jellyfin.Plugin.RadioStreaming.dll`.

## Install

Create a `RadioStreaming` directory under Jellyfin's `plugins` directory, copy `Jellyfin.Plugin.RadioStreaming.dll` into it, and restart Jellyfin. Jellyfin 10.10.7 provides the referenced Jellyfin assemblies.

After restart, open **Dashboard > Plugins > Radio Streaming**, add stations, and save. Stations are listed under **Radio Stations**. Use each station's built-in favorite action to add it to that user's favorites.

For a local logo, enter a full path on the Jellyfin server, such as `D:\Media\Radio\station.png`. Jellyfin must be able to read the image file.

## Project structure

- `Configuration`: persistent plugin settings and the dashboard station editor.
- `Models`: station data, kept independent of Jellyfin playback types.
- `Channels`: maps stations to Jellyfin's native playable audio-channel items.
- `PluginServiceRegistrator.cs`: Jellyfin dependency-injection registration.

Future features can add station providers/importers under a separate `Sources` area, playback/relay logic under `Services`, and additional station metadata or category models without coupling them to the configuration UI.
