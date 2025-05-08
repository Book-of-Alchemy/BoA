// PlayerBaseBehavior.cs

using UnityEngine;

/// <summary>
/// 모든 플레이어 행동(Behavior)의 공통 추상 클래스
/// InputSystem 콜백 대신 InputManager 이벤트 방식으로 동작합니다.
/// 자식 클래스는 SubscribeInput/UnsubscribeInput 만 구현하세요.
/// </summary>
public abstract class PlayerBaseBehavior : MonoBehaviour
{
    /// <summary>이 Behavior를 소유한 PlayerController</summary>
    protected PlayerController Controller;
    /// <summary>입력 매니저 shortcut</summary>
    protected InputManager InputManager => InputManager.Instance;

    /// <summary>
    /// PlayerController에서 호출하여 초기화합니다.
    /// </summary>
    public virtual void Initialize(PlayerController controller)
    {
        Controller = controller;
        // 자식에서 SubscribeInput() 호출
    }

    /// <summary>입력 이벤트 구독</summary>
    protected abstract void SubscribeInput();

    /// <summary>입력 이벤트 구독 해제</summary>
    protected abstract void UnsubscribeInput();

    /// <summary>
    /// MonoBehaviour 비활성화 시 구독 해제 자동 호출
    /// </summary>
    protected virtual void OnDisable()
    {
        UnsubscribeInput();
    }
}
