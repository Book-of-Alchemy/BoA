using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UIBase
{
    public Slider masterSlider;
    public Slider bgmslider;
    public Slider sfxSlider;
    
    public override void HideDirect()//Call at OnClick Event 
    {
        UIManager.Hide<UI_Setting>();   
    }

    public override void Opened(params object[] param)
    {
        masterSlider.value = SoundManager.Instance.initialMasterVolume;
        bgmslider.value = SoundManager.Instance.initialBGMVolume;
        sfxSlider.value = SoundManager.Instance.initialSFXVolume;
        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmslider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }

}
