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

        Help = new DebugCommand("help", "���������� ������ ��������� ������", "help");

        ClearAllBonuses = new DebugCommand("clear_all_bonuses", "������� ������� ���� �������", "clear_all_bonuses");
        ClearAllBonuses.Execute.AddListener(() =>
        {
            Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_JUMP, 1);
            Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_SPEED, 1);
            Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_DAMAGE, 1);
            Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_INVULNERABLE, 1);
        }
        );

        GetBonusForEver = new DebugCommand<string>("get_bonus", "�������� �� �������������� ����� ������ ������ bonus " +
            "(jump - ��������� ������, " +
            "speed - ����������� ��������, " +
            "damage - ����������� ����, " +
            "invulnerability - ������������",
            "get_bonus bonus");
        GetBonusForEver.Execute.AddListener((bonusType) =>
        {
            if (bonusType.Equals("jump"))
            {
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_JUMP, 3);
            }
            else if (bonusType.Equals("speed"))
            {
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_SPEED, 3);
            }
            else if (bonusType.Equals("damage"))
            {
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_DAMAGE, 3);
            }
            else if (bonusType.Equals("invulnerability"))
            {
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_INVULNERABLE, 3);
            }
        });

        TeleportToArena = new DebugCommand("tp_to_arena", "���������� �������� � ����������������� � �����", "tp_to_arena");

        KillWave = new DebugCommand("kill_wave", "���� ��� �����, ������� ���� ������", "kill_wave");

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
