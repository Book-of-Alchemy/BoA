using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnvRule/EnvRuleDataBase")]
public class EnvRuleDataBase : ScriptableObject
{
    public List<EnvRuleData> envRuleDatas;

    public Dictionary<(DamageType, EnvironmentType), EnvRuleData> ruleDic = new Dictionary<(DamageType, EnvironmentType), EnvRuleData>();

    private void OnValidate()
    {
        ruleDic.Clear();
        if (envRuleDatas == null) return;

        foreach (var ruleData in envRuleDatas)
        {
            if (ruleData == null) continue;

            if (ruleDic.ContainsKey((ruleData.reactionDamageType, ruleData.sourceTileType)))
            {
                Debug.LogWarning($"중복되는 룰이 존재합니다.");
            }

            ruleDic[(ruleData.reactionDamageType, ruleData.sourceTileType)] = ruleData;
        }
    }
}
