using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    //Spawn transform
    public GameObject enemySpawn;
    //Enemy game object
    public GameObject enemyGO;

    //Add every enemy spawned to this list
    public static List<GameObject> enemies = new List<GameObject>();

    float spawnTimer = 1;
    bool canSpawn;

    int maxEnemies;

    static public int currRound;

    bool running= false;

    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;

        currRound = 1;

        
        maxEnemies = 2;
    }

    // Update is called once per frame
    void Update()
    {


        

        if (canSpawn && enemies.Count < maxEnemies)
        {
            GameObject clone = Instantiate(enemyGO, enemySpawn.transform);
            clone.tag = "enemy";
            enemies.Add(clone);
            StartCoroutine(spawnWait());
        }
        //If all enemies have been spawned in
        else if(enemies.Count >= maxEnemies)
        {
            //Find all enemies in scene
            GameObject[] aliveEnemies = GameObject.FindGameObjectsWithTag("enemy");

            //If all enemies have been killed
            if (aliveEnemies.Length <= 0) 
            {


                if (!running) 
                {
                    StartCoroutine(nextRnd());

                }
            }
        }


    }

    IEnumerator nextRnd()
    {
        canSpawn = false;
        running = true;
        yield return new WaitForSeconds(3);
        //Reset list
        enemies.Clear();
        //Next round
        currRound += 1;
        maxEnemies += (currRound * 2);
        running = false;
        canSpawn = true;
    }

    

    IEnumerator spawnWait() 
    {
        canSpawn = false;
        float hack = (Random.Range(1, 5) - currRound / 2);
        if(hack >= 0) 
        {
            yield return new WaitForSeconds(spawnTimer + hack);

        }
        else 
        {
            yield return new WaitForSeconds(spawnTimer
                );

        }
        canSpawn = true;
    }


}
