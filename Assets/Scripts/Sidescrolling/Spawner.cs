using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    [SerializeField]
    private Transform[] enemySpawnPoint;

    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private float nextActionTime;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 3, nextActionTime);        
    }

    void SpawnEnemy()
    {
        foreach (var item in enemySpawnPoint)
        {
            Instantiate(enemy, item.position, Quaternion.identity);
        }        
    }

}
