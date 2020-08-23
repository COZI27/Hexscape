using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


       if (musicPlayer == null) musicPlayer = gameObject.GetComponent<AudioSource>();
       
        CreateEffectChannels(numberOfChannels);
    }


    AudioSource[] audioChannels;
    int currentChannel;

    [Header("Audio Settings")]
    [SerializeField] private float soundEffectVolume = 1.0f;
    //[SerializeField] private float musicVolume = 1.0f;

    [SerializeField] private int numberOfChannels = 4;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicPlayer;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] soundEffects;




    public void CreateEffectChannels(int channelCount)
    {
        audioChannels = new AudioSource[channelCount];

        for (int i = 0; i < audioChannels.Length; i++)
        {
            audioChannels[i] = this.gameObject.AddComponent<AudioSource>();
           
        }
    }

    public void PopulateSoundEffectsArray(AudioClip[] clipsToLoad, bool inEditor = false)
    {
        if (inEditor)
        {
            soundEffects = clipsToLoad;
            Debug.Log("Sound Effects Array Defined");
        }
        else Debug.LogWarning("Failed to Populate Sound Effects Array. Can only be executed in editor.");
    }

    public void PlaySoundEffect(SoundEffectEnum effectToPlay)
    {
        audioChannels[currentChannel].pitch = 1;
        audioChannels[currentChannel].volume = soundEffectVolume;
        audioChannels[currentChannel].clip = soundEffects[(int)effectToPlay];
        audioChannels[currentChannel].Play();
        NextChannel();
    }

    public void PlaySoundEffect(SoundEffectEnum effectToPlay, float pitch)
    {
        audioChannels[currentChannel].pitch = pitch;
        audioChannels[currentChannel].volume = soundEffectVolume;
        audioChannels[currentChannel].clip = soundEffects[(int)effectToPlay];
        audioChannels[currentChannel].Play();
        NextChannel();
    }

    void NextChannel()
    {
        currentChannel++;
        if (currentChannel >= audioChannels.Length) currentChannel = 0;
    }
}
