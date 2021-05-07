using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//MusicPlayMode defines all of the different ways music can be played for a certain scene
public enum MusicPlayMode
{
    OrderedPlaylist,    // A playlist where songs are played in the listed order

    RandomPlaylist,     // A playlist where songs are played in a random order
                        //   (if there are multiple songs in the playlist, each song never being played more than once in a row)

    LoopSingleTrack,    // One track is looped for the duration of the scene

    Dynamic,            // The music played in the scene is determined by the placement of DynamicAudioAreas

    None                // No music will be played in the scene
}

// ||=======================================================================||
// || AudioManager: Handles playing music and sound effects                 ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    #region InspectorVariables
    // Variables in this region are set in the inspector

    [SerializeField] private SoundClass[]   sounds;             // All sound effects that can be played
    [SerializeField] private SceneMusic[]   sceneMusicSetup;    // Array of data defining how music should be handled in each game scene

    [SerializeField] private AudioSource    musicSource;        // The audio source used to play non-dynamic music
    [SerializeField] private GameObject     soundSourcePrefab;  // The prefab instantiated when a sound is played

    #endregion

    #region Properties

    public SceneMusic       CurrentSceneMusic       { get { return currentSceneMusic; } }       
    public DynamicAudioArea CurrentDynamicAudioArea { get { return currentDynamicAudioArea; }
                                                      set { currentDynamicAudioArea = value; } }

    #endregion

    private Dictionary<string, SoundClass>  soundsDict;                 // Dictionary containing sound effects indexed by their string id's
    private SceneMusic                      currentSceneMusic;          // Defines how music is handled in the loaded scene
    private int                             currentPlaylistIndex;       // If using a playlist, the index of the song that is currently playing in it
    private DynamicAudioArea[]              dynamicAudioAreas;          // All (if any) dynamic audio areas in the loaded scene
    private DynamicAudioArea                currentDynamicAudioArea;    // The dynamic audio area that the player most recently entered

    private void Awake()
    {
        // Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            // Set this class as the instance and ensure that it stays when changing scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Add all sounds to the sound dictionary
        SetupSoundDictionary();

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void SetupSoundDictionary()
    {
        // Add all sounds to the dictionary, indexed by their id's
        //   so they can easily be returned when needed
        soundsDict = new Dictionary<string, SoundClass>();

        for (int i = 0; i < sounds.Length; i++)
        {
            soundsDict.Add(sounds[i].Id, sounds[i]);
        }
    }

    private void Update()
    {
        if(!musicSource.isPlaying)
        {
            if (currentSceneMusic.PlayMode == MusicPlayMode.OrderedPlaylist || currentSceneMusic.PlayMode == MusicPlayMode.RandomPlaylist)
            {
                // If the current music track has finished playing and a playlist is being used, move to the next song (either sequentially or randomly)
                PlayNextMusicFromPlaylist();
            }
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        // Ensure the current dynamic audio area is null by default, in case one was set in a previous scene
        currentDynamicAudioArea = null;

        // Set playmode to none by default in case none was set in the inspector
        currentSceneMusic = new SceneMusic(scene.name, MusicPlayMode.None, null);

        for (int i = 0; i < sceneMusicSetup.Length; i++)
        {
            // Find the scene music setup for the loaded scene
            if(sceneMusicSetup[i].SceneName == scene.name)
            {
                SceneMusic sceneMusic = sceneMusicSetup[i];
                currentSceneMusic = sceneMusic;

                if(sceneMusic.PlayMode == MusicPlayMode.OrderedPlaylist || sceneMusic.PlayMode == MusicPlayMode.RandomPlaylist)
                {
                    // Play a track from the playlist, random or otherwise
                    currentPlaylistIndex = -1;  // Starts at -1 since PlayNextMusicFromPlaylist will begin by incrementing the index to 0
                    PlayNextMusicFromPlaylist();
                }
                else if(sceneMusic.PlayMode == MusicPlayMode.LoopSingleTrack)
                {
                    // Loop the first (and ideally only) track in the playlist for the duration of the scene
                    currentPlaylistIndex = 0;
                    PlayMusic(sceneMusic.Playlist[0], true);
                }
                else if(sceneMusic.PlayMode == MusicPlayMode.Dynamic)
                {
                    // Find all dynamic audio areas in the scene
                    GameObject[] dynamicAudioGameObjs = GameObject.FindGameObjectsWithTag("DynamicAudioArea");

                    // Setup the dynamic audio areas array based on the number of areas in the scene
                    dynamicAudioAreas = new DynamicAudioArea[dynamicAudioGameObjs.Length];

                    for (int j = 0; j < dynamicAudioGameObjs.Length; j++)
                    {
                        // Add all dynamic audio areas to the array
                        dynamicAudioAreas[j] = dynamicAudioGameObjs[j].GetComponent<DynamicAudioArea>();
                    }

                    // Play all dynamic audio sources so they are in sync
                    PlayAllDynamicSources();
                }

                break;
            }
        }
    }

    public void PlayMusic(MusicClass music, bool loop)
    {
        // Play the given music track, adjusting volume/loop to match parameters

        musicSource.clip    = music.AudioClip;
        musicSource.volume  = music.Volume;
        musicSource.loop    = loop;

        musicSource.Play();
    }

    private void PlayNextMusicFromPlaylist()
    {
        // Set the currentPlaylistIndex so that the next song in the playlist is played,
        //   either sequentially for an OrderedPlaylist or randomly for a RandomPlaylist

        switch (currentSceneMusic.PlayMode)
        {
            case MusicPlayMode.OrderedPlaylist:
            {
                if (currentPlaylistIndex < (currentSceneMusic.Playlist.Length - 1))
                {
                    // Go to the next track in the playlist
                    currentPlaylistIndex++;
                }
                else
                {
                    // Final track was played, go back to the first track
                    currentPlaylistIndex = 0;
                }
            }
            break;

            case MusicPlayMode.RandomPlaylist:
            {
                int previousIndex = currentPlaylistIndex;

                if (currentSceneMusic.Playlist.Length > 1)
                {
                    // Pick a random index that is different to the last one so a new track is played
                    while (currentPlaylistIndex == previousIndex)
                    {
                        currentPlaylistIndex = Random.Range(0, currentSceneMusic.Playlist.Length);
                    }
                }
                else
                {
                    // There is only one track to play, no randomisation needed
                    currentPlaylistIndex = 0;
                }
            }
            break;
        }

        // Play a track from the playlist based on the index set above
        PlayMusic(currentSceneMusic.Playlist[currentPlaylistIndex], false);
    }

    private void PlaySoundEffect(SoundClass sound, bool use3DSpace, Vector3 sourcePosition = default)
    {
        // Pick a random volume/sound within the set ranges
        float volume    = Random.Range(sound.VolumeRange.Min, sound.VolumeRange.Max);
        float pitch     = Random.Range(sound.PitchRange.Min, sound.PitchRange.Max);

        // Create the sound source GameObject
        GameObject goSource = Instantiate(soundSourcePrefab, sourcePosition, Quaternion.identity, transform);
        goSource.name = "Sound_" + sound.Id;

        // Set AudioSource values based on given parameters
        AudioSource audioSource = goSource.GetComponent<AudioSource>();

        audioSource.clip        = sound.AudioClips[Random.Range(0, sound.AudioClips.Length)];

        audioSource.volume      = volume; //  * saved sound value

        audioSource.pitch       = pitch;

        if (use3DSpace)
        {
            // Enable spatialBlend if playing sound in 3D space, so it will sound like it originates from sourcePosition
            audioSource.spatialBlend = 1.0f;
        }

        // if (looping)
        // {
        //     // If looping, set the audioSource to loop and give it an identifiable name so it can later be stopped/deleted
        //     goSource.name = "LoopSound_" + loopId;
        //     audioSource.loop = true;
        // }

        // Play the sound
        audioSource.Play();
    }

    private void PlaySoundEffect(string id, bool use3DSpace, Vector3 sourcePosition = default)
    {
        if (soundsDict.ContainsKey(id))
        {
            // Play a sound with the given parameters
            PlaySoundEffect(soundsDict[id], use3DSpace, sourcePosition);
        }
        else
        {
            Debug.LogError("Trying to play sound effect with invalid id: " + id);
        }
    }

    public void PlaySoundEffect2D(string id)
    {
        // Plays a sound with the given id that is not positioned in 3D space
        PlaySoundEffect(id, false);
    }

    public void PlaySoundEffect2D(SoundClass sound)
    {
        // Plays the given sound, not positioned in 3D space
        PlaySoundEffect(sound, false);
    }

    public void PlaySoundEffect3D(string id, Vector3 sourcePosition)
    {
        // Plays a sound with the given id positioned in 3D space at sourcePosition
        PlaySoundEffect(id, true, sourcePosition);
    }

    public void PlaySoundEffect3D(SoundClass sound, Vector3 sourcePosition)
    {
        // Plays the given sound, positioned in 3D space at sourcePosition
        PlaySoundEffect(sound, true, sourcePosition);
    }

    public void PlayAllDynamicSources()
    {
        // Plays all dynamic sources at the same time. Dynamic sources should
        //   always be played at the same time so they remain synchronised
        for (int i = 0; i < dynamicAudioAreas.Length; i++)
        {
            dynamicAudioAreas[i].MusicSource.Play();
        }
    }
}

// ||=======================================================================||
// || SceneMusic: Contains data about how/which music will be played for    ||
// ||   a certain scene.                                                    ||
// ||=======================================================================||
// || Written by Joseph Allen                                               ||
// || for the prototype phase.                                              ||
// ||=======================================================================||

[System.Serializable]
public struct SceneMusic
{
    public SceneMusic(string sceneName, MusicPlayMode playMode, MusicClass[] playlist)
    {
        SceneName = sceneName;
        PlayMode  = playMode;
        Playlist  = playlist;
    }

    public string           SceneName;  // Name of the scene to use these settings for
    public MusicPlayMode    PlayMode;   // The play mode to use in the scene
    public MusicClass[]     Playlist;   // The playlist used if the chosen PlayMode is OrderedPlaylist or RandomPlaylist
}