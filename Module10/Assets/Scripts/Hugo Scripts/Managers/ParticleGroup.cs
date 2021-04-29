using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGroup: MonoBehaviour
{
    //lists systems in group
    public List<ParticleSystem> systems = new List<ParticleSystem>();

    //returns status of effect
    public bool HasStopped()
    {
        bool ended = true;

        //if all particle systems in group have stopped, returns true
        foreach(ParticleSystem sys in systems)
        {
            ended = !sys.isPlaying;
        }

        return ended;
    }

    public void PlayEffect()
    {
        foreach (ParticleSystem sys in systems)
        {
            sys.Play();
        }
    }
}
