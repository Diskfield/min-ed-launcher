# Minimal Elite Dangerous Launcher

![GitHub Downloads (all assets, all releases)]
![GitHub Downloads (all assets, latest release)]

Linux launcher for the game Elite Dangerous. Created mainly to avoid the long startup
time of the default launcher when running on Linux. Can be used with Steam, Epic and Frontier
accounts.

![preview-gif]

## Table of Contents
* [Features]
* [Usage]
    * [Setup]
        * [Steam]
        * [Epic]
        * [Frontier]
    * [Arguments]
        * [Shared]
        * [Min-Launcher-Specific]
    * [Settings]
    * [Multi-Account]
        * [Frontier account via Steam or Epic]
        * [Epic account via Steam]
    * [Icon on Linux]
    * [Troubleshooting]
    * [Cache]
* [Build]
    * [Release Artifacts]    

## Features
* **Minimal Interface**
  
  No waiting for a GUI to load. No waiting for the embedded web browser to load. No waiting
  for remote logs to be sent to FDev. For those that play on Linux, no waiting for the Wine
  environment.

* **Auto-run**
  
  Automatically starts the game instead of you having to click the _Play_ button. The default
  launcher also supports this by using the `/autorun` [flag].

* **Auto-launch Processes**
  
  Via the [Settings] file, you can specify additional programs to launch automatically. This
  includes things like [Elite Log Agent] and [VoiceAttack]. Processes are automatically closed
  when the game closes. This means you don't have to run the programs on computer startup or
  have to remember to start them before you launch the game.

* **Auto-quit**

  Automatically close the launcher when you close your game. The default launcher also supports
  this by using the `/autoquit` [flag].

* **Auto-restart**

  Automatically restart your game. This can be useful if you're [grinding] for manufactured 
  engineering materials. Off by default. For the Epic platform, this feature requires either 
  [Legendary] or [Heroic]. See the [Epic accounts and the /restart feature] section for more details.

* **Multi-Account**

  Supports running your Steam, Epic and Frontier Store accounts with one game installation.
  See [details] for how this works.
  
## Usage

This launcher doesn't setup/link new accounts. You'll need to either launch Elite Dangerous at least once with
the default launcher or manually create and link your account(s) via the [Linked Thirdparty Accounts] section of
Frontier's website. Linking is only required if you purchased the game via Steam or Epic.

### Setup
#### Steam
1. Download the [latest release]
2. Extract executable from the tar archive
3. Place `MinEdLauncher` in your Elite Dangerous install location so that it's in the same folder
   as `EDLaunch.exe`. To find your install directory:
    1. Right click _Elite Dangerous_ in your Steam library
    2. Select _properties_
    3. In the _Local Files_ tab, click _Browse Local Files..._
4. Update your launch options to point to `MinEdLauncher`.
    1. Right click _Elite Dangerous_ in your Steam library
    2. Select _properties_
    3. In the _general_ tab, click _Set Launch Options_
    4. Set the value to a command that runs `MinEdLauncher` in a terminal emulator. The command will depend on which terminal emulator you use. Examples for
       [alacritty], [gnome-terminal] (Gnome), [xfce4-terminal] (Xfce4), [konsole] (KDE/Steam Deck) and [Ptyxis] (Bazzite) are below.

       ```sh alacritty
       alacritty -e ./MinEdLauncher %command% /autorun /autoquit
       ```  
       ```sh gnome-terminal
       gnome-terminal -- ./MinEdLauncher %command% /autorun /autoquit
       ```
       ```sh xfce4-terminal
       xfce4-terminal --disable-server -x ./MinEdLauncher %command% /autorun /autoquit
       ```
       ```sh konsole
       LD_LIBRARY_PATH="" konsole -e env LD_LIBRARY_PATH="$LD_LIBRARY_PATH" ./MinEdLauncher %command% /autorun /autoquit
       ```
       ```sh ptyxis
       LD_LIBRARY_PATH="" ptyxis -- env LD_LIBRARY_PATH="$LD_LIBRARY_PATH" ./MinEdLauncher %command% /autorun /autoquit
       ```
       > [!NOTE]  
       > Konsole and Ptyxis don't launch when started from the Steam runtime environment. Unsetting LD_LIBRARY_PATH allows them to launch but also causes Proton to run using system libraries causing undefined behavior. Resetting LD_LIBRARY_PATH inside the terminal emulator window solves this.
5. Launch your game as you normally would in Steam
#### Epic
1. Download the [latest release]
2. Extract executable from the tar archive
3. Place `MinEdLauncher` in your Elite Dangerous install location so that it's in the same folder
   as `EDLaunch.exe`.
This method utilizes [Legendary].

1. Ensure you've authenticated, installed Elite Dangerous via [legendary] and setup your wine prefix
2. Pass an exchange code to the launcher by either
   1. Using the `--dry-run` flag and passing the arguments directly to `MinEdLauncher` via command substitution
       ```sh
       WINEPREFIX=/your/wine/prefix /path/to/MinEdLauncher $(legendary launch --dry-run 9c203b6ed35846e8a4a9ff1e314f6593 2>&1 | grep "Launch parameters" | cut -d':' -f 3-) /autorun /autoquit
       ```
   2. Using the `get-token` command and passing it to `MinEdLauncher` via Steam and command substitution
       ```shell
       steam -gameidlaunch 359320 -auth_password=$(legendary get-token 2>&1 | cut -d':' -f 3- | xargs)
       ```

#### Frontier
Frontier accounts are launched through Steam or Epic with the `/frontier profile-name` argument. See the
[multi-account] section for details.

### Arguments
#### Shared
The following arguments are understood by both the vanilla launcher and the minimal launcher.
Note that specifying a product flag will override whatever option you select when Steam prompts
you to select a version of the game.

| Argument       | Effect                                                    |
|----------------|-----------------------------------------------------------|
| /autoquit      | Automatically close the launcher when the game starts     |
| /autorun       | Automatically start selected product when launcher starts |
| /ed            | Select Legacy Elite Dangerous as the startup product      |
| /edh           | Select Legacy Horizons as the startup product             |
| /eda           | Select Elite Dangerous Arena as the startup product       |
| /vr            | Tell the game that you want to play in VR mode            |
| -auth_password | Epic exchange code. Used for authenticating with Epic     |

#### Min Launcher Specific
The following arguments are in addition to the above:

| Argument               | Effect                                                                                                                                                                                                  |
|------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| /autoquit waitForExit  | Automatically close the launcher when the game closes                                                                                                                                                   |
| /edo                   | Select Elite Dangerous Odyssey as the startup product                                                                                                                                                   |
| /edh4                  | Select Elite Dangerous Horizons as the startup product                                                                                                                                                  |
| /frontier profile-name | Use this argument to login with a Frontier Store account. `profile-name` can be any name you want. Keep it to letters, numbers, dashes and underscores. See more details in the [multi-account] section |
| /restart delay         | Restart the game after it has closed with _delay_ being the number of seconds given to cancel the restart (i.e `/restart 3`)                                                                            |
| /dryrun                | Prints output without launching any processes                                                                                                                                                           |
| /skipInstallPrompt     | Skips the prompt to install uninstalled products when `/autorun` is not specified                                                                                                                       |
| /settingsOverlay path  | Load a partial settings overlay file on top of the default `settings.json`. See [Settings Overlay] below                                                                                               |

##### Epic accounts and the /restart feature
The restart feature requires either [Legendary] or [Heroic] to work with Epic accounts.

After Elite launches, it invalidates the launcher's initial Epic auth token and doesn't communicate
the new token which then prevents the ability to login with FDev servers a second time. To work around
this, min-ed-launcher can use a more privileged Epic token (created by Legendary/Heroic) to generate
the needed auth tokens. Simply logging in with Legendary or Heroic will allow min-ed-launcher to restart
without re-launching.

Once you've logged in with either, you can go back to using the normal Epic launcher if you wish. It 
will require re-logging in every few days though, so it may be preferable to just stick with the alternate launchers.

### Settings
The settings file controls additional settings for the launcher that go beyond what the default
launcher supports. The file is in the standard XDG config location. If this file doesn't exist, it will be created on launcher startup.

`$XDG_CONFIG_HOME/min-ed-launcher/settings.json` (`~/.config` if `$XDG_CONFIG_HOME` isn't set)

| Key                       | Type   | Default                    | Description                                                                     |
|---------------------------|--------|----------------------------|---------------------------------------------------------------------------------|
| `apiUri`                  | string | `"https://api.zaonce.net"` | FDev API base URI                                                               |
| `watchForCrashes`         | bool   | `false`                    | Launch game via `WatchDog64.exe`                                                |
| `gameLocation`            | string | *null*                     | Path to game install folder. Auto-detected if omitted                           |
| `language`                | string | *null*                     | Game language (`en`, or a language folder name)                                 |
| `autoUpdate`              | bool   | `true`                     | Auto-update out-of-date games                                                   |
| `checkForLauncherUpdates` | bool   | `true`                     | Check for new min-ed-launcher versions                                          |
| `maxConcurrentDownloads`  | int    | `4`                        | Max simultaneous update downloads                                               |
| `forceUpdate`             | array  | `[]`                       | SKUs to force-update via FDev servers                                           |
| `processes`               | array  | `[]`                       | Programs to launch before the game. See [process fields](#process-fields) below |
| `shutdownProcesses`       | array  | `[]`                       | Programs to launch after game shutdown. Same fields as `processes`              |
| `shutdownTimeout`         | int    | `10`                       | Seconds to wait before force-killing processes                                  |
| `filterOverrides`         | array  | `[]`                       | Override product filters for launch flags (e.g. `/edo`, `/edh`)                 |
| `additionalProducts`      | array  | `[]`                       | Extra products for the authorized list                                          |
| `cacheDir`                | string | *(OS default)*             | Directory for update downloads. See [cache] section for default location        |
| `gameStartDelay`          | int    | `0`                        | Seconds to wait after starting processes but before launching the game          |
| `shutdownDelay`           | int    | `0`                        | Seconds to wait before closing processes                                        |
| `accountAliases`          | object | `{}`                       | Aliases for account names/ids                                                   |

#### Account Aliases
The `accountAliases` field allows you to specify aliases for account names and ids. Useful for when the launcher may be
seen by others (streaming, video capture, etc...) and you don't want to expose your account name.

Each key is either your Frontier account's registered name or Frontier ID. Keys are case-sensitive. The value is the
alias you want to use. Use an empty string (`""`) if you want to hide it completely.

Your ID and name can be viewed at https://user.frontierstore.net/user/info

#### Process fields

| Field               | Type   | Default      | Description                                                                   |
|---------------------|--------|--------------|-------------------------------------------------------------------------------|
| `fileName`          | string | *(required)* | Path to executable                                                            |
| `arguments`         | string | *null*       | Command-line arguments                                                        |
| `alias`             | string | *null*       | Changes CLI output string to specified value instead of outputting `fileName` |
| `restartOnRelaunch` | bool   | `false`      | Restart the process when the game restarts via `/restart`                     |
| `keepOpen`          | bool   | `false`      | Don't stop this process when the launcher exits                               |

> [!NOTE]
> Launching Flatpaks via `processes` through Steam requires resetting the LD_LIBRARY_PATH env var and setting 
> MEL_LD_LIBRARY_PATH to the host's LD_LIBRARY_PATH.
> ```sh
> LD_LIBRARY_PATH="" konsole -e env MEL_LD_LIBRARY_PATH="$LD_LIBRARY_PATH" ./MinEdLauncher %command% /autorun /autoquit
> ```

```json
{
  "apiUri": "https://api.zaonce.net",
  "watchForCrashes": false,
  "gameLocation": null,
  "language": "en",
  "autoUpdate": true,
  "checkForLauncherUpdates": true,
  "maxConcurrentDownloads": 4,
  "forceUpdate": ["PUBLIC_TEST_SERVER_OD"],
  "cacheDir": "/path/to/dir",
  "gameStartDelay": 0,
  "shutdownDelay": 0,
  "processes": [
    {
      "fileName": "/path/to/app"
    },
    {
      "fileName": "/path/to/app2",
      "arguments": "--arg1 --arg2",
      "alias": "App Two",
      "restartOnRelaunch": true,
      "keepOpen": true
    }
  ],
  "shutdownProcesses": [
    {
      "fileName": "/path/to/app",
      "arguments": "--arg1 --arg2"
    }
  ],
  "shutdownTimeout": 10,
  "filterOverrides": [
    { "sku": "FORC-FDEV-DO-1000", "filter": "edo" },
    { "sku": "FORC-FDEV-DO-38-IN-40", "filter": "edh4" }
  ],
  "additionalProducts": [{
    "filter": "edh4",
    "directory": "elite-dangerous-odyssey-64",
    "serverargs": "",
    "gameargs": "SeasonTwo",
    "sortkey": "04",
    "product_name": "Elite Dangerous: Horizons (4.0)",
    "product_sku": "FORC-FDEV-DO-38-IN-40"
  }],
  "accountAliases": {
      "Dwight Schrute": "CMDR Recyclops",
      "fid1234": "CMDR Other Name"
  }
}
```

#### Settings Overlay

The `/settingsOverlay <path>` flag loads a partial JSON file on top of the default `settings.json`. The overlay only needs to contain the keys you want to override — all other settings are inherited from the base file.

For example, to suppress all auto-launched processes for a specific profile, create a file like:

```json
{
  "processes": []
}
```

Then use it with: `./MinEdLauncher /frontier alice /settingsOverlay ~/.config/min-ed-launcher/alice.json /autorun`

Merge rules:
- Scalar values (bool, int, string) — overlay wins if the key is present
- Lists (processes, shutdownProcesses, etc.) — overlay **replaces** the base list if the key is present, even if empty
- Keys not present in the overlay inherit their value from the base `settings.json`

### Multi-Account
#### Frontier account via Steam or Epic
By using the `/frontier profile-name` argument, you can login with any number of Frontier accounts with a
single game installation. Your launch command might look like the following

`alacritty -e ./MinEdLauncher %command% /frontier profile-name /autorun /autoquit`

See the [setup] section above for how you might run this on your platform.

If you have multiple frontier accounts, use a different profile name for each of them. After successfully
logging in, there will be a `.cred` file created in
`$XDG_CONFIG_DIR/min-ed-launcher/`. This file stores your username, password and machine
token. No encryption happens but the file permissions are set to 600. If you login once and then decide
you want to login again (refresh machine token), you can delete the appropriate `.cred` file.

#### Epic account via Steam
There is rudimentary support for running your Epic account via Steam.

In order to authenticate with an Epic account:
1. Get an Epic exchange code. This part is really clunky and will need to be done for every launch as the exchange code expires after one use.
   Get a code via [Legendary].
    ```sh
    legendary get-token
    ```
2. Add the `-auth_password=code` argument to your launch options. `alacritty -e ./MinEdLauncher %command% /autoquit -auth_password=code`

> [!NOTE]
> If you want to use Steam's installation, you'll need to set `gameLocation` in your [settings] file. For example
> ```sh
> "gameLocation": "/home/user/.local/share/Steam/steamapps/common/Elite Dangerous",
> ```

### Steam Shortcuts
You can create multiple shortcuts with different launch parameters. Prefer avoiding the steam protocol handler
as it has some [quirks] that make it more of a pain to use.

Example command for a shortcut that launches Odyssey:

`steam -gameidlaunch 359320 /edo`

### Icon on Linux
1. Copy the included `min-ed-launcher.svg` file to your environment's default icon location

   Common icon locations are `~/.local/share/icons/hicolor/scalable/apps` and `/usr/share/icons/hicolor/scalable/apps`.
2. Copy the included `min-ed-launcher.desktop` file to your environment's default application launcher location

   Common locations are `~/.local/share/applications` and `/usr/share/applications`

   You may need to update your cache by running `update-desktop-database` pointed at whereever you copied the
   desktop file. e.g. `update-desktop-database ~/.local/share/applications/`
3. Set the WM_CLASS/Application Id property in your launch options. How you do this will depend on your terminal 
   emulator. Examples are below:
   * Alacritty - `alacritty --class min-ed-launcher -e ./MinEdLauncher %command% /autorun /autoquit`
   * kitty - `kitty --class min-ed-launcher ./MinEdLauncher %command% /autorun /autoquit`

### Troubleshooting
Debug logging is placed at `$XDG_STATE_HOME/min-ed-launcher/min-ed-launcher.log` (`~/.local/state` if `$XDG_STATE_HOME` isn't set)

### Cache
When updating your game files, the launcher downloads updates into a temporary directory. You may delete these files at any time.
The default location is `$XDG_CACHE_HOME/min-ed-launcher` (`~/.cache` if `$XDG_CACHE_HOME` isn't set)

You can override the default location by specifying the `cacheDir` property in your [settings].

## Build
1. Install the [.Net SDK]

    At least version 10. In general, min-ed-launcher follows the latest version of the SDK.
2. Clone repository and build
    ```
    $ git clone https://github.com/rfvgyhn/min-ed-launcher
    $ cd min-ed-launcher
    $ dotnet build -c Release
    $ dotnet run -c Release --project src/MinEdLauncher/MinEdLauncher.fsproj
   ```
   If you'd prefer a single, self-contained binary, use `publish` instead of `build`. See [publish.sh] for more details. 

### Release Artifacts
Run `publish.sh`. This script makes use of `dotnet publish`. Artifacts will end up in the `artifacts` folder.

[preview-gif]: https://rfvgyhn.blob.core.windows.net/elite-dangerous/min-ed-launcher-demo.gif
[Settings]: #settings
[Settings Overlay]: #settings-overlay
[flag]: #arguments
[setup]: #setup
[Elite Log Agent]: https://github.com/DarkWanderer/Elite-Log-Agent
[VoiceAttack]: https://voiceattack.com/
[details]: #multi-account
[grinding]: https://www.reddit.com/r/EliteDangerous/comments/ggffqq/psa_2020_farming_engineering_materials_a_compiled/
[latest release]: https://github.com/Rfvgyhn/min-ed-launcher/releases
[alacritty]: https://github.com/alacritty/alacritty
[gnome-terminal]: https://wiki.gnome.org/Apps/Terminal
[xfce4-terminal]: https://docs.xfce.org/apps/terminal/start
[konsole]: https://konsole.kde.org/
[Ptyxis]: https://gitlab.gnome.org/chergert/ptyxis
[.Net SDK]: https://dotnet.microsoft.com/download/dotnet
[Features]: #features
[Usage]: #usage
[Steam]: #steam
[Epic]: #epic
[Frontier]: #frontier
[Arguments]: #arguments
[Shared]: #shared
[Min-Launcher-Specific]: #min-launcher-specific
[Epic accounts and the /restart feature]: #epic-accounts-and-the-restart-feature
[Multi-Account]: #multi-account
[Frontier account via Steam or Epic]: #frontier-account-via-steam-or-epic
[Epic account via Steam]: #epic-account-via-steam
[Icon on Linux]: #icon-on-linux
[Troubleshooting]: #troubleshooting
[Cache]: #cache
[Build]: #build
[Release Artifacts]: #release-artifacts
[legendary]: https://github.com/derrod/legendary
[heroic]: https://github.com/Heroic-Games-Launcher/HeroicGamesLauncher
[quirks]: https://github.com/rfvgyhn/min-ed-launcher/issues/45#issuecomment-1030312606
[publish.sh]: publish.sh
[multi-account]: #frontier-account-via-steam-or-epic
[Linked Thirdparty Accounts]: https://user.frontierstore.net/user/info
[GitHub Downloads (all assets, all releases)]: https://img.shields.io/github/downloads/rfvgyhn/min-ed-launcher/total
[GitHub Downloads (all assets, latest release)]: https://img.shields.io/github/downloads/rfvgyhn/min-ed-launcher/latest/total
[Epic-PS-Shortcut]: https://private-user-images.githubusercontent.com/731861/451544289-7ede1c3d-90f0-4fd7-b12f-221771a626d4.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NTM3NDUyMzMsIm5iZiI6MTc1Mzc0NDkzMywicGF0aCI6Ii83MzE4NjEvNDUxNTQ0Mjg5LTdlZGUxYzNkLTkwZjAtNGZkNy1iMTJmLTIyMTc3MWE2MjZkNC5wbmc_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjUwNzI4JTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI1MDcyOFQyMzIyMTNaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT0zMjQ4MDE4MzFiMTRjYzYzYjRiMmE5OTNmZjI0YjI1NDZjYzliMDBmMGQzYmE0MDVjODBhNDRhYzg2YjlkZDNkJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCJ9.4GWg6I-VBf_ChkifnZN4TGUyp7EE36GQjmS1QGrocB4
