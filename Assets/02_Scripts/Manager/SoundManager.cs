using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioMixer mixer;  // MasterMixer 에셋 할당
    [Header("사운드 리스트")]
    public List<SoundData> SoundDatas = new List<SoundData>();
    Dictionary<string, SoundData> map;

    [Range(0f, 1f)] public float initialMasterVolume = 1f;
    [Range(0f, 1f)] public float initialBGMVolume = 1f;
    [Range(0f, 1f)] public float initialSFXVolume = 1f;

    AudioSource[] BGMSources = new AudioSource[2];
    int activeMusic = 0;
    [SerializeField] float crossfadeTime = 1.5f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            map = SoundDatas.ToDictionary(d => d.id, d => d);

            for(int i = 0; i < 2; i++)
            {
                var go = new GameObject($"BGM_Source_{i}");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                BGMSources[i] = src;
            }
        }
        else 
            Destroy(gameObject);
    }

    public void Play(string id)
    {
        if (!map.TryGetValue(id, out SoundData data)) return;

        switch(data.type)
        {
            case SoundType.SFX:
                AudioPool.Instance.PlayeOneShot(data.clip, data.volume, data.mixerGroup);
                break;
            case SoundType.BGM:
                PlayBGM(data);
                break;
        }
    }

    public void PlayBGM(SoundData data)
    {
        int next = 1 - activeMusic;
        var from  = BGMSources[activeMusic];
        var to = BGMSources[next];

        to.clip = data.clip;
        to.outputAudioMixerGroup = data.mixerGroup;
        to.loop = data.loop;
        to.volume = data.volume;
        to.pitch = data.pitch;
        to.Play();

        if (data.crossfadeOnChange && from.isPlaying)
            StartCoroutine(Crossfade(from, to));
        else
        {
            from.Stop();
            to.volume = data.volume;
        }

        activeMusic = next;

    }

    IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float t = 0f;
        float startVol = from.volume;
        while (t < crossfadeTime)
        {
            t += Time.deltaTime;
            float pct = t / crossfadeTime;
            from.volume = Mathf.Lerp(startVol, 0f, pct);
            to.volume = Mathf.Lerp(0f, to.volume, pct);
            yield return null;
        }
        from.Stop();
    }

    public void SetMasterVolume(float linear)
    {
        mixer.SetFloat("MasterVolume", linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f);
        initialMasterVolume = linear;
    }

    public void SetBGMVolume(float linear)
    {
        mixer.SetFloat("BGMVolume", linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f);
        initialBGMVolume = linear;
    }

    public void SetSFXVolume(float linear)
    {
        mixer.SetFloat("SFXVolume", linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f);
        initialSFXVolume = linear;
    }

}
