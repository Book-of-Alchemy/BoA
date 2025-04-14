using UnityEngine;

public class EnemyStats : CharacterStats
{
    public override void Die()
    {
        Destroy(gameObject);
    }
}
