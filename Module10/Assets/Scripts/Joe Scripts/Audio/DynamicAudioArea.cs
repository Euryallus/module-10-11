using UnityEngine;

public class DynamicAudioArea : MonoBehaviour
{

    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private MusicClass     musicToTrigger; //The music to be triggered when the player enters this area
    [SerializeField] private AudioSource    musicSource;    //The audio source used to play the triggered music

    #endregion

    #region Properties

    public AudioSource MusicSource { get { return musicSource; } }

    #endregion

    private float baseVolume;   //Volume of the music before fading is applied
    private bool fadingIn;      //Whether the audio source should be fading in
    private bool fadingOut;     //Whether the audio source should be fading out

    private void Awake()
    {
        //Set defaults
        baseVolume          = musicToTrigger.Volume;
        musicSource.clip    = musicToTrigger.AudioClip;

        //Mute the source by default until the player enters it
        musicSource.volume  = 0.0f;
    }

    private void Update()
    {
        if (musicSource.timeSamples >= (musicToTrigger.AudioClip.samples - 2048))
        {
            //If the current audio clip will be done playing roughly in the next frame,
            //  restart all dynamic audio sources so the music loops seamlessly
            AudioManager.Instance.PlayAllDynamicSources();
        }

        if(fadingIn)
        {
            //Slowly increase the audio source volume each frame to fade music in
            musicSource.volume += Time.unscaledDeltaTime * 0.5f;

            //Once music is fully faded in, stop fading
            if(musicSource.volume >= baseVolume)
            {
                fadingIn = false;
                musicSource.volume = baseVolume;
            }
        }
        else if(fadingOut)
        {
            //Slowly decrease the audio source volume each frame to fade music out
            musicSource.volume -= Time.unscaledDeltaTime * 0.5f;

            //Once music is fully faded out, stop fading
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
            //Player has entered the audio area trigger

            if (audioManager.CurrentSceneMusic.PlayMode == MusicPlayMode.Dynamic)
            {
                //The loaded scene is using dynamic audio

                if (musicToTrigger != null)
                {
                    if(audioManager.CurrentDynamicAudioArea != null)
                    {
                        //If the player previously entered another area, fade its music out for a cross-fade effect
                        audioManager.CurrentDynamicAudioArea.FadeOut();
                    }

                    //This is now the current/most recent dynamic audio area
                    audioManager.CurrentDynamicAudioArea = this;

                    //Fade music for this are in
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
        //Start fading in, set fading out to false in case the source is in the process of fading out
        fadingOut = false;
        fadingIn = true;
    }

    public void FadeOut()
    {
        //Start fading out, set fading in to false in case the source is in the process of fading in
        fadingOut = true;
        fadingIn = false;
    }
}