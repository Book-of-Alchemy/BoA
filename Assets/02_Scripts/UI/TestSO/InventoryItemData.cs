using UnityEngine;

public enum eTestItemType
{
    Armor,
    Consumable,
    Quest
}

[CreateAssetMenu(fileName = "TestItem", menuName= "Inventory/New TestItem")]
public class InventoryItemData : ScriptableObject // 아이템의 데이터를 가진 클래스
{
    public int Id => _id;
    public string Name => _name;
    public eTestItemType type => _type;
    public string description => _description;
    public Sprite Icon => _icon;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private eTestItemType _type;
    [SerializeField] [TextArea] private string _description;
    [SerializeField] private Sprite _icon;

}
