using UnityEngine;
using System.Collections;

public enum EnemyAIType
{
    BasicTracking,
    // 확장 예정: Ranged, Patrol 등
}

public class EnemyController : MonoBehaviour
{
    public EnemyAIType AIType = EnemyAIType.BasicTracking;
    public float MoveSpeed = 3f;      // 적 이동 속도
    public float DetectionRange = 10f; // 플레이어 인식 범위 (그리드 단위)

    private bool _isMoving = false;
    private PlayerStats _playerStats;

    void Start()
    {
        // TurnManager에 등록된 플레이어 참조, 없으면 씬 내 검색
        if (TurnManager.Instance != null && TurnManager.Instance.Player != null)
            _playerStats = TurnManager.Instance.Player;
        else
            _playerStats = FindObjectOfType<PlayerStats>();
    }

    // TurnManager에서 호출, 적 턴 동안 실행할 루틴
    public IEnumerator TakeTurn()
    {
        switch (AIType)
        {
            case EnemyAIType.BasicTracking:
                yield return StartCoroutine(HandleBasicTracking());
                break;
            // 확장: 다른 AI 유형 추가 가능
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }
        yield break;
    }

    // 기본 추적형 AI
    // 플레이어가 인식 범위 내에 있으면 한 셀씩 이동하여 추적, 아니면 대기
    private IEnumerator HandleBasicTracking()
    {
        Vector3 enemyGridPos = GetGridCenter(transform.position);
        Vector3 playerGridPos = _playerStats != null ? GetGridCenter(_playerStats.transform.position) : enemyGridPos;
        float distance = Vector2.Distance(new Vector2(enemyGridPos.x, enemyGridPos.y),
                                          new Vector2(playerGridPos.x, playerGridPos.y));

        if (distance <= DetectionRange)
        {
            // 플레이어 방향으로 한 셀 이동 (대각 이동 가능)
            Vector3 delta = playerGridPos - enemyGridPos;
            float dirX = Mathf.Abs(delta.x) > 0.1f ? Mathf.Sign(delta.x) : 0f;
            float dirY = Mathf.Abs(delta.y) > 0.1f ? Mathf.Sign(delta.y) : 0f;

            Vector3 nextTarget = enemyGridPos + new Vector3(dirX, dirY, 0);
            // 확장: 장애물 체크 등 추가 처리 가능

            yield return StartCoroutine(MoveToTarget(nextTarget));
        }
        else
        {
            // 플레이어 감지 실패 시 잠깐 대기
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    // 입력 좌표를 기준으로 그리드 셀 중앙 좌표 계산  
    private Vector3 GetGridCenter(Vector3 pos)
    {
        float x = Mathf.Floor(pos.x) + 0.5f;
        float y = Mathf.Floor(pos.y) + 0.5f;
        return new Vector3(x, y, pos.z);
    }

    // 대상 좌표까지 부드럽게 이동하는 코루틴  
    private IEnumerator MoveToTarget(Vector3 target)
    {
        _isMoving = true;
        while ((target - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        _isMoving = false;
        yield break;
    }
}
