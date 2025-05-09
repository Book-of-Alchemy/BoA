using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueText; // 대화 텍스트
    [SerializeField] private Button _skipBtn;
    [SerializeField] private float _textSpeed = 0.1f; // 한 글자 표시 간격

    //트윈 조작용 변수
    private Tween _typingTween;

    private void Awake()
    {
        if(_skipBtn == null) //null체크 후 이벤트 등록
        {
            TryGetComponent<Button>(out _skipBtn);
            _skipBtn.onClick.AddListener(SkipDialogue);
        }
    }

    public void ShowDialogue(string sentence)
    {
        // 기존에 트윈이 완료 후 재시작
        _typingTween?.Kill();
        _dialogueText.text = string.Empty;

        _dialogueText.text = sentence;          // 문장 전체를 세팅
        _dialogueText.maxVisibleCharacters = 0; // 보이는 글자 수 0

        // duration = 글자 길이 * 글자당 시간
        float duration = sentence.Length * _textSpeed;
        //보여지는 글자 보간 0 ~ 글자 길이
        _typingTween = DOTween
            .To(x => _dialogueText.maxVisibleCharacters = (int)x, 0, sentence.Length, duration)
            .OnComplete(() =>{_typingTween = null;});
    }

    public void SkipDialogue()
    {
        if (_typingTween != null)
        {
            _typingTween.Complete();  // 트윈 즉시종료
            _typingTween = null;
        }
    }
}
