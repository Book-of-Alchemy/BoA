using UnityEngine;

public class EnemyStats : CharacterStats
{
    public void OnDeath()
    {
        Debug.Log(gameObject.name + " 가 사망");
    }
}
