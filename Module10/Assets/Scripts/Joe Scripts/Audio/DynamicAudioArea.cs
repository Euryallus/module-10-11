using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAudioArea : MonoBehaviour
{
    public AudioSource MusicSource { get { return musicSource; } }

    [SerializeField] private MusicClass     musicToTrigger;
    [SerializeField] private AudioSource    musicSource;

    private float baseVolume;

    private bool fadingIn;
    private bool fadingOut;

    private void Awake()
    {
        baseVolume          = musicToTrigger.Volume;
        musicSource.clip    = musicToTrigger.AudioClip;

        musicSource.volume  = 0.0f;
    }

    private void Update()
    {
        if (musicSource.timeSamples >= (musicToTrigger.AudioClip.samples - 2048))
        {
            AudioManager.Instance.PlayAllDynamicSources();
        }

        if(fadingIn)
        {
            musicSource.volume += Time.unscaledDeltaTime * 0.5f;

            if(musicSource.volume >= baseVolume)
            {
                fadingIn = false;
                musicSource.volume = baseVolume;
            }
        }
        else if(fadingOut)
        {
            musicSource.volume -= Time.unscaledDeltaTime * 0.5f;

            if (musicSource.volume <= 0.0f)
            {
                fadingOut = false;
                musicSource.volume = 0.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioManager audioManager = AudioManager.Instance;

        if(other.gameObject.CompareTag("Player"))
        {
            if (audioManager.CurrentSceneMusic.PlayMode == MusicPlayMode.Dynamic)
            {
                if (musicToTrigger != null)
                {
                    if(audioManager.CurrentDynamicAudioArea != null)
                    {
                        audioManager.CurrentDynamicAudioArea.FadeOut();
                    }

                    audioManager.CurrentDynamicAudioArea = this;

                    FadeIn();
                }
                else
                {
                    Debug.LogError("Entering DynamicAudioError with no music set!");
                }
            }
        }
    }

    public void FadeIn()
    {
        fadingOut = false;
        fadingIn = true;
    }

    public void FadeOut()
    {
        fadingOut = true;
        fadingIn = false;
    }
}