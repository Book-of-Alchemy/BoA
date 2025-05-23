using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioMixer mixer;  // MasterMixer 에셋 할당
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("사운드 리스트")]
    public List<SoundData> SoundDatas = new List<SoundData>();
    Dictionary<string, SoundData> map;

    [Range(0f, 1f)] public float masterVolume;
    [Range(0f, 1f)] public float bgmVolume;
    [Range(0f, 1f)] public float sfxVolume;

    //고정 사운드
    [Range(0f, 1f)] private float fixedBGMVolume = 0.5f;
    [Range(0f, 1f)] private float fixedSFXVolume = 0.5f;


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


            for (int i = 0; i < 2; i++)
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

    private void OnEnable()
    {
        GameSceneManager.Instance.OnSceneTypeChanged += OnSceneChange;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.OnSceneTypeChanged -= OnSceneChange;
    }

    public void Play(string id)
    {
        if (!map.TryGetValue(id, out SoundData data)) return;

        switch(data.type)
        {
            case SoundType.SFX:
                AudioPool.Instance.PlayeOneShot(data.clip, fixedSFXVolume, GetGroup(data.type));
                SetMasterVolume(masterVolume);
                SetBGMVolume(sfxVolume);
                break;
            case SoundType.BGM:
                PlayBGM(data);
                SetMasterVolume(masterVolume);
                SetBGMVolume(bgmVolume);
                break;
        }
    }

    public void PlayBGM(SoundData data)
    {
        int next = 1 - activeMusic;
        var from  = BGMSources[activeMusic];
        var to = BGMSources[next];

        to.clip = data.clip;
        to.outputAudioMixerGroup = GetGroup(data.type);
        to.loop = true;
        to.volume = fixedBGMVolume;
        to.Play();



        if (from.isPlaying)
            StartCoroutine(Crossfade(from, to));
        else
        {
            from.Stop();
            to.volume = fixedBGMVolume;
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
            to.volume = Mathf.Lerp(0f, fixedBGMVolume, pct);
            yield return null;
        }
        from.Stop();
    }

    private AudioMixerGroup GetGroup(SoundType type)
    {
        return type switch
        {
            SoundType.BGM => bgmGroup,
            SoundType.SFX => sfxGroup,
            _ => masterGroup,
        };
    }

    public void SetMasterVolume(float linear)
    {
        mixer.SetFloat("MasterVolume", linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f);
        masterVolume = linear;
    }

    public void SetBGMVolume(float linear)
    {
        mixer.SetFloat("BGMVolume", linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f);
        bgmVolume = linear;
    }

    public void SetSFXVolume(float linear)
    {
        mixer.SetFloat("SFXVolume", linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f);
        sfxVolume = linear;
    }

    public void OnSceneChange(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.MainMenu:
                Play("Village");
                break;
            case SceneType.Town:
                Play("Village");
                break;
            case SceneType.Dungeon:
                Play(GetProperBGM(QuestManager.Instance.AcceptedQuest));
                break;
            default:
                Play("Village");
                break;
        }
    }

    string GetProperBGM(QuestProgress progress)
    {
        if (progress == null) return "Village";
        return progress.Data.biome_id switch
        {
            120001 => "Forest",
            _ => "Village",
        };

    }

}
