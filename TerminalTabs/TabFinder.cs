using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace TerminalTabs;

/// <summary>One tab of an open Windows Terminal window, as UI Automation sees it.</summary>
internal sealed record TerminalTab(string Title, string WindowTitle, int Index, bool IsSelected, IntPtr Window, AutomationElement Element);

/// <summary>Finds the tabs of every open Windows Terminal window and switches to one.</summary>
internal static partial class TabFinder
{
    // Every Terminal build (Store, Preview, our own) uses this window class
    private const string WINDOW_CLASS = "CASCADIA_HOSTING_WINDOW_CLASS";
    private const int SW_RESTORE = 9;

    public static List<TerminalTab> FindAll()
    {
        var tabs = new List<TerminalTab>();
        var windowCondition = new PropertyCondition(AutomationElement.ClassNameProperty, WINDOW_CLASS);
        var tabCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);

        foreach (AutomationElement window in AutomationElement.RootElement.FindAll(TreeScope.Children, windowCondition)) {
            try {
                var windowTitle = Clean(window.Current.Name);
                var hwnd = new IntPtr(window.Current.NativeWindowHandle);
                var index = 0;
                foreach (AutomationElement tab in window.FindAll(TreeScope.Descendants, tabCondition)) {
                    var isSelected = tab.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var pattern)
                        && ((SelectionItemPattern)pattern).Current.IsSelected;
                    tabs.Add(new TerminalTab(Clean(tab.Current.Name), windowTitle, index, isSelected, hwnd, tab));
                    index++;
                }
            } catch (ElementNotAvailableException) {
                // the window closed while we read it
            }
        }

        return tabs;
    }

    /// <summary>Selects the tab and brings its window to the front.</summary>
    public static void Switch(TerminalTab tab)
    {
        if (IsIconic(tab.Window)) {
            ShowWindow(tab.Window, SW_RESTORE);
        }
        if (tab.Element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var pattern)) {
            ((SelectionItemPattern)pattern).Select();
        }

        // Focus through UI Automation, which may raise the window where a plain
        // SetForegroundWindow from this background process would only flash it
        try {
            tab.Element.SetFocus();
        } catch (InvalidOperationException) {
            // not focusable right now; the call below still tries
        }
        SetForegroundWindow(tab.Window);
        FocusTerminal(tab);
    }

    // The keyboard goes to the tab's terminal, not its header, so typing works at once.
    // The selected tab's terminal is the visible one; it may take a moment to swap in.
    private static void FocusTerminal(TerminalTab tab)
    {
        var window = AutomationElement.FromHandle(tab.Window);
        var termCondition = new PropertyCondition(AutomationElement.ClassNameProperty, "TermControl");
        for (var i = 0; i < 10; i++) {
            foreach (AutomationElement term in window.FindAll(TreeScope.Descendants, termCondition)) {
                if (term.Current.IsOffscreen) {
                    continue;
                }
                try {
                    term.SetFocus();
                    return;
                } catch (InvalidOperationException) {
                    // not ready yet; try again below
                }
            }
            System.Threading.Thread.Sleep(50);
        }
    }

    // Tab titles carry the shell's status marks in front (Claude's ✳, a spinner):
    // drop anything before the first letter or digit
    private static string Clean(string title)
    {
        var i = 0;
        while (i < title.Length && !char.IsLetterOrDigit(title[i])) {
            i++;
        }

        return i < title.Length ? title[i..] : title;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsIconic(IntPtr hWnd);
}
