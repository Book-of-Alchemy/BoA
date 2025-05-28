
using System.Linq;

public class HealItem : BaseItem
{
    public override void UseItem(ItemData data)
    {
        Inventory.Instance.RemoveItem(
    Inventory.Instance.GetItemIndex(data.id)
    );
        if (data.tags.Contains(Tag.HP))
            GameManager.Instance.PlayerTransform.Heal(data.effect_value);
        else if(data.tags.Contains(Tag.MP))
            GameManager.Instance.PlayerTransform.ChangeMana(data.effect_value);

        FinishUse();

        var dungeonBehavior = GameManager.Instance.PlayerTransform.GetComponent<DungeonBehavior>();
        if (dungeonBehavior != null)
        {
            dungeonBehavior.CancelItemUse();
        }

        Destroy(this.gameObject);
    }
    public override void CancelUse()
    {
    }

}
