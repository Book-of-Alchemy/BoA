using UnityEngine;
using UnityEngine.Audio;

public enum SoundType { BGM, SFX}

[CreateAssetMenu(menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    [Header("식별")]
    public string id;

    [Header("클립 및 타입")]
    public SoundType type;
    public AudioClip clip;

    [Header("공통 설정")]
    [Range(0f,1f)]public float volume = 1f;
    [Range(0.5f, 2f)]public float pitch = 1f;

    [Header("BGM 전용")]
    public bool loop = true;
    public bool crossfadeOnChange = true;

    [Header("AudioMixer")]
    public AudioMixerGroup mixerGroup;
}
