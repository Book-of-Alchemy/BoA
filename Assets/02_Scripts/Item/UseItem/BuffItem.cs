
public class BuffItem : BaseItem
{
    public override void UseItem(ItemData data)
    {
        Inventory.Instance.RemoveItem(
    Inventory.Instance.GetItemIndex(data.id)
    );
        StatusEffectFactory.CreateEffect(data.effect_id, data.effect_strength, data.effect_duration, 10, GameManager.Instance.PlayerTransform);
        FinishUse();
        Destroy(this.gameObject);
    }
    public override void CancelUse()
    {
    }
}
