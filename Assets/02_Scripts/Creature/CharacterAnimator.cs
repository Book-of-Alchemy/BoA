using UnityEngine;


[RequireComponent(typeof(Animator))]//animator가 null이 되는 현상을 방지해줌
public class CharacterAnimator : MonoBehaviour
{
    private Animator _anim;
    // 애니메이터 파라미터 이름을 문자열 대신 해시(int)로 저장, 런타임에 문자열 비교과정이 생략되고 해시 비교만으로 파라미터를 제어가능함.
    private static readonly int MoveTriggerHash = Animator.StringToHash("Move");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int ThrowTriggerHash = Animator.StringToHash("Throw");
    private static readonly int DeathTriggerHash = Animator.StringToHash("Death");
    private static readonly int KnockBackTriggerHash = Animator.StringToHash("KnockBack");
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void PlayMove()
    {
        _anim.SetTrigger(MoveTriggerHash);
    }

    public void PlayAttack()
    {
        _anim.SetTrigger(AttackTriggerHash);
    }

    public void PlayThrow()
    {
        _anim.SetTrigger(ThrowTriggerHash);
    }
    public void PlayKnockBack()
    {
        _anim.SetTrigger(KnockBackTriggerHash);
    }
    public void PlayDeath()
    {
        _anim.SetTrigger(DeathTriggerHash);
    }
    
}
