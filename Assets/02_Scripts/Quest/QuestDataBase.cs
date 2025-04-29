using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QuestDataBase")]
public class QuestDataBase : ScriptableObject
{
    public List<QuestData> questData;
}
