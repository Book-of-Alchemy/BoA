using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioPool : MonoBehaviour
{
    private static AudioPool _instance;
    public static AudioPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioPool>();

                if (_instance == null)
                {
                    _instance = new GameObject(typeof(AudioPool).Name).AddComponent<AudioPool>();
                }

                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    [SerializeField] int initialSize = 10;

    Queue<AudioSource> pool = new Queue<AudioSource>();

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            for (int i = 0; i < initialSize; i++)
                pool.Enqueue(CreateSource());
        }
        else Destroy(gameObject);
    }

    AudioSource CreateSource()
    {
        var go = new GameObject("SFX_Source");
        go.transform.SetParent(transform);
        var source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    public AudioSource Get() => pool.Count > 0 ? pool.Dequeue() : CreateSource();

    public void Return(AudioSource source)
    {
        source.clip = null;
        source.loop = false;
        pool.Enqueue(source);
    }

    public void PlayeOneShot(AudioClip clip, float volume, AudioMixerGroup group)
    {
        var source = Get();
        source.clip = clip;
        source.volume = volume;
        source.outputAudioMixerGroup = group;
        source.Play();
        StartCoroutine(RecycleAfterPlay(source));
    }

    IEnumerator RecycleAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(()=>source.isPlaying);
        Return(source);
    }
    
}
