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

    //---------- FUNCTIONS ----------\\

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
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
        AudioSourceCoroutine(audioSource);
        
    }

    private IEnumerable AudioSourceCoroutine(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
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