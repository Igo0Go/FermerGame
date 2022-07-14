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

        Help = new DebugCommand("help", "показывает список доступных команд", "help");

        ClearAllBonuses = new DebugCommand("clear_all_bonuses", "убирает эффекты всех бонусов", "clear_all_bonuses");
        ClearAllBonuses.Execute.AddListener(() =>
        {
            GameController.TAKE_BONUS_JUMP.Invoke(1);
            GameController.TAKE_BONUS_SPEED.Invoke(1);
            GameController.TAKE_BONUS_DAMAGE.Invoke(1);
            GameController.TAKE_BONUS_INVULNERABLE.Invoke(1);
        }
        );

        GetBonusForEver = new DebugCommand<string>("get_bonus", "получить на неограниченное врем€ эффект бонуса bonus " +
            "(jump - усиленный прыжок, " +
            "speed - увеличенна€ скорость, " +
            "damage - увеличенный урон, " +
            "invulnerability - неу€звимость",
            "get_bonus bonus");
        GetBonusForEver.Execute.AddListener((bonusType) =>
        {
            if (bonusType.Equals("jump"))
            {
                GameController.TAKE_BONUS_JUMP.Invoke(3);
            }
            else if (bonusType.Equals("speed"))
            {
                GameController.TAKE_BONUS_SPEED.Invoke(3);
            }
            else if (bonusType.Equals("damage"))
            {
                GameController.TAKE_BONUS_DAMAGE.Invoke(3);
            }
            else if (bonusType.Equals("invulnerability"))
            {
                GameController.TAKE_BONUS_INVULNERABLE.Invoke(3);
            }
        });

        TeleportToArena = new DebugCommand("tp_to_arena", "пропустить обучение и телепортироватьс€ к арене", "tp_to_arena");

        KillWave = new DebugCommand("kill_wave", "если идЄт волна, убивает всех врагов", "kill_wave");

        commandList = new List<BaseDebugCommand>()
        {
            Help,
            ClearAllBonuses,
            GetBonusForEver,
            TeleportToArena,
            KillWave
        };
    }


    #region Commands

    public static List<BaseDebugCommand> commandList;

    public static DebugCommand Help;

    public static DebugCommand<string> GetBonusForEver;

    public static DebugCommand ClearAllBonuses;

    public static DebugCommand TeleportToArena;

    public static DebugCommand KillWave;

    #endregion
}
