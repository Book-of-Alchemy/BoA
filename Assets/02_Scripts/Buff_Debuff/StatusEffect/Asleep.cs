
public class Asleep : Debuff
{
    UnitBase unit;
    CharacterStats stats;
    public Asleep(StatusEffectData data)
    {
        this.data = data;
    }
    public Asleep(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void Tick(CharacterStats target)
    {
        if (unit == null || remainingTime <= tickInterval)
            return;

        unit.NextActionTime += 10;
    }

    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        stats = target;
        unit = target.unitBase;
        target.OnTakeDamage += WakeUp;

        Tick(target);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.OnTakeDamage -= WakeUp;
    }

    void WakeUp(DamageInfo damageInfo)
    {
        OnExpire(stats);
        stats.activeEffects.Remove(this);
    }
}
