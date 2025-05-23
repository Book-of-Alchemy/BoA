using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectDataBase", menuName = "StatusEffect/StatusEffectDataBase")]
public class StatusEffectDataBase : ScriptableObject
{
    public List<StatusEffectData> statusEffects = new List<StatusEffectData>();
    public Dictionary<int, StatusEffectData> statusEffectsDataById = new Dictionary<int, StatusEffectData>();

    private void OnValidate()
    {
        ArrangeData();
    }

    void ArrangeData()
    {
        statusEffectsDataById.Clear();

        foreach (var data in statusEffects)
        {
            if (data == null) continue;

            if (!statusEffectsDataById.ContainsKey(data.id))
            {
                statusEffectsDataById.Add(data.id, data);
            }
            else
            {
            }
        }
    }

    public StatusEffectData GetStatusEffectById(int id)
    {
        statusEffectsDataById.TryGetValue(id, out var data);
        return data;
    }
}