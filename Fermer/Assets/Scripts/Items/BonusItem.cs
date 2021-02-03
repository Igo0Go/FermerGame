using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : GameItem //разные бонусы
{
    public BonusType type;

    public override void Action()
    {
        if(PlayerBonusStat.bonusPack[type] == 1)
        {
            Messenger<int>.Broadcast("TAKE_BONUS_" + type.ToString().ToUpper(), 2);
        }
    }
}

public enum BonusType
{
    Speed, //усиленный прыжок
    Jump, //увеличенная скорость
    Damage, //двойной урон
    Invulnerable //неуязвимость
}
