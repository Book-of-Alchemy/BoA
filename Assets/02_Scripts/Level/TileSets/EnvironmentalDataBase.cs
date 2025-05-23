using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/EnvironmentalDataBase")]
public class EnvironmentalDataBase : ScriptableObject
{
    public List<EnvironmentalData> environmentalDatas = new List<EnvironmentalData>();
    public GameObject environmentalPrefab;

    public Dictionary<EnvironmentType, EnvironmentalData> datasByType = new Dictionary<EnvironmentType, EnvironmentalData>();

    private void OnValidate()
    {
        datasByType.Clear();

        foreach (var data in environmentalDatas)
        {
            if (data == null)
            {
                Debug.LogWarning("환경 데이터 리스트에 null 항목이 있습니다.");
                continue;
            }

            if (datasByType.ContainsKey(data.environment_type))
                Debug.LogWarning("중복되는 환경타일입니다.");

            datasByType[data.environment_type] = data;
        }
    }
}
