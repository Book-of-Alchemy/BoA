


public class Bleed : StackableDebuff<Bleed>
{
    public Bleed(StatusEffectData data)
    {
        this.data = data;
    }
    public Bleed(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        this.target = target;
        target.OnTileChanged += AggravateBleed;
    }
    public override void OnExpire(CharacterStats target)
    {
        target.OnTileChanged -= AggravateBleed;
    }
    public override void Tick(CharacterStats target)
    {
        float damage = value * stackCount;
        DamageInfo damageInfo = new DamageInfo(damage, DamageType.None, null, target);
        target.TakeDamage(damageInfo);
    }
    public void AggravateBleed()
    {
        stackCount++;
        remainingTime += 10;
    }

}
