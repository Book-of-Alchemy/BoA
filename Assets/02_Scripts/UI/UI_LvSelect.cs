using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UI_LvSelect : UIBase
{
    // Inspector 참조
    [SerializeField] private UIAnimator _uiAnimator; 
    [SerializeField] private RectTransform _selector; //화살표 이미지

    [Header("ArtifactPanel")]
    [SerializeField] private List<ArtifactPanelUI> _panels;

    public override void HideDirect()
    {
        _uiAnimator.FadeOut(OnFadeOut);
    }

    public override void Opened(params object[] param)
    {
        ArtifactData[] data = ArtifactFactory.RandomArtifacts();
        for (int i = 0; i < 3; i++)
        {
            _panels[i].SetData(data[i], OnArtifactSelected);
            _panels[i].OnBtnSelected += MoveSelectorTo;
        }

        _uiAnimator.SlideFromY(()=> _panels[0]._btn.Select());
    }

    private void OnFadeOut()
    {
        UIManager.Hide<UI_LvSelect>();
    }

    private void OnArtifactSelected(ArtifactData data) //아티팩트가 선택되면 호출
    {
        //아티팩트 획득 및 UI FadeOut
        ArtifactFactory.EquipArtifact(data.id);
        _uiAnimator.FadeOut(OnFadeOut);
    }

    private void MoveSelectorTo(RectTransform target)
    {
        Vector3 basePos = target.position;
        _selector.DOMoveY(basePos.y, 0f);
    }

    //Inspector 버튼 이벤트
    public void OnClickBtn() => _uiAnimator.FadeOut(OnFadeOut);
}
