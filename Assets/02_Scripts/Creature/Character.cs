using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public CharacterStats Stats = new CharacterStats();
    protected bool _isMoving;
    public bool IsMoving => _isMoving;

    protected virtual void Start()
    {
        Vector3Int cell = Vector3Int.FloorToInt(transform.position);
        transform.position = cell + new Vector3(0.5f, 0.5f, 0f);
        Stats.Initialize();
    }

    public virtual void TakeDamage(int amount)
    {
        Stats.CurrentHp -= amount;
        if (Stats.CurrentHp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        gameObject.SetActive(false);
    }

    protected IEnumerator Move(Vector2Int dir)
    {
        _isMoving = true;

        Vector3Int curCell = Vector3Int.FloorToInt(transform.position);
        Vector3Int nextCell = curCell + new Vector3Int(dir.x, dir.y, 0);
        Vector3 targetPos = nextCell + new Vector3(0.5f, 0.5f, 0f);

        // 정확한 범위 충돌 검사
        Collider2D hit = Physics2D.OverlapBox(targetPos, new Vector2(0.8f, 0.8f), 0f);
        if (hit != null && hit.GetComponent<Character>() != null)
        {
            Debug.Log("충돌 감지: 캐릭터가 있어 이동 취소됨");
            _isMoving = false;
            yield break;
        }

        // 이동
        while ((transform.position - targetPos).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Stats.MoveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        _isMoving = false;
        TurnManager.Instance.EndTurn();
    }



    public virtual void Attack(Character target)
    {
        if (target == null || target.Stats.CurrentHp <= 0)
            return;

        int finalDamage = Stats.Atk - target.Stats.Def;
        finalDamage = Mathf.Max(finalDamage, 1);

        if (Random.value < Stats.CritRate)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * Stats.CritDmg);
            Debug.Log("치명타!");
        }

        target.TakeDamage(finalDamage);
        Debug.Log($"{gameObject.name}이(가) {target.name}에게 {finalDamage} 피해를 입혔습니다.");
    }
}
