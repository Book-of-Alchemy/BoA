using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/EnvironmentalDataBase")]
public class EnvironmentalDataBase : ScriptableObject
{
    public List<EnvironmentalData> environmentalDatas = new List<EnvironmentalData>();
    public GameObject environmentalPrefab;

    public Dictionary<EnvironmentType, EnvironmentalData> datasByType = new Dictionary<EnvironmentType, EnvironmentalData>();

    private void OnEnable()
    {
        datasByType.Clear();

        foreach (var data in environmentalDatas)
        {
            if (data == null)
            {
                continue;
            }

            if (datasByType.ContainsKey(data.environment_type))
            {

            }
            datasByType[data.environment_type] = data;
        }
    }
}
