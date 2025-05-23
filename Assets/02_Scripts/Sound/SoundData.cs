using UnityEngine;

public enum SoundType { BGM, SFX }

[CreateAssetMenu(menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    [Header("식별")]
    public string id;

    [Header("클립 및 타입")]
    public SoundType type;
    public AudioClip clip;
    public string audioClipPath;
}
