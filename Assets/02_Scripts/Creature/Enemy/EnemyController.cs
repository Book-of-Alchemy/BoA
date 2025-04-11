using UnityEngine;
using System.Collections;

public enum EnemyAIType
{
    BasicTracking,
    // 확장 예정: Ranged, Patrol 등 추가 가능
}

public class EnemyController : MonoBehaviour
{
    public EnemyAIType AIType = EnemyAIType.BasicTracking;
    public float MoveSpeed = 3f;      // 기본 이동 속도 (카드널 기준)
    public float DetectionRange = 10f; // 플레이어 인식 범위 (그리드 단위)

    private bool _isMoving = false;
    private PlayerStats _playerStats;

    void Start()
    {
        // TurnManager의 플레이어 또는 씬 내의 PlayerStats 검색
        if (TurnManager.Instance != null && TurnManager.Instance.Player != null)
            _playerStats = TurnManager.Instance.Player;
        else
            _playerStats = FindObjectOfType<PlayerStats>();
    }

    // TurnManager에서 호출하는 적 턴 루틴
    public IEnumerator TakeTurn()
    {
        switch (AIType)
        {
            case EnemyAIType.BasicTracking:
                yield return StartCoroutine(HandleBasicTracking());
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }
        yield break;
    }

    // 기본 추적형 AI: 플레이어가 인식 범위 내이면 한 셀 단위 직선으로 이동
    private IEnumerator HandleBasicTracking()
    {
        Vector3 enemyCenter = GetGridCenter(transform.position);
        Vector3 playerCenter = _playerStats != null ? GetGridCenter(_playerStats.transform.position) : enemyCenter;
        float distance = Vector2.Distance(new Vector2(enemyCenter.x, enemyCenter.y), new Vector2(playerCenter.x, playerCenter.y));

        if (distance <= DetectionRange)
        {
            Vector3 delta = playerCenter - enemyCenter;
            float dirX = Mathf.Abs(delta.x) > 0.1f ? Mathf.Sign(delta.x) : 0f;
            float dirY = Mathf.Abs(delta.y) > 0.1f ? Mathf.Sign(delta.y) : 0f;

            bool isDiagonal = (dirX != 0f && dirY != 0f);
            Vector3 targetCell = enemyCenter + new Vector3(dirX, dirY, 0);
            yield return StartCoroutine(MoveToTarget(targetCell, isDiagonal));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    // 현재 pos를 기준으로 그리드 셀 중앙 좌표 반환
    private Vector3 GetGridCenter(Vector3 pos)
    {
        float x = Mathf.Floor(pos.x) + 0.5f;
        float y = Mathf.Floor(pos.y) + 0.5f;
        return new Vector3(x, y, pos.z);
    }

    // 대상 셀까지 부드럽게 이동하는 코루틴  
    // MoveSpeed는 카드널 이동 속도이며, 대각 이동이면 피타고라스 계산에 따라 effective 속도 사용
    private IEnumerator MoveToTarget(Vector3 targetCell, bool isDiagonal = false)
    {
        _isMoving = true;
        // 목표 pos = target 셀의 중심 (0.5 오프셋)
        Vector3 destination = targetCell + new Vector3(0.5f, 0.5f, 0);
        float effectiveSpeed = isDiagonal ? MoveSpeed / Mathf.Sqrt(2f) : MoveSpeed;

        while ((destination - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, effectiveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        _isMoving = false;
        yield break;
    }
}
