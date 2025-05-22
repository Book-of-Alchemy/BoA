using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

[CreateAssetMenu(menuName = "Research/ResearchDataBase")]

public class ResearchDataBase : ScriptableObject
{
    public List<ResearchData> researchData;

    public ResearchData GetDataByStatType(StatType type)
    {
        return researchData.FirstOrDefault(r => r.stat_type == type);
    }
}
