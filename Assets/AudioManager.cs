using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SoundInfos
{
    public string name;
    public Slider Volume;
    public Text VolumeText;
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public int[] muteinfo;
    public int musicId = 5;
    public Sound[] sounds;
    [SerializeField]
    public Sprite[] muted;
    public Button[] mute;
    public SoundInfos[] Volumes;
    private void Awake()
    {

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        
    }

    public void ChangeVolume()
    {
        Play("Answer");
        if (Volumes[0].Volume.value>0)//master volume ne ise geri kalnda o
        {
            mute[0].image.sprite = muted[0];
            mute[1].image.sprite = muted[0];
            mute[2].image.sprite = muted[0];
        }
        else if(Volumes[0].Volume.value == 0)
        {
            mute[0].image.sprite = muted[1];
            mute[1].image.sprite = muted[1];
            mute[2].image.sprite = muted[1];
        }
        if (Volumes[1].Volume.value >0)
        {
            mute[1].image.sprite = muted[0];
        }
        else
        {
            mute[1].image.sprite = muted[1];
        }
        if (Volumes[2].Volume.value >0)
        {
            mute[2].image.sprite = muted[0];
        }
        else
        {
            mute[2].image.sprite = muted[1];
        }

        AudioListener.volume = Volumes[0].Volume.value;
        sounds[musicId].source.volume = Volumes[1].Volume.value;
        for (int i = 0; i < musicId; i++)
        {
            
            sounds[i].source.volume = Volumes[2].Volume.value;
        }
        

        
            Volumes[0].VolumeText.text = ((int)(Volumes[0].Volume.value * 100)).ToString();
            Volumes[1].VolumeText.text = ((int)(Volumes[1].Volume.value * 100)).ToString();
            Volumes[2].VolumeText.text = ((int)(Volumes[2].Volume.value * 100)).ToString();


        Save();
    }

    public void ResetSound()
    {
        FindObjectOfType<AudioManager>().Play("ResetLevel");
    }
    public void Save()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetFloat(Volumes[i].Volume.name, Volumes[i].Volume.value);
        }
    }

    public void Load()
    {
        for (int i = 0; i < 3; i++)
        {
            Volumes[i].Volume.value = PlayerPrefs.GetFloat(Volumes[i].Volume.name, 0.5f);
        }

    }
    public void MuteMusic()
    {
        Play("Button");
        if (muteinfo[1] == 0)
        {
            Volumes[1].Volume.value = 1;
            mute[1].image.sprite = muted[0];

            muteinfo[1] = 1;
        }
        else
        {
            Volumes[1].Volume.value = 0;
            mute[1].image.sprite = muted[1];

            muteinfo[1] = 0;

        }
        PlayerPrefs.SetInt("MuteMusic", muteinfo[1]);
    }
    public void MuteSFX()
    {
        Play("Button");
        if (muteinfo[2] == 0)
        {
            Volumes[2].Volume.value = 1;
            mute[2].image.sprite = muted[0];

            muteinfo[2] = 1;
        }
        else
        {
            Volumes[2].Volume.value = 0;
            mute[2].image.sprite = muted[1];

            muteinfo[2] = 0;

        }
        PlayerPrefs.SetInt("MuteSFX", muteinfo[2]);
    }
    public void MuteAll()
    {
        Play("Button");
        if (muteinfo[0] == 0)
        {
            Volumes[0].Volume.value = 1;
            Volumes[1].Volume.value = 1;
            Volumes[2].Volume.value = 1;
            mute[0].image.sprite = muted[0];
            mute[1].image.sprite = muted[0];
            mute[2].image.sprite = muted[0];

            muteinfo[0] = 1;
        }
        else
        {
            Volumes[0].Volume.value = 0;
            Volumes[1].Volume.value = 0;
            Volumes[2].Volume.value = 0;

            mute[0].image.sprite = muted[1];
            mute[1].image.sprite = muted[1];
            mute[2].image.sprite = muted[1];

            muteinfo[0] = 0;

        }
        PlayerPrefs.SetInt("MuteAll", muteinfo[0]);
    }
    private void Start()
    {
        Load();
        Play("Music");
    }
    public void Play(string name)
    {
        Sound s=Array.Find(sounds, sound => sound.name == name);
        if (s==null)
        {
            return;
        }
        s.source.Play();
    }
}
