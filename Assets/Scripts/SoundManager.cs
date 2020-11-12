using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum Sound {
    Footman_As_You_Wish_Sound_Effect,
    Footman_Yes_My_Lord_Sound_Effect,
    Peasant_More_Work_Sound_Effect,
    Peasant_Yes_My_Lord_Sound_Effect
}

[Serializable]
public class SoundEffect {
    [SerializeField]
    public Sound Sound;
    [SerializeField]
    public AudioClip AudioClip;
}

public class SoundManager : Singleton<SoundManager>
{
    public SoundEffect[] sounds;
    private int audioSourcesCount = 3;
    private List<AudioSource> audioSources;
    private Vector3 cameraPosition;

    private void Awake()
    {
        cameraPosition = Camera.main.transform.position;
        audioSources = new List<AudioSource>();
        
        for (int i = 0; i < audioSourcesCount; i++)
        {
            GameObject go = new GameObject($"AudioSource{i}");
            go.transform.position = cameraPosition;
            audioSources.Add(go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(Sound sound)
    {
        var audioClip = sounds.First(x => x.Sound == sound).AudioClip;
        
        if(audioClip == null)
        {
            Debug.LogWarning("No hay audioclip para esta accion");
            return;
        }
        
        var selectedAudioSource = audioSources.FirstOrDefault(x => !x.isPlaying);

        if (selectedAudioSource == null)
        {
            selectedAudioSource = audioSources.First();
            foreach (var audioSource in audioSources)
            {
                selectedAudioSource = selectedAudioSource.time > audioSource.time ? audioSource : selectedAudioSource;
            }
        }

        selectedAudioSource.clip = audioClip;
        selectedAudioSource.Play();
    }
}
