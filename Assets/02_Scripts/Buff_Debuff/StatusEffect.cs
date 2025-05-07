using System;
using System.Collections;
using System.Collections.Generic;

public interface IImunity
{
    public IEnumerable<Type> BlockedTypes { get; }
}

public abstract class  StatusEffect
{
    public int remainingTime = 30;//30턴 = 3턴을 의미
    public int tickInterval = 10;
    protected int nextTickTime;
    public int value = 10;
    public StatusEffectData data;

    // 지속 효과인지 여부
    public virtual bool IsPersistent => false;
    public virtual bool IsStackable => false;
    public virtual void Initialize(int startTime)
    {
        nextTickTime = startTime + tickInterval;
    }
    /// <summary>
    /// 삭제처리는 character stat에서 관리
    /// </summary>
    /// <param name="globalTime"></param>
    /// <param name="target"></param>
    public virtual void TryTick(int globalTime, CharacterStats target)
    {
        if (globalTime >= nextTickTime)
        {
            Tick(target);
            nextTickTime += tickInterval;
            remainingTime -= tickInterval;
        }
    }

    /// <summary>
    /// tick에는 tick형 메서드만 작성
    /// stat변경형은 onapply on expired만 하면됨
    /// </summary>
    /// <param name="target"></param>
    public virtual void Tick(CharacterStats target) { }

    public virtual void OnApply(CharacterStats target) { }
    public virtual void OnExpire(CharacterStats target) { }

    public bool IsExpired => remainingTime <= 0;
}
