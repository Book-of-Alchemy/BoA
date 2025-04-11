using UnityEngine;

public enum eTestItemType
{
    Armor,
    Consumable,
    Quest
}

[CreateAssetMenu(fileName = "TestItem", menuName= "New TestItem")]
public class TestItem : ScriptableObject
{
    public string name;
    public eTestItemType type;
    [TextArea]public string description;
    public int maxStack;
    public bool isEquippable;

    public Sprite _Icon;
}
