using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class ConsoleEventCenter
{
    public static bool ShowConsole
    {
        get
        {
            return _showConsole;
        }
        set
        {
            _showConsole = value;
            ShowConsoleChanged.Invoke(value);
        }
    }
    private static bool _showConsole;

    public static UnityEvent<bool> ShowConsoleChanged { get; private set; }

    public static void Reload()
    {
        ShowConsoleChanged = new UnityEvent<bool>();

        Help = new DebugCommand("help", "show command list on screen", "help");

        commandList = new List<BaseDebugCommand>()
        {
            Help
        };
    }


    #region Commands

    public static List<BaseDebugCommand> commandList;

    public static DebugCommand Help;

    #endregion
}
