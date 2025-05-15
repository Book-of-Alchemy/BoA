using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDetector : Artifact
{
    public TrapDetector(ArtifactData data) : base(data)
    {
    }
    // 아티팩트 획득시
    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        foreach(var level in TileManger.Instance.levels)
        {
            foreach(var tile in level.tiles)
            {
                float random = Random.value;

                if(random > 0.1f)
                    tile.Value.TrpaOnTile.IsDetected = true;
            }
        }
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
    }
}
