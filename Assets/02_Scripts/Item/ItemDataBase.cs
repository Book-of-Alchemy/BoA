using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data Base")]
public class ItemDataBase : ScriptableObject
{
    public List<ItemData> itemDatas;
    public List<RecipeData> recipeDatas;
    public GameObject typeObjectPrefab;
    public Dictionary<int, ItemData> dicItemData = new Dictionary<int, ItemData>();
    private int intNullvalue = -1;
}
