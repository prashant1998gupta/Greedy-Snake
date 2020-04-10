using SA;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<AudioManager>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Debug.Log(gameObject.name);
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play(SoundType.BACKGROUNDMUSIC);
    }

   

    /*public void Play(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i] == null)
            {
                return;
            }

            if (sounds[i].name == name)
            {
                sounds[i].source.Play();
            }
        }


    }*/

    public void Play(SoundType soundType)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i] == null)
            {
                return;
            }

            if (sounds[i].soundType== soundType)
            {
                sounds[i].source.Play();
            }
        }
    }

    }
public enum SoundType
{
    BACKGROUNDMUSIC , 
    SWIPESOUND,
    PLAYEREAT,
    GAMEOVER,
    BUTTONCLICK,
}