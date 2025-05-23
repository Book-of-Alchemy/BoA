
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactDataBase", menuName = "Artifact/ArtifactDataBase")]
public class ArtifactDataBase : ScriptableObject
{
    public List<ArtifactData> artifacts = new List<ArtifactData>();
    public Dictionary<int, ArtifactData> artifactsDataById = new Dictionary<int, ArtifactData>();

    private void OnEnable()
    {
        ArrangeData();
    }

    void ArrangeData()
    {
        artifactsDataById.Clear();

        foreach (var data in artifacts)
        {
            if (data == null) continue;

            if (!artifactsDataById.ContainsKey(data.id))
            {
                artifactsDataById.Add(data.id, data);
            }
            else
            {
            }
        }
    }

    public ArtifactData GetStatusEffectById(int id)
    {
        artifactsDataById.TryGetValue(id, out var data);
        return data;
    }
}
