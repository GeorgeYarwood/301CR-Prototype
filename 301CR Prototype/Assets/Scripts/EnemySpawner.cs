using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Spawn transform
    public GameObject enemySpawn;
    //Enemy game object
    public GameObject enemyGO;

    //Add every enemy spawned to this list
    List<GameObject> enemies = new List<GameObject>();

    float spawnTimer = 15;
    bool canSpawn;

    int maxEnemies = 15;

    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn && enemies.Count < maxEnemies) 
        {
            GameObject clone = Instantiate(enemyGO, enemySpawn.transform);
            clone.tag = "enemy";
            enemies.Add(clone);
            StartCoroutine(spawnWait());
        }


    }

    IEnumerator spawnWait() 
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnTimer);
        canSpawn = true;
    }


}
