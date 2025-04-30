using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{

    public int currentTime = 0;
    public int nextActionTime = 0;
    public int actionCost = 10;
    public float actionCostMultiplier = 1f;
    public bool ActionInProgress { get; private set; }
    public Tile CurTile => Stats?.CurTile;
    

    public CharacterStats Stats;
    private int? _totalCost = null;

    protected virtual void Awake()
    {
        Stats = GetComponent<CharacterStats>();
    }

    public virtual void Init()
    {
        TurnManager.Instance.allUnits.Add(this);
        currentTime = TurnManager.Instance.globalTime;
        nextActionTime = currentTime + actionCost;
    }
    public abstract void PerformAction();
    public void SetNextActionCost(int cost)
    {
        _totalCost = cost;
    }
    public virtual int GetModifiedActionCost()
    {
        if (_totalCost.HasValue)
        {
            int cost = _totalCost.Value;
            _totalCost = null;            // 한 번 쓰고 자동 초기화
            return cost;
        }
        return Mathf.Max(1, Mathf.RoundToInt(actionCost * actionCostMultiplier));
    }

    public void StartTurn()
    {
        PerformAction();
    }

    public void AdvanceTime(int cost)
    {
        currentTime = nextActionTime;
        nextActionTime += cost;
    }

    public void OnTurnEnd()
    {
        if (ActionInProgress)
            ActionInProgress = false;
    }

    public bool IsPlayer => this is PlayerUnit;


    /*public float BaseActionPoints = 1.0f; // 기본 액션 포인트
    public float ActionPoints { get; private set; } // 최종 액션 포인트

    private List<BuffEffectInstance> _activeEffects = new List<BuffEffectInstance>();
    // _buffPoint가 null이면 효과 없음, null이 아니면 BaseActionPoints에 효과들을 누적한 값
    private float? _buffPoint = null;

    public float GetFinalActionPoints() => _buffPoint.HasValue ? _buffPoint.Value : BaseActionPoints;

    // 기존 ApplyBuff: BuffEffect 객체를 인수로 받음
    public void ApplyBuff(BuffEffect newBuff)
    {
        if (!_buffPoint.HasValue)
            _buffPoint = BaseActionPoints;

        BuffEffectInstance existing = _activeEffects.Find(e => e.buffEffect.effectType == newBuff.effectType);
        if (existing != null)
        {
            if (newBuff.stackable)
                existing.AddEffect(newBuff);
            else
                existing.OverwriteEffect(newBuff);
        }
        else
        {
            _activeEffects.Add(new BuffEffectInstance(newBuff));
        }
        RecalculateBuffPoint();
    }

    // 오버로딩: effectValue와 duration을 인수로 받아 처리
    // duration이 0이면 즉시 소비하여 _buffPoint에 바로 반영
    public void ApplyBuff(float effectValue, int duration)
    {
        if (duration == 0)
        {
            if (!_buffPoint.HasValue)
                _buffPoint = BaseActionPoints;
            _buffPoint += effectValue;
            ActionPoints = _buffPoint.Value;
        }
        else
        {
            // 임시 효과 객체 생성
            // 여기서는 효과 타입으로 일단 AttackDecrease를 사용합니다.
            BuffEffect tempBuff = new BuffEffect(BuffEffectType.AttackDecrease, null, effectValue, duration, false);
            ApplyBuff(tempBuff);
        }
    }

    // 턴 종료 시 효과 지속 시간 업데이트 및 만료 효과 제거
    public void UpdateBuffs()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            _activeEffects[i].DecrementDuration();
            if (_activeEffects[i].remainingTurns <= 0)
                _activeEffects.RemoveAt(i);
        }
        RecalculateBuffPoint();
    }

    // 모든 활성 효과의 효과 값을 누적하여 _buffPoint를 재계산
    private void RecalculateBuffPoint()
    {
        if (_activeEffects.Count == 0)
            _buffPoint = null;
        else
        {
            float sum = 0;
            foreach (BuffEffectInstance effectInst in _activeEffects)
                sum += effectInst.effectValue;
            _buffPoint = BaseActionPoints + sum;
        }
        ActionPoints = _buffPoint.HasValue ? _buffPoint.Value : BaseActionPoints;
    }*/
}
