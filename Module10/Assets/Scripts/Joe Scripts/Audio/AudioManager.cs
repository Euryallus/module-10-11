using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MusicPlayMode
{
    OrderedPlaylist,
    RandomPlaylist,
    LoopSingleTrack,
    Dynamic
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private SoundClass[]   sounds;
    [SerializeField] private SceneMusic[]   sceneMusicSetup;

    [SerializeField] private AudioSource    musicSource;
    [SerializeField] private GameObject     soundSourcePrefab;
    [SerializeField] private GameObject     dynamicMusicSourcePrefab;

    public SceneMusic CurrentSceneMusic { get { return currentSceneMusic; } }
    public DynamicAudioArea CurrentDynamicAudioArea { get { return currentDynamicAudioArea; } set { currentDynamicAudioArea = value; } }

    private Dictionary<string, SoundClass> soundsDict;

    private SceneMusic currentSceneMusic;

    private int currentPlaylistIndex;

    private DynamicAudioArea[]  dynamicAudioAreas;
    private DynamicAudioArea    currentDynamicAudioArea;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if(!musicSource.isPlaying)
        {
            if (currentSceneMusic.PlayMode == MusicPlayMode.OrderedPlaylist || currentSceneMusic.PlayMode == MusicPlayMode.RandomPlaylist)
            {
                PlayNextMusicFromPlaylist();
            }
        }
    }

    public void PlayAllDynamicSources()
    {
        for (int i = 0; i < dynamicAudioAreas.Length; i++)
        {
            dynamicAudioAreas[i].MusicSource.Play();
        }
    }

    private void PlayNextMusicFromPlaylist()
    {
        switch (currentSceneMusic.PlayMode)
        {
            case MusicPlayMode.OrderedPlaylist:
            {
                if (currentPlaylistIndex < (currentSceneMusic.Playlist.Length - 1))
                {
                    currentPlaylistIndex++;
                }
                else
                {
                    currentPlaylistIndex = 0;
                }

                PlayMusic(currentSceneMusic.Playlist[currentPlaylistIndex], false);
            }
            break;

            case MusicPlayMode.RandomPlaylist:
            {
                int previousIndex = currentPlaylistIndex;

                if (currentSceneMusic.Playlist.Length > 1)
                {
                    while (currentPlaylistIndex == previousIndex)
                    {
                        currentPlaylistIndex = Random.Range(0, currentSceneMusic.Playlist.Length);
                    }
                }
                else
                {
                    currentPlaylistIndex = 0;
                }

                PlayMusic(currentSceneMusic.Playlist[currentPlaylistIndex], false);
            }
            break;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        for (int i = 0; i < sceneMusicSetup.Length; i++)
        {
            if(sceneMusicSetup[i].SceneName == scene.name)
            {
                SceneMusic sceneMusic = sceneMusicSetup[i];

                currentSceneMusic = sceneMusic;

                if(sceneMusic.PlayMode == MusicPlayMode.OrderedPlaylist || sceneMusic.PlayMode == MusicPlayMode.RandomPlaylist)
                {
                    currentPlaylistIndex = -1;
                    PlayNextMusicFromPlaylist();
                }
                else if(sceneMusic.PlayMode == MusicPlayMode.LoopSingleTrack)
                {
                    currentPlaylistIndex = 0;
                    PlayMusic(sceneMusic.Playlist[0], true);
                }
                else if(sceneMusic.PlayMode == MusicPlayMode.Dynamic)
                {
                    GameObject[] dynamicAudioGameObjs = GameObject.FindGameObjectsWithTag("DynamicAudioArea");

                    dynamicAudioAreas = new DynamicAudioArea[dynamicAudioGameObjs.Length];

                    for (int j = 0; j < dynamicAudioGameObjs.Length; j++)
                    {
                        dynamicAudioAreas[j] = dynamicAudioGameObjs[j].GetComponent<DynamicAudioArea>();
                        dynamicAudioAreas[j].MusicSource.Play();
                    }
                }
            }
        }
    }

    private void SetupDictionaries()
    {
        soundsDict  = new Dictionary<string, SoundClass>();

        for (int i = 0; i < sounds.Length; i++)
        {
            soundsDict.Add(sounds[i].Id, sounds[i]);
        }
    }

    public void PlayMusic(MusicClass music, bool loop)
    {
        musicSource.clip    = music.AudioClip;
        musicSource.volume  = music.Volume;
        musicSource.loop    = loop;

        musicSource.Play();
    }

    //public void PlayMusic(string id, bool loop)
    //{
    //    if (musicDict.ContainsKey(id))
    //    {
    //        PlayMusic(musicDict[id], loop);
    //    }
    //    else
    //    {
    //        Debug.LogError("Trying to play music with invalid id: " + id);
    //    }
    //}

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

[System.Serializable]
public struct SceneMusic
{
    public string           SceneName;
    public MusicPlayMode    PlayMode;
    public MusicClass[]     Playlist;
}
