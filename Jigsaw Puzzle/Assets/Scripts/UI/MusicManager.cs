using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    bool isSFXOn;
    bool isMusicOn;

    static MusicManager instance;
    public static MusicManager Instance
    {
        get
        {
            return instance;
        }
    }

    AudioSource mainMusic;
    AudioSource soundEffect;

    [SerializeField]
    AudioClip[] mainMusicArray;
    [SerializeField]
    AudioClip[] soundEffectArray;
    void Start()
    {
        mainMusic = GetComponents<AudioSource>()[0];
        soundEffect = GetComponents<AudioSource>()[1];
        if (PlayerPrefs.GetInt("isSFXOn") == 0)
        {
            soundEffect.volume = 0;
        }
        else
        {
            soundEffect.volume = 100;
        }

        if (PlayerPrefs.GetInt("isMusicOn") == 0)
        {
            mainMusic.volume = 0;
        }
        else
        {
            mainMusic.volume = 1;
        }
    }
    private void Awake()
    {
        OnInit();
    }
    void OnInit()
    {
        if (PlayerPrefs.HasKey("isSFXOn"))
        {
            if (PlayerPrefs.GetInt("isSFXOn") == 0)
            {
                isSFXOn = false;
            }
            else
            {
                isSFXOn = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("isSFXOn", 1);
            isSFXOn = true;
        }

        if (PlayerPrefs.HasKey("isMusicOn"))
        {
            if (PlayerPrefs.GetInt("isMusicOn") == 0)
            {
                isMusicOn = false;
            }
            else
            {
                isMusicOn = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("isMusicOn", 1);
            isMusicOn = true;
        }

        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public bool GetIsMuicOn()
    {
        return isMusicOn;
    }

    public bool GetIsSFXOn()
    {
        return isSFXOn;
    }

    public void SetMusic(bool _status)
    {
        isMusicOn = _status;
        if (!isMusicOn)
        {
            mainMusic.volume = 0;
            PlayerPrefs.SetInt("isMusicOn", 0);
        }
        else
        {
            mainMusic.volume = 0.5f;
            PlayerPrefs.SetInt("isMusicOn", 1);
        }
       
    }

    public void SetSFX(bool _status)
    {
        isSFXOn = _status;
        if (!isSFXOn)
        {
            soundEffect.volume = 0;
            PlayerPrefs.SetInt("isSFXOn", 0);
        }
        else
        {
            soundEffect.volume = 1;
            PlayerPrefs.SetInt("isSFXOn", 1);
        }
    }

    public void PlayNewMainMusic(MainMusic _music)
    {
        mainMusic.clip = mainMusicArray[(int)_music];
        mainMusic.Play();
    }

    public void PlayNewSoundEffect(SoundEffect _sound)
    {
        if (isSFXOn)
            soundEffect.PlayOneShot((soundEffectArray[(int)_sound]));
    }

    public void SetVolumeMainMusic(float _amount)
    {
        mainMusic.volume = _amount;
    }
}
