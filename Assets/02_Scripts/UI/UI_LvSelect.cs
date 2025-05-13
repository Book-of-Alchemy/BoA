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

    [Header("TestSO")]
    [SerializeField] private List<ArtifactData> _soDatas;

    public override void HideDirect()
    {
        _uiAnimator.FadeOut(OnFadeOut);
    }

    public override void Opened(params object[] param)
    {

        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, _soDatas.Count);
            _panels[i].SetData(_soDatas[rand], OnArtifactSelected);
            _panels[i].OnBtnSelected += MoveSelectorTo;
            _soDatas.RemoveAt(rand);
        }

        _uiAnimator.SlideFromY(()=> _panels[0]._btn.Select());
    }

    private void OnFadeOut()
    {
        UIManager.Hide<UI_LvSelect>();
    }

    private void OnArtifactSelected(ArtifactData data) //아티팩트가 선택되면 호출
    {
        //아티팩트 data 획득 함수 추가 필요
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
