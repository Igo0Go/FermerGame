using System.Collections.Generic;
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

        Help = new DebugCommand("help", "показывает список доступных команд", "help");

        Bonus = new DebugCommand<int>("bonus", "получить на неограниченное врем€ эффект бонуса bonus " +
            "(1 - усиленный прыжок, " +
            "2 - увеличенна€ скорость, " +
            "3 - увеличенный урон, " +
            "4 - неу€звимость, "+
            "0 - сн€ть все",
            "bonus [индекс]");
        Bonus.Execute.AddListener((bonusType) =>
        {
            switch (bonusType)
            {
                case 0:
                    GameController.TAKE_BONUS_JUMP.Invoke(1);
                    GameController.TAKE_BONUS_SPEED.Invoke(1);
                    GameController.TAKE_BONUS_DAMAGE.Invoke(1);
                    GameController.TAKE_BONUS_INVULNERABLE.Invoke(1);
                    break;
                case 1:
                    GameController.TAKE_BONUS_JUMP.Invoke(3);
                    break;
                case 2:
                    GameController.TAKE_BONUS_SPEED.Invoke(3);
                    break;
                case 3:
                    GameController.TAKE_BONUS_DAMAGE.Invoke(3);
                    break;
                case 4:
                    GameController.TAKE_BONUS_INVULNERABLE.Invoke(3);
                    break;
                default:
                    break;
            }
        });

        Teleport = new DebugCommand<string>("tp", "“елепорт с сохранением точки дл€ рестарта", "tp [point]");

        KillWave = new DebugCommand("kill_wave", "если идЄт волна, убивает всех врагов", "kill_wave");

        Gun = new DebugCommand<int>("gun", "получить оружие c 1000 боезапаса: (" +
            "1 - пистолет, 2 - дробовик, 3 ракетницу, 4 - плазмопушку)", "gun [индекс]");

        commandList = new List<BaseDebugCommand>()
        {
            Help,
            Bonus,
            Teleport,
            KillWave,
            Gun
        };
    }

    #region Commands

    public static List<BaseDebugCommand> commandList;

    public static DebugCommand Help;

    public static DebugCommand<int> Bonus;

    public static DebugCommand<int> Gun;

    public static DebugCommand<string> Teleport;

    public static DebugCommand KillWave;

    #endregion
}
