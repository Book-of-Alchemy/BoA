using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Text : UIBase
{
    [SerializeField] private TextMeshProUGUI _text;

    public override void HideDirect()
    {

    }

    public override void Opened(params object[] param)
    {
        //필요한 값 초기화
        string inText = param[0].ToString();

        _text.text = inText;
        _text.DOFade(0f, 2f);
        transform.DOScale(Vector3.one * 1.3f, 0.7f);
        ////50% 확률로 왼쪽 또는 오른쪽으로 방향 저장
        //Vector3 direction = new Vector3(UnityEngine.Random.value < 0.5f ? -1f : 1f, 1f, 0f).normalized;
        ////텍스트 이동 방향
        //Vector3 targetPos = transform.position + direction * _jumpDistance;

        ////targetPos 이동하며 _duration에 따라 서서히 사라짐
        //_text.DOFade(0f, _duration);
        //transform.DOJump(targetPos, _jumpPower, 1, _duration).SetEase(Ease.OutQuad);
    }
}
