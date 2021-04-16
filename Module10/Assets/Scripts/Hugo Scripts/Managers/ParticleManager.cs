using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [SerializeField]
    private List<ParticleIndex> particleObjects = new List<ParticleIndex>();

    private List<GameObject> initialisedEffects = new List<GameObject>();

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
    }


    // Update is called once per frame
    void Update()
    {
        foreach(GameObject obj in initialisedEffects)
        {
            if(obj.GetComponent<ParticleGroup>().HasStopped())
            {
                initialisedEffects.Remove(obj);
                Destroy(obj);
            }
        }
    }

    public void SpawnParticle(Vector3 position, string name)
    {
        foreach(ParticleIndex sys in particleObjects)
        {
            if(sys.sysName == name)
            {
                initialisedEffects.Add(Instantiate(sys.effect));
                initialisedEffects[initialisedEffects.Count - 1].transform.position = position;
            }
        }
    }
}
