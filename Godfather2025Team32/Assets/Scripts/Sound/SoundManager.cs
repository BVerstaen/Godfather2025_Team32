using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    private SoundManager _instance;
    public SoundManager Instance { get { return _instance; } private set { _instance = value; } }

    [SerializeField] private SoundStruct[] _allSoundStructs;
    [SerializeField] private Dictionary<SoundEnum, AudioClip> _allAudioClips;
    private List<AudioSource> _audioSourcesPool = new List<AudioSource>();
    [SerializeField] private int _numAudioSourcesInPool = 10;
    private List<AudioSource> _freeAudioSources = new List<AudioSource>();

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

    public void PlaySound(SoundEnum soundEnum)
    {
        AudioClip audioClip = _allAudioClips[soundEnum];
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
        for (int i = 0; i < _audioSourcesPool.Count; i++)
        {
            currentSource = new AudioSource();
            _audioSourcesPool.Add(currentSource);
            _freeAudioSources.Add(currentSource);
        }
    }

    private AudioSource GetAnAudioFromPool()
    {
        AudioSource audioSource;
        if (_audioSourcesPool.Count == 0)
        {
            audioSource = new AudioSource();
        }
        else
        {
            audioSource = _audioSourcesPool[0];
            _audioSourcesPool.Remove(audioSource);
        }
        return audioSource;
    }
}