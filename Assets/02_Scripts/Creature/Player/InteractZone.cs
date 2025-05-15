using System.Collections.Generic;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    private readonly HashSet<Collider2D> _npcs = new HashSet<Collider2D>();

    // 외부에서 읽기 전용으로 NPC 콜라이더 집합에 접근할 수 있도록
    public IReadOnlyCollection<Collider2D> NPCs => _npcs;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
            _npcs.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
            _npcs.Remove(other);
    }
}
