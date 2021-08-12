# MouseTrap

Some games don't capture the mouse cursor properly and let it escape to other monitors, where clicking will cause the game to lose focus. Typically these are older games before multi-monitor was common, though the game that caused me to develop MouseTrap was Valheim which has the issue sometimes.

MouseTrap does one thing; it keeps the cursor within the bounds of the monitor where the active window is located. You can of course still use Alt-Tab to switch between windows. Disable or close MouseTrap to release the cursor.

![Screenshot](Screenshot.png)

## Download
The latest installer is available on the [releases](https://github.com/MvRens/MouseTrap/releases) page.
.NET Framework 4.8 is required, the installer will check if it is installed and guide you to the download page otherwise.

Currently only a 64-bits version is included. If you need a 32-bits version, either build it yourself or ask nicely!


## Developing
MouseTrap is written in C# using Visual Studio 2019.

## Builds
Builds are automatically run using AppVeyor.

Master build
[![Build status](https://ci.appveyor.com/api/projects/status/fpcmve1686qb04li/branch/master?svg=true)](https://ci.appveyor.com/project/MvRens/mousetrap/branch/master)

Latest build
[![Build status](https://ci.appveyor.com/api/projects/status/fpcmve1686qb04li?svg=true)](https://ci.appveyor.com/project/MvRens/mousetrap)