using System;
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

    public void PlaySound(Sound sound)
    {
        var audioClip = sounds.First(x => x.Sound == sound).AudioClip;
        if(audioClip == null)
        {
            Debug.LogWarning("No hay audioclip para esta accion");
            return;
        }
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, 0.5f);
    }
}
