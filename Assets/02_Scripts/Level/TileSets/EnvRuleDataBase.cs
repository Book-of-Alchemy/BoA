using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnvRule/EnvRuleDataBase")]
public class EnvRuleDataBase : ScriptableObject
{
    public List<EnvRuleData> envRuleDatas;

    public Dictionary<(DamageType, EnvironmentType), EnvRuleData> ruleDic = new Dictionary<(DamageType, EnvironmentType), EnvRuleData>();

    private void OnEnable()
    {
        Arrange();
    }

    public void Arrange()
    {
        ruleDic.Clear();

        foreach (var ruleData in envRuleDatas)
        {
            if (ruleData == null) continue;

            ruleDic[(ruleData.reactionDamageType, ruleData.sourceTileType)] = ruleData;
        }
    }
}
