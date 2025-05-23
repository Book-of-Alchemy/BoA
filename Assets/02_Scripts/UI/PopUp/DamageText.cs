using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class DamageText : UIBase //데미지Text를 띄우는 UI 클래스
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _duration = 1.0f;
    [SerializeField] private float _jumpDistance = 70f;
    [SerializeField] private float _jumpPower = 150f;

    public override void Opened(params object[] param)
    {
        //필요한 값 초기화
        float damage = (float)param[0];
        Vector3 worldPos = (Vector3)param[1];
        int a = (int)Math.Round(damage);

        //Text 값, 위치 변경
        _text.text = a.ToString();
        transform.position = Camera.main.WorldToScreenPoint(worldPos);

        //50% 확률로 왼쪽 또는 오른쪽으로 방향 저장
        Vector3 direction = new Vector3(UnityEngine.Random.value < 0.5f ? -1f : 1f, 1f, 0f).normalized;
        //텍스트 이동 방향
        Vector3 targetPos = transform.position + direction * _jumpDistance;

        //targetPos 이동하며 _duration에 따라 서서히 사라짐
        _text.DOFade(0f, _duration);
        transform.DOJump(targetPos, _jumpPower, 1, _duration).SetEase(Ease.OutQuad);
    }

    public override void HideDirect()
    {

    }
}