using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : Singleton<SoundSystem>
{
    [SerializeField] private List<SoundModelSO> soundModels = new List<SoundModelSO>();  
    private Dictionary<SoundModelSO.SoundName, AudioClip> soundDictionary = new Dictionary<SoundModelSO.SoundName, AudioClip>();
    private List<AudioSource> audioSources = new List<AudioSource>();
    private List<AudioSource> audioSourcesOnUse = new List<AudioSource>();
    private AudioSource enviromentAudiSource;

    private new void Awake()
    {
        base.Awake();
        LoadDictionary();
        for (int i = 0; i < 10; i++)
        {
            audioSources.Add(CreateNewAudioSource());
        }
        PlayEnviromentMusic();
    }

    private void FixedUpdate()
    {
        CheckAudioSources();
    }

    public void PlayEnviromentMusic()
    {
        enviromentAudiSource = CreateNewAudioSource();
        enviromentAudiSource.clip = soundDictionary[SoundModelSO.SoundName.BackgroundMusic];
        enviromentAudiSource.loop = true;
        enviromentAudiSource.Play();
        enviromentAudiSource.volume = 0.1f;
    }

    public void PlayClickSound()
    {
        PlaySound(SoundModelSO.SoundName.ButtonClick, Vector3.zero);
    }

    private void CheckAudioSources()
    {
        for (int i = audioSourcesOnUse.Count - 1; i >= 0; i--)
        {
            AudioSource source = audioSourcesOnUse[i];
            if (!source.isPlaying)
            {
                audioSourcesOnUse.RemoveAt(i);
                audioSources.Add(source);
            }
        }
    }
    private AudioSource CreateNewAudioSource()
    {
        GameObject go = new GameObject("AudioSource");
        go.transform.parent = this.transform;
        AudioSource source = new GameObject("AudioSource").AddComponent<AudioSource>();
        return source;
    }
    private void LoadDictionary()
    {
        foreach (var model in soundModels)
        {
            soundDictionary[model.Name] = model.Clip;
        }
    }

    public void PlaySound(SoundModelSO.SoundName sound, Vector3 playPosition, bool usePtich = false)
    {
        AudioSource audioSource = null;
        if (audioSources.Count > 0)
        {
            audioSource = audioSources[0];
            audioSources.RemoveAt(0);
        }
        else
        {
             audioSource = CreateNewAudioSource();
        }
        audioSourcesOnUse.Add(audioSource);
        audioSource.transform.position = playPosition;
        audioSource.clip = soundDictionary[sound];
        if (usePtich)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
        audioSource.Play();
    }

}
