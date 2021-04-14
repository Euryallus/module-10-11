using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Effect", menuName = "Audio/Sound Effect")]
public class SoundEffect : ScriptableObject
{
    [SerializeField] [Tooltip("All possible audio clips that will randomly be chosen from when this sound effect is played")]
    private AudioClip[] audioClips;

    [SerializeField] [Tooltip("Unique id for this sound effect that will be used to play it in code")]
    private string id;

    private float volumeRange;

    private Vector2 pitchRange;
}
