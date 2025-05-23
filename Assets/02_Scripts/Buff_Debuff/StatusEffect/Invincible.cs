

public class Invincible : Buff
{
    public Invincible(StatusEffectData data)
    {
        this.data = data;
    }
    public Invincible(StatusEffectData data, int value, int remainingTime, int tickInterval)
    {
        this.data = data;
        this.value = value;
        this.remainingTime = remainingTime;
        this.tickInterval = tickInterval;
    }
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if(!shouldRegister) return;
        target.isInvincible = true;
    }

    public override void OnExpire(CharacterStats target)
    {
       target.isInvincible = false;
    }
}
