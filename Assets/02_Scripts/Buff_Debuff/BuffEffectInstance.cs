public class BuffEffectInstance
{
    public BuffEffect buffEffect { get; private set; }
    public float effectValue { get; private set; }
    public int remainingTurns { get; private set; }

    public BuffEffectInstance(BuffEffect buffEffect)
    {
        this.buffEffect = buffEffect;
        effectValue = buffEffect.effectValue;
        remainingTurns = buffEffect.duration;
    }

    // 스택 가능한 효과일 경우 새 효과 값을 누적
    public void AddEffect(BuffEffect buffEffect)
    {
        effectValue += buffEffect.effectValue;
        remainingTurns += buffEffect.duration;
    }

    // 중첩 불가능한 효과의 경우 새 효과로 덮어씀
    public void OverwriteEffect(BuffEffect buffEffect)
    {
        effectValue = buffEffect.effectValue;
        remainingTurns = buffEffect.duration;
    }

    public void DecrementDuration() => remainingTurns--;
}
