using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCampManager : MonoBehaviour
{
    public float spawnDistanceMax = 10f;

    [SerializeField]
    public List<EnemyBase> unitsDifficulty = new List<EnemyBase>();

    public List<EnemyBase> possibleUnits = new List<EnemyBase>();

    public int difficultyLevel = 10;
    public List<EnemyBase> spawnedEnemies = new List<EnemyBase>();

    public Vector3 spawnPoint;

    void Start()
    {
        SpawnUnits(difficultyLevel);
    }

    public void SpawnUnits(int difficultyLevel)
    {
        unitsDifficulty = new List<EnemyBase>();

        int i = Random.Range(0, possibleUnits.Count);
        int startPlace = possibleUnits[i].difficulty;

        unitsDifficulty.Add(possibleUnits[i]);
        RemainingUnits(startPlace);

        foreach(EnemyBase prefab in unitsDifficulty)
        {
            spawnedEnemies.Add(Instantiate(prefab));
            EnemyBase created = spawnedEnemies[spawnedEnemies.Count - 1];
            created.centralHubPos = transform.position;


            Vector3 randomPosition = Random.insideUnitSphere * spawnDistanceMax;

            randomPosition += transform.position;
            randomPosition.y = transform.position.y;

            created.transform.position = randomPosition;
        }
    }

    private void RemainingUnits(int total)
    {
        if(total >= difficultyLevel)
        {
            return;
        }

        int n = 1;
        int i = 0;

        if(difficultyLevel - total > 1)
        {
            i = Random.Range(0, possibleUnits.Count);
            n = possibleUnits[ i ].difficulty;
        }

        unitsDifficulty.Add(possibleUnits[i]);
        RemainingUnits(total + n);
    }

    private void RandomStartPoint()
    {
        Vector3 randomPosition = Random.insideUnitSphere * spawnDistanceMax;

        randomPosition += transform.position;
        randomPosition.y = transform.position.y;

        spawnPoint = randomPosition;

    }

    
}
