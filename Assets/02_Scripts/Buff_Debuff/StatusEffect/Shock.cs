

public class Shock : StackableDebuff<Poison>
{
    private bool isProcessingExtraTick = false;
    public Shock(StatusEffectData data)
    {
        this.data = data;
    }
    public Shock(StatusEffectData data, int value, int remainingTime, int tickInterval)
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
        target.OnTakeDamage += ExtraTick;
    }
    public override void OnExpire(CharacterStats target)
    {
        target.OnTakeDamage -= ExtraTick;
    }
    public override void Tick(CharacterStats target)
    {
        float damage = value * stackCount;
        DamageInfo damageInfo = new DamageInfo(damage, DamageType.Lightning, null, target);
        target.TakeDamage(damageInfo);
    }

    protected void ExtraTick(DamageInfo damageInfo)
    {
        // 추가 대미지를 처리 중이면 중복 실행 금지
        if (isProcessingExtraTick) return;

        try
        {
            isProcessingExtraTick = true;

            float damage = value * stackCount;
            DamageInfo extraDmg = new DamageInfo(damage, DamageType.Lightning, null, target);

            target.TakeDamage(extraDmg);
        }
        finally//return시에도 반드시 실행하고 종료
        {
            isProcessingExtraTick = false;
        }
    }
}
