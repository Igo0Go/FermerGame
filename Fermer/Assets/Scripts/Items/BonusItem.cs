public class BonusItem : GameItem //разные бонусы
{
    public BonusType type;

    public override void Action()
    {
        if(PlayerBonusStat.bonusPack[type] == 1)
        {
            switch (type)
            {
                case BonusType.Speed:
                    GameController.TAKE_BONUS_SPEED.Invoke(2);
                    break;
                case BonusType.Jump:
                    GameController.TAKE_BONUS_JUMP.Invoke(2);
                    break;
                case BonusType.Damage:
                    GameController.TAKE_BONUS_DAMAGE.Invoke(2);
                    break;
                case BonusType.Invulnerable:
                    GameController.TAKE_BONUS_INVULNERABLE.Invoke(2);
                    break;
                default:
                    break;
            }
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
