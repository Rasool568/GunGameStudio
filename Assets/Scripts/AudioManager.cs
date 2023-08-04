using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //Синглтон
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }

    //Воспроизводит звук на камере игрока
    public static void PlaySoundOnPlayer(AudioClip _clip)
    {
        Camera.main.GetComponent<AudioSource>().volume = 1f;
        Camera.main.GetComponent<AudioSource>().PlayOneShot(_clip);
    }
    //Также воспроизводит звук, но с регулировкой громкости
    public static void PlaySoundOnPlayer(AudioClip _clip, float _volume)
    {
        Camera.main.GetComponent<AudioSource>().volume = _volume;
        Camera.main.GetComponent<AudioSource>().PlayOneShot(_clip);
    }

    public static bool AudioMute()
    {
        AudioSource camAudio = Camera.main.GetComponent<AudioSource>();
        camAudio.mute = !camAudio.mute;
        return camAudio.mute;
    }
}
