
public class DefIncrease : StatBuff
{
    public DefIncrease(StatusEffectData data)
    {
        this.data = data;
    }
    public DefIncrease(StatusEffectData data, int value, int remainingTime, int tickInterval)
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
        modifier = new StatModifier("DefenceIncrease", value, ModifierType.Flat);
        target.statBlock.AddModifier(StatType.Defence, modifier);
    }

    public override void OnExpire(CharacterStats target)
    {
        target.statBlock.RemoveModifier(StatType.Defence, modifier);
    }
}
