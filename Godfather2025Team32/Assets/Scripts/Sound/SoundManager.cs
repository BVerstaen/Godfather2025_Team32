using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    static private SoundManager _instance;
    static public SoundManager Instance { get { return _instance; } private set { _instance = value; } }

    [SerializeField] private SoundStruct[] _allSoundStructs;
    private List<AudioSource> _audioSourcesPool = new List<AudioSource>();
    [SerializeField] private int _numAudioSourcesInPool = 10;
    private List<AudioSource> _freeAudioSources = new List<AudioSource>();
    [SerializeField] private GameObject _audioSourcesParent;
    private Dictionary<SoundEnum, AudioSource> _dicoInfiniteAudioSources = new Dictionary<SoundEnum, AudioSource>();

    //---------- FUNCTIONS ----------\\

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayInfiniteLoop(SoundEnum soundEnum)
    {
        AudioClip audioClip = GetAudioCLip(soundEnum);
        AudioSource audioSource = GetAnAudioFromPool();
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.loop = true;
        _dicoInfiniteAudioSources[soundEnum] = audioSource;
    }

    public void StopInfiniteLoop(SoundEnum soundEnum)
    {
        AudioSource audioSource = _dicoInfiniteAudioSources[soundEnum];
        _dicoInfiniteAudioSources.Remove(soundEnum);
        PutAudioSourceInPool(audioSource);
    }

    private AudioClip GetAudioCLip(SoundEnum soundEnum)
    {
        foreach (SoundStruct soundStruct in _allSoundStructs)
        {
            if (soundStruct.sound == soundEnum) return soundStruct.audioClip;
        }
        return null;
    }

    public void PlaySound(SoundEnum soundEnum)
    {
        AudioClip audioClip = GetAudioCLip(soundEnum);
        AudioSource audioSource = GetAnAudioFromPool();
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.loop = false;
        AudioSourceCoroutine(audioSource);
    }

    private IEnumerable AudioSourceCoroutine(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        PutAudioSourceInPool(audioSource);
    }

    private void PutAudioSourceInPool(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = null;
        _freeAudioSources.Add(audioSource);
    }

    private void SetPool()
    {
        AudioSource currentSource;
        for (int i = 0; i < _numAudioSourcesInPool; i++)
        {
            currentSource = _audioSourcesParent.AddComponent<AudioSource>();
            _audioSourcesPool.Add(currentSource);
            _freeAudioSources.Add(currentSource);
        }
    }

    private AudioSource GetAnAudioFromPool()
    {
        AudioSource audioSource;
        if (_freeAudioSources.Count == 0)
        {
            audioSource = _audioSourcesParent.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = _freeAudioSources[0];
            _freeAudioSources.Remove(audioSource);
        }
        return audioSource;
    }
}