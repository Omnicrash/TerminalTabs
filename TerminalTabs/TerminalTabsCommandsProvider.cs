// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TerminalTabs;

public partial class TerminalTabsCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public TerminalTabsCommandsProvider()
    {
        DisplayName = "Terminal tabs";
        Icon = TerminalTabsPage.ICON;
        _commands = [
            new CommandItem(new TerminalTabsPage()) { Title = DisplayName },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
