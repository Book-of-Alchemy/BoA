using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTrap : TrapBase
{
    public override void Action()
    {
        effectTiles = TileUtility.GetItemRangedTile(tile.curLevel, tile, trapData.effect_range, true);
        DamageType damageType = (DamageType)(trapData.effect_value == -1 ? 0 : trapData.effect_value);

        foreach (var effect in effectTiles)
        {
            if (effect == null)
                continue;
            DamageInfo damageInfo = new DamageInfo(trapData.damage, damageType, creatorStats, effect.CharacterStatsOnTile, false, new Tag[] { Tag.Trap });
            //버프 디버프 적용 추가
            if (effect.CharacterStatsOnTile == null)
                continue;
            effect.CharacterStatsOnTile.TakeDamage(damageInfo);
        }
        SoundManager.Instance.Play("trap active");
    }
}
