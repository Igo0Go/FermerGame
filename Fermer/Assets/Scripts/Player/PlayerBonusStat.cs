using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerBonusStat
{
    public static Dictionary<BonusType, int> bonusPack;
    public static int scoreMultiplicator = 1;

    public static void Init()
    {
        scoreMultiplicator = 1;
        bonusPack = new Dictionary<BonusType, int>();
        bonusPack.Add(BonusType.Damage, 1);
        bonusPack.Add(BonusType.Invulnerable, 1);
        bonusPack.Add(BonusType.Jump, 1);
        bonusPack.Add(BonusType.Speed, 1);
    }
}
