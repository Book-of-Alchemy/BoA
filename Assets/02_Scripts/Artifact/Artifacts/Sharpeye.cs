
using UnityEngine;

public class Sharpeye : Artifact
{
    public Sharpeye(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.OnAttack += ActiveSharpeye;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.OnAttack -= ActiveSharpeye;
    }
    
    public int CalculateManhattanDistance(Vector2Int a,Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public void ActiveSharpeye(DamageInfo damageInfo)
    {
        int distance = CalculateManhattanDistance(damageInfo.source.CurTile.gridPosition,damageInfo.target.CurTile.gridPosition);
        if (distance >= 3)
        {
            modifier = new StatModifier("Sharpeye", 70, ModifierType.Precent);
            damageInfo.source.statBlock.AddModifier(StatType.ThrownDmg, modifier);
            damageInfo.target.OnTakeDamage += RemoveSharpeyeModifier;
        }
    }
    public void RemoveSharpeyeModifier(DamageInfo damageInfo)
    {
        damageInfo.source.statBlock.RemoveModifier(StatType.ThrownDmg, modifier);
        damageInfo.target.OnTakeDamage -= RemoveSharpeyeModifier;
    }
}
