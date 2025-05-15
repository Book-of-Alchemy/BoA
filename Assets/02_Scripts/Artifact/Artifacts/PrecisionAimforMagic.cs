using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionAimforMagic : Artifact
{
    public PrecisionAimforMagic(ArtifactData data) : base(data)
    {
    }

    public override void Equip(PlayerStats player)
    {
        base.Equip(player);
        player.isPrecisionAim = true;
    }

    public override void UnEquip(PlayerStats player)
    {
        base.UnEquip(player);
        player.isPrecisionAim = false;
    }
}
