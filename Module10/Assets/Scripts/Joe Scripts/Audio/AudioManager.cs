using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private SoundClass[]   sounds;
    [SerializeField] private MusicClass[]   music;

    [SerializeField] private AudioSource    musicSource;
    [SerializeField] private GameObject     soundSourcePrefab;

    private Dictionary<string, SoundClass> soundsDict;
    private Dictionary<string, MusicClass> musicDict;

    private void Awake()
    {
        //Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SetupDictionaries();

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayMusic("calm", true);
    }

    private void SetupDictionaries()
    {
        soundsDict  = new Dictionary<string, SoundClass>();
        musicDict   = new Dictionary<string, MusicClass>();

        for (int i = 0; i < sounds.Length; i++)
        {
            soundsDict.Add(sounds[i].Id, sounds[i]);
        }

        for (int i = 0; i < music.Length; i++)
        {
            musicDict.Add(music[i].Id, music[i]);
        }
    }

    public void PlayMusic(string id, bool loop)
    {
        if (musicDict.ContainsKey(id))
        {
            musicSource.clip    = musicDict[id].AudioClip;
            musicSource.volume  = musicDict[id].Volume;
            musicSource.loop    = loop;

            musicSource.Play();
        }
        else
        {
            Debug.LogError("Trying to play music with invalid id: " + id);
        }
    }

    private void PlaySoundEffect(SoundClass sound, bool use3DSpace, Vector3 sourcePosition = default)
    {
        float volume = Random.Range(sound.VolumeRange.Min, sound.VolumeRange.Max);
        float pitch = Random.Range(sound.PitchRange.Min, sound.PitchRange.Max);

        //Create the sound source GameObject
        GameObject goSource = Instantiate(soundSourcePrefab, sourcePosition, Quaternion.identity, transform);
        goSource.name = "Sound_" + sound.Id;

        //Set audioSource values based on given parameters
        AudioSource audioSource = goSource.GetComponent<AudioSource>();

        audioSource.clip = sound.AudioClips[Random.Range(0, sound.AudioClips.Length)];

        audioSource.volume = volume; // * saved sound value

        audioSource.pitch = pitch;

        if (use3DSpace)
        {
            //Enable spatialBlend if playing sound in 3D space, so it will sound like it originates from sourcePosition
            audioSource.spatialBlend = 1f;
        }

        //if (looping)
        //{
        //    //If looping, set the audioSource to loop and give it an identifiable name so it can later be stopped/deleted
        //    goSource.name = "LoopSound_" + loopId;
        //    audioSource.loop = true;
        //}

        //Play the sound
        audioSource.Play();
    }

    private void PlaySoundEffect(string id, bool use3DSpace, Vector3 sourcePosition = default)
    {
        if (soundsDict.ContainsKey(id))
        {
            PlaySoundEffect(soundsDict[id], use3DSpace, sourcePosition);
        }
        else
        {
            Debug.LogError("Trying to play sound effect with invalid id: " + id);
        }
    }

    public void PlaySoundEffect2D(string id)
    {
        PlaySoundEffect(id, false);
    }

    public void PlaySoundEffect2D(SoundClass sound)
    {
        PlaySoundEffect(sound, false);
    }

    public void PlaySoundEffect3D(string id, Vector3 sourcePosition)
    {
        PlaySoundEffect(id, true, sourcePosition);
    }

    public void PlaySoundEffect3D(SoundClass sound, Vector3 sourcePosition)
    {
        PlaySoundEffect(sound, true, sourcePosition);
    }
}
