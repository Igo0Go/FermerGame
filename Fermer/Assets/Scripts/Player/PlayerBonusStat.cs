using System.Collections.Generic;

public static class PlayerBonusStat
{
    public static Dictionary<BonusType, int> bonusPack;
    public static int scoreMultiplicator = 1;

    public static void Init()
    {
        scoreMultiplicator = 1;
        bonusPack = new Dictionary<BonusType, int>
        {
            { BonusType.Damage, 1 },
            { BonusType.Invulnerable, 1 },
            { BonusType.Jump, 1 },
            { BonusType.Speed, 1 }
        };
    }
}
