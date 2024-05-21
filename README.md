# ml-patcher-plugin
![Code size](https://img.shields.io/github/languages/code-size/Kaden5480/ml-patcher-plugin?color=5c85d6)
![Open issues](https://img.shields.io/github/issues/Kaden5480/ml-patcher-plugin?color=d65c5c)
![License](https://img.shields.io/github/license/Kaden5480/ml-patcher-plugin?color=a35cd6)

A plugin for MelonLoader which supports BepInEx style patchers.

# Overview
- [Features](#features)
- [Installing](#installing)
    - [Windows](#windows)
    - [Linux](#linux)
- [Developing patchers](#developing-patchers)
- [Building from source](#building)
    - [Dotnet](#dotnet-build)
    - [Visual Studio](#visual-studio-build)
    - [Custom game locations](#custom-game-locations)

# Installing
## Windows
### Prerequisites
- Install Microsoft Visual C++ 2015-2022 Redistributable from
[this link](https://aka.ms/vs/17/release/vc_redist.x64.exe)
or by running `winget install Microsoft.VCRedist.2015+.x64` in cmd/powershell/terminal.
- Install Microsoft .NET Desktop Runtime 6 from
[this link](https://download.visualstudio.microsoft.com/download/pr/d0849e66-227d-40f7-8f7b-c3f7dfe51f43/37f8a04ab7ff94db7f20d3c598dc4d74/windowsdesktop-runtime-6.0.29-win-x64.exe)
or by running `winget install Microsoft.DotNet.DesktopRuntime.6` in cmd/powershell/terminal.

### MelonLoader
- Download the latest nightly MelonLoader build
[here](https://nightly.link/LavaGang/MelonLoader/workflows/build/alpha-development/MelonLoader.Windows.x64.CI.Release.zip).
- Find your game directory, this is most easily done by going to the game in steam,
  pressing the settings for the game (⚙️), selecting "Manage", then "Browse local files".
- Extract the contents of the downloaded zip file into your game directory.
- Run the game and then quit it.
- If MelonLoader was installed correctly, you should notice new directories
  in your game directory (such as Mods).

### MLPatcherPlugin
- Download the latest release
[here](https://github.com/Kaden5480/ml-patcher-plugin/releases).
- The compressed zip file will contain a `Plugins`, and `UserLibs` directory.
- Copy the files from `Plugins` to `Plugins` in your game directory.
- Copy the files from `UserLibs` to `UserLibs` in your game directory.

## Linux
### Prerequisites
- Install [protontricks](https://pkgs.org/download/protontricks).

### Prefix configuration
- Open protontricks.
- Select the game you intend to mod.
- Select "Select the default wineprefix" and press "OK".
- Select "Run winecfg" and press "OK".
- Change "Windows Version" to "Windows 10" and press "Apply".
- Switch to the "Libraries" tab.
- Where it says "New override for library:", choose "version", press "Add", then press "OK".

### Installing prefix components
- Open protontricks.
- Select the game you intend to mod.
- Select "Select the default wineprefix" and press "OK".
- Select "Install Windows DLL or component" and press "OK".
- Select the packages "dotnetdesktop5" and "vcrun2019" and press "OK".
- You may get errors that say checksums didn't match, you can ignore these. When
  you are asked to "Continue anyway", choose "Yes".

### MelonLoader
- Download the latest nightly MelonLoader build
[here](https://nightly.link/LavaGang/MelonLoader/workflows/build/alpha-development/MelonLoader.Windows.x64.CI.Release.zip).
- Find your game directory, this is most easily done by going to the game in steam,
  pressing the settings for the game (⚙️), selecting "Manage", then "Browse local files".
- Extract the contents of the downloaded zip file into your game directory.
- Run the game and then quit it.
- If MelonLoader was installed correctly, you should notice new directories
  in your game directory (such as Mods).

### MLPatcherPlugin
- Download the latest release
[here](https://github.com/Kaden5480/ml-patcher-plugin/releases).
- The compressed zip file will contain a `Plugins`, and `UserLibs` directory.
- Copy the files from `Plugins` to `Plugins` in your game directory.
- Copy the files from `UserLibs` to `UserLibs` in your game directory.

# Developing patchers
To develop a patcher you must have:
- A public static class called `Patcher`.
- A public static void method called `Patch` within the `Patcher` class, which accepts
  one argument of type `Mono.Cecil.AssemblyDefinition`.
- A public static IEnumerable<string>  which can be read so MLPatcherPlugin knows which
  DLLs your patcher targets.
- To be loaded by MLPatcherPlugin, place the compiled plugin in MelonLoader's `Plugins` directory.

Here is an example of a valid patcher:
```cs
using System.Collections.Generic;

using Mono.Cecil;

namespace MyPatcher {
    public static class Patcher {
        // The DLLs this patcher targets in the '<Game>_Data/Managed' directory
        public static IEnumerable<string> TargetDLLs { get; } = new string[] {
            "Assembly-CSharp.dll",
        };

        public static void Patch(AssemblyDefinition assembly) {
            // Patches here
        }
    }
}
```

# Building from source
Whichever approach you use for building from source, the resulting
binaries can be found in `bin/`.

The following configurations are supported:
- Debug
- Release

## Dotnet build
To build with dotnet, run the following command, replacing
<configuration> with the desired value:
```sh
dotnet build -c <configuration>
```

## Visual Studio build
To build with Visual Studio, open MLPatcherPlugin.sln and build by pressing ctrl + shift + b,
or by selecting Build -> Build Solution.
