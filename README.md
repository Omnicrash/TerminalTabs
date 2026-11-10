# Terminal tabs

A [PowerToys Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview)
extension that jumps to a Windows Terminal tab by name.

Open Command Palette, type `#` and part of a tab's name - `#spira` - and press Enter: the
Terminal window holding that tab comes to the front with the tab selected. Without a name,
`#` lists every tab of every open Terminal window, and with several windows each tab says
which window it is in.

I run a couple dozen long-lived sessions, each in its own tab, spread over two windows.
Finding one meant clicking through tab strips. Nothing else did this: Command Palette's
window switcher stops at windows, and the community tab switchers cover browsers.

## How it works

The extension reads the tabs of every open Terminal window through UI Automation, the
same interface screen readers use, so it needs no changes to Terminal and works with the
Store, Preview and self-built versions alike. Tab titles are shown as Terminal shows them,
minus any status marks in front (a spinner, an asterisk). Enter selects the tab and raises
its window.

## Building and installing

You need Windows 11 with PowerToys, [Developer Mode](https://learn.microsoft.com/windows/advanced-settings/developer-mode)
on, the .NET 10 SDK, and Visual Studio 2026 with the Windows App SDK (WinUI) workload.

`build.cmd` builds the extension, registers it from its build folder the way Visual
Studio's Deploy does, and restarts Command Palette so it loads the new build. Then, in
Command Palette:

1. Type `terminal tabs` to open the list.
2. To start it with `#`, open Command Palette's settings, find **Terminal tabs** under
   Extensions, and give it the alias `#` (or any other free symbol), marked as direct.

## License

MIT. The project started from Command Palette's extension template, which is MIT-licensed
by Microsoft; its files keep their original headers.
