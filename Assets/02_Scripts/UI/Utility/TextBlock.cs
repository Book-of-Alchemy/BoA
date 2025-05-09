using UnityEngine;

public class TextBlock : UIBase
{
    [SerializeField] private TextWriter _typeWriter; // Inspector
    [SerializeField] private UIAnimator _uIAnimator; // Inspector

    public string[] TestText;

    void Start()
    {
        _uIAnimator.SlideFromY(() => { 
        foreach (var text in TestText)
        {
            _typeWriter.ShowDialogue(text);
        } }
        );
    }

    public override void Opened(params object[] param)
    {
        
    }

    public override void HideDirect()
    {
        
    }
}
