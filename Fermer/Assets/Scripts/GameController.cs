using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public static class GameController
{
    public static UnityEvent<bool> PAUSE { get; private set; }
    public static UnityEvent START_FINAL_LOADING { get; private set; }
    public static UnityEvent<int> NEXT_WAVE { get; private set; }
    public static UnityEvent<GameObject> KILL_ENEMY_FROM_WAVE { get; private set; }

    public static UnityEvent<float> MUSIC_CHANGED { get; private set; }
    public static UnityEvent<float> SOUNDS_CHANGED { get; private set; }
    public static UnityEvent<float> VOICE_CHANGED { get; private set; }
    public static UnityEvent<float> MOUSE_CHANGED { get; private set; }

    public static UnityEvent<float> CHANGE_HEALTH { get; private set; }
    public static UnityEvent<float> CHANGE_MAX_HEALTH { get; private set; }
    public static UnityEvent<int> DAMAGE_MARKER_ACTIVATE { get; private set; }
    public static UnityEvent PLAYER_DEAD { get; private set; }
    public static UnityEvent HIT { get; private set; }

    public static UnityEvent<int> CHANGE_SPRINT_COUNT { get; private set; }
    public static UnityEvent SPRINT_ACTION { get; private set; }
    public static UnityEvent<Vector3> START_SPRINT { get; private set; }
    public static UnityEvent STOP_SPRINT { get; private set; }

    public static UnityEvent<int> ENEMY_HIT { get; private set; }
    public static UnityEvent ENEMY_DEAD { get; private set; }

    public static UnityEvent WEAPON_ARE_HIDDEN { get; private set; }
    public static UnityEvent WEAPON_READY { get; private set; }
    public static UnityEvent<int> WEAPON_ARE_CHANGED { get; private set; }
    public static UnityEvent<int> AMMO_ARE_CHANGED { get; private set; }
    public static UnityEvent RETURN_TO_DEFAULT { get; private set; }

    public static UnityEvent<int> TAKE_BONUS_JUMP { get; private set; }
    public static UnityEvent<int> TAKE_BONUS_SPEED { get; private set; }
    public static UnityEvent<int> TAKE_BONUS_DAMAGE { get; private set; }
    public static UnityEvent<int> TAKE_BONUS_INVULNERABLE { get; private set; }

    public static UnityEvent PLAYER_MUSIC_LOADED { get; private set; }
    public static UnityEvent<string> PLAYER_MUSIC_LOAD_CLIP_COMPLETED { get; private set; }
    public static UnityEvent<string> PLAYER_MUSIC_CHANGED { get; private set; }

    public static bool toArena;
    public static bool useSubTitles = true;

    public static List<AudioClip> playerMusic;

    public static float MusicVolume = 0.25f;
    public static float SoundsVolume = 0.25f;
    public static float VoicesVolume = 1;
    public static float MouseSensivity = 0.5f;


    static GameController()
    {
        Init();
    }

    public static void Init()
    {
        PAUSE = new UnityEvent<bool>();
        START_FINAL_LOADING = new UnityEvent();
        NEXT_WAVE = new UnityEvent<int>();
        KILL_ENEMY_FROM_WAVE = new UnityEvent<GameObject>();

        MUSIC_CHANGED = new UnityEvent<float>();
        SOUNDS_CHANGED = new UnityEvent<float>();
        VOICE_CHANGED = new UnityEvent<float>();
        MOUSE_CHANGED = new UnityEvent<float>();

        CHANGE_HEALTH = new UnityEvent<float>();
        CHANGE_MAX_HEALTH = new UnityEvent<float>();
        DAMAGE_MARKER_ACTIVATE = new UnityEvent<int>();
        PLAYER_DEAD = new UnityEvent();
        HIT = new UnityEvent();

        CHANGE_SPRINT_COUNT = new UnityEvent<int>();
        SPRINT_ACTION = new UnityEvent();
        START_SPRINT = new UnityEvent<Vector3>();
        STOP_SPRINT = new UnityEvent();

        ENEMY_HIT = new UnityEvent<int>();
        ENEMY_DEAD = new UnityEvent();

        WEAPON_ARE_HIDDEN = new UnityEvent();
        WEAPON_READY = new UnityEvent();
        WEAPON_ARE_CHANGED = new UnityEvent<int>();
        AMMO_ARE_CHANGED = new UnityEvent<int>();

        RETURN_TO_DEFAULT = new UnityEvent();

        TAKE_BONUS_JUMP = new UnityEvent<int>();
        TAKE_BONUS_SPEED  = new UnityEvent<int>();
        TAKE_BONUS_DAMAGE = new UnityEvent<int>();
        TAKE_BONUS_INVULNERABLE = new UnityEvent<int>();

        PLAYER_MUSIC_CHANGED = new UnityEvent<string>();
        PLAYER_MUSIC_LOAD_CLIP_COMPLETED = new UnityEvent<string>();
        PLAYER_MUSIC_LOADED = new UnityEvent();
    }
}