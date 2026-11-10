using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TerminalTabs;

/// <summary>The list of open Terminal tabs; Command Palette filters it as you type.</summary>
internal sealed partial class TerminalTabsPage : ListPage
{
    public const string ID = "com.omnicrash.terminaltabs";

    // The command prompt glyph of Segoe Fluent Icons, like the palette's own commands use
    public static readonly IconInfo ICON = new("\uE756");

    public TerminalTabsPage()
    {
        Id = ID;
        Icon = ICON;
        Title = "Terminal tabs";
        Name = "Open";
        PlaceholderText = "Type a tab's name";
    }

    public override IListItem[] GetItems()
    {
        var tabs = TabFinder.FindAll();
        if (tabs.Count == 0) {
            return [new ListItem(new NoOpCommand()) { Title = "No Terminal tabs open" }];
        }

        // several windows: say which one a tab is in
        var windows = tabs.Select(t => t.Window).Distinct().Count();

        return [.. tabs.Select(tab => new ListItem(new SwitchTabCommand(tab)) {
            Icon = ICON,
            Title = tab.Title,
            Subtitle = windows > 1 ? $"in {tab.WindowTitle}" : string.Empty,
        })];
    }
}

/// <summary>Brings one tab to the front.</summary>
internal sealed partial class SwitchTabCommand : InvokableCommand
{
    private readonly TerminalTab _tab;

    public SwitchTabCommand(TerminalTab tab)
    {
        _tab = tab;
        Name = "Switch to tab";
    }

    public override CommandResult Invoke()
    {
        TabFinder.Switch(_tab);

        return CommandResult.Dismiss();
    }
}
