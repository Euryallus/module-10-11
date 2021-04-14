using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSourceObject : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    void Update()
    {
        //Destroy this audio source gameobject once the sound is done playing
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}