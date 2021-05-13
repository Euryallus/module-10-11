using UnityEngine;

// ||=======================================================================||
// || DynamicAudioArea: Triggers a dynamic audio track when the player      ||
// ||   enters its area if the active scene is using MusicPlayMode.Dynamic  || 
// ||=======================================================================||
// || Used on prefab: Joe/Audio/DynamicAudioArea                            ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

public class DynamicAudioArea : MonoBehaviour, IPersistentObject
{

    #region InspectorVariables
    // Variables in this region are set in the inspector

    [SerializeField] private MusicClass     musicToTrigger; // The music to be triggered when the player enters this area
    [SerializeField] private AudioSource    musicSource;    // The audio source used to play the triggered music

    #endregion

    #region Properties

    public AudioSource MusicSource { get { return musicSource; } }

    #endregion

    private float   baseVolume;     // Volume of the music before fading is applied
    private bool    fadingIn;       // Whether the audio source should be fading in
    private bool    fadingOut;      // Whether the audio source should be fading out
    private bool    active;

    private void Awake()
    {
        // Set defaults
        musicSource.clip = musicToTrigger.AudioClip;

        // Mute the source by default until the player enters it
        musicSource.volume = 0.0f;

        // Allow volume to be adjusted while the game is paused
        musicSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
    }

    private void Start()
    {
        SaveLoadManager.Instance.SubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.UnsubscribeSaveLoadEvents(OnSave, OnLoadSetup, OnLoadConfigure);
    }

    public void OnSave(SaveData saveData)
    {
        string locationId = GetLocationId();

        Debug.Log("Saving data for DynamicAudioArea with location id: " + locationId);

        saveData.AddData("audioAreaActive_" + locationId, active);
    }

    public void OnLoadSetup(SaveData saveData)
    {
        bool loadedActive = saveData.GetData<bool>("audioAreaActive_" + GetLocationId());

        if(loadedActive)
        {
            ActivateAudioArea();
        }
    }

    public void OnLoadConfigure(SaveData saveData)
    {

    }

    private string GetLocationId()
    {
        return (int)transform.position.x + "_" + (int)transform.position.y + "_" + (int)transform.position.z;
    }

    private void Update()
    {
        if (musicSource.timeSamples >= (musicToTrigger.AudioClip.samples - 2048))
        {
            // If the current audio clip will be done playing roughly in the next frame,
            //   restart all dynamic audio sources so the music loops seamlessly
            AudioManager.Instance.PlayAllDynamicSources();
        }

        if(fadingIn)
        {
            // Slowly increase the audio source volume each frame to fade music in
            musicSource.volume += Time.unscaledDeltaTime * 0.5f * baseVolume;

            // Once music is fully faded in, stop fading
            if(musicSource.volume >= baseVolume)
            {
                fadingIn = false;
                musicSource.volume = baseVolume;
            }
        }
        else if(fadingOut)
        {
            // Slowly decrease the audio source volume each frame to fade music out
            musicSource.volume -= Time.unscaledDeltaTime * 0.5f * baseVolume;

            // Once music is fully faded out, stop fading
            if (musicSource.volume <= 0.0f)
            {
                fadingOut = false;
                musicSource.volume = 0.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ActivateAudioArea();
        }
    }

    public void ActivateAudioArea()
    {
        AudioManager audioManager = AudioManager.Instance;

        // Player has entered the audio area trigger

        if (audioManager.CurrentSceneMusic.PlayMode == MusicPlayMode.Dynamic)
        {
            // The loaded scene is using dynamic audio

            if (musicToTrigger != null)
            {
                if (audioManager.CurrentDynamicAudioArea != null)
                {
                    // If the player previously entered another area, fade its music out for a cross-fade effect
                    audioManager.CurrentDynamicAudioArea.DeactivateAudioArea();
                }

                // This is now the current/most recent dynamic audio area
                audioManager.CurrentDynamicAudioArea = this;

                // Fade music for this are in
                FadeIn();

                active = true;
            }
            else
            {
                Debug.LogError("Entering DynamicAudioError with no music set!");
            }
        }
    }

    public void DeactivateAudioArea()
    {
        FadeOut();

        active = false;
    }

    public void UpdateSourceVolume(float volume)
    {
        Debug.Log("Updating source volume: " + musicToTrigger.name);

        baseVolume = musicToTrigger.Volume * volume;

        if(active)
        {
            musicSource.volume = baseVolume;
        }
    }

    private void FadeIn()
    {
        // Start fading in, set fading out to false in case the source is in the process of fading out
        fadingOut = false;
        fadingIn = true;
    }

    private void FadeOut()
    {
        // Start fading out, set fading in to false in case the source is in the process of fading in
        fadingOut = true;
        fadingIn = false;
    }
}