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
    private List<SoundEnum> _currentSoundsPlaying = new List<SoundEnum>();

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

        SetPool();
    }
    
    public void PlayInfiniteLoop(SoundEnum soundEnum)
    {
        if (_dicoInfiniteAudioSources.ContainsKey(soundEnum)) return;
        AudioClip audioClip = GetAudioCLip(soundEnum);
        AudioSource audioSource = GetAnAudioFromPool();
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.loop = true;
        _dicoInfiniteAudioSources[soundEnum] = audioSource;
        Debug.Log("Start sound loop " +  soundEnum);
    }

    public void StopInfiniteLoop(SoundEnum soundEnum)
    {
        if (!_dicoInfiniteAudioSources.ContainsKey(soundEnum)) return;
        AudioSource audioSource = _dicoInfiniteAudioSources[soundEnum];
        _dicoInfiniteAudioSources.Remove(soundEnum);
        PutAudioSourceInPool(audioSource);
        Debug.Log("Stop sound loop " +  soundEnum);
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
        _currentSoundsPlaying.Add(soundEnum);
        AudioSourceCoroutine(audioSource, soundEnum);
        Debug.Log("Play sound " +  soundEnum);
    }

    public void PlayRandomSound(List<SoundEnum> soundsEnum)
    {
        int index = Random.Range(0, soundsEnum.Count);
        SoundEnum soundChoosed = soundsEnum[index];
        PlaySound(soundChoosed);
    }

    private IEnumerable AudioSourceCoroutine(AudioSource audioSource, SoundEnum soundEnum)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        PutAudioSourceInPool(audioSource);
        _currentSoundsPlaying.Remove(soundEnum);
    }

    public bool IsSoundPlaying(SoundEnum soundEnum)
    {
        return _currentSoundsPlaying.Contains(soundEnum);
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