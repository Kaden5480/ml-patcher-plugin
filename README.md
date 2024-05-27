# ml-patcher-plugin
![Code size](https://img.shields.io/github/languages/code-size/Kaden5480/ml-patcher-plugin?color=5c85d6)
![Open issues](https://img.shields.io/github/issues/Kaden5480/ml-patcher-plugin?color=d65c5c)
![License](https://img.shields.io/github/license/Kaden5480/ml-patcher-plugin?color=a35cd6)

A plugin for MelonLoader which supports BepInEx style patchers.

# Overview
- [Installing](#installing)
- [Developing patchers](#developing-patchers)
- [Building from source](#building)
    - [Dotnet](#dotnet-build)
    - [Visual Studio](#visual-studio-build)
    - [Custom game locations](#custom-game-locations)

# Installing
## MelonLoader
If you haven't installed MelonLoader, follow the install instructions here:
- [Windows](https://github.com/Kaden5480/modloader-instructions#melonloader-windows)
- [Linux](https://github.com/Kaden5480/modloader-instructions#melonloader-linux)

## MLPatcherPlugin
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
To build with Visual Studio, open MLPatcherPlugin.sln and build by pressing
ctrl + shift + b, or by selecting Build -> Build Solution.
