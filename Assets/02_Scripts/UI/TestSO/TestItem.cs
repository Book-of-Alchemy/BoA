using UnityEngine;

public enum eTestItemType
{
    Armor,
    Consumable,
    Quest
}

[CreateAssetMenu(fileName = "TestItem", menuName= "Inventory/New TestItem")]
public class TestItem : ScriptableObject // 아이템의 데이터를 가진 클래스
{
    public string Name;
    public eTestItemType type;
    [TextArea]public string description;
    public int maxStack;
    public bool isEquippable;

    public Sprite _Icon;
}
