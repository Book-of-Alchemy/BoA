using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : Buff
{
    SpriteRenderer spriteRenderer;
    public override void OnApply(CharacterStats target)
    {
        base.OnApply(target);
        if (!shouldRegister) return;
        target.isHidden = true;
        spriteRenderer = target.GetComponent<SpriteRenderer>();//임시 반투명 처리 향후 쉐이더 적용예정
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f,1f, 1f, 0.7f);
        }
    }

    public override void OnExpire(CharacterStats target)
    {
        target.isHidden = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
