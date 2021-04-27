using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSourceObject : MonoBehaviour
{
    #region InspectorVariables
    //Variables in this region are set in the inspector

    [SerializeField] private AudioSource audioSource;

    #endregion

    void Update()
    {
        //Destroy this audio source gameobject once the sound is done playing
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}