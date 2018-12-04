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
       

        //if (audioPlayer == null) audioPlayer = this.gameObject.AddComponent<AudioSource>();
        CreateChannels(numberOfChannels);
    }

    public AudioClip[] soundEffects;
    public int numberOfChannels = 4;

    AudioSource[] audioChannels;
    int currentChannel;

    // For song
    [SerializeField] private AudioSource musicPlayer; 


    public void CreateChannels(int channelCount)
    {
        audioChannels = new AudioSource[channelCount];

        for (int i = 0; i < audioChannels.Length; i++)
        {
            audioChannels[i] = this.gameObject.AddComponent<AudioSource>();
           
        }
    }

    public void SetSoundEffectArraySize(int size, bool inEditor = false)
    {
        if (inEditor)
        {
            soundEffects = new AudioClip[size];
            Debug.Log("Sound Effects Array Defined");
        }
    }

// Use this for initialization
void Start()
    {
        //soundEffects = new AudioClip[(int)AudioEffects.NumberOfSounds];

        //soundEffects = Resources.LoadAll<AudioClip>("Sounds");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadSoundLibrary()
    {

    }

    public void PlaySoundEffect(SoundEffectEnum effectToPlay)
    {
        audioChannels[currentChannel].pitch = 1;
        audioChannels[currentChannel].clip = soundEffects[(int)effectToPlay];
        audioChannels[currentChannel].Play();
        NextChannel();
    }

    public void PlaySoundEffect(SoundEffectEnum effectToPlay, float pitch)
    {
        audioChannels[currentChannel].pitch = pitch;
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
