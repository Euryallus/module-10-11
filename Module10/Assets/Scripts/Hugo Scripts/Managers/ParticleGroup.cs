using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGroup: MonoBehaviour
{
    public List<ParticleSystem> systems = new List<ParticleSystem>();

    public bool HasStopped()
    {
        bool ended = true;

        foreach(ParticleSystem sys in systems)
        {
            ended = !sys.isPlaying;
        }

        return ended;
    }
}
