
using UnityEngine;

[CreateAssetMenu(menuName = "EnvRule/EnvRule")]
public class EnvRuleData : ScriptableObject
{
    public int Id;
    public EnvironmentType sourceTileType;
    public DamageType reactionDamageType;
    public int effect_ID = -1;
    public EnvironmentType resultTileType;
    public float damageMultiplier;
    public DamageType emittedDamageType;
}
