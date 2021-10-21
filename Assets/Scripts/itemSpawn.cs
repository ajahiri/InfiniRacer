using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] item;
    public static int diff;

    TrackCheckpoints checkHandler;

    public void Awake()
    {
        // Get loaded item references for spawning items quickly
        checkHandler = GameObject.Find("CheckpointHandler").GetComponent<TrackCheckpoints>();

        diff = (int)PlayerPrefs.GetFloat("GlobalDifficulty", 3);

        // Bomb and Bannana spawning increases with difficulty
        // Fuel, Coins and Boost spawning decreases with difficulty

        int buffLikelyness = Random.Range(0, 6);

        if (GameObject.FindGameObjectWithTag("Player") && checkHandler.buffItems.Length > 0 && checkHandler.deBuffItems.Length > 0)
        {
            if (buffLikelyness < diff)
            {
                SpawnDeBuffs();
            } else if (buffLikelyness >= diff)
            {
                SpawnBuffs();
            }
        }
    }

    public void SpawnDeBuffs()
    {
        Instantiate(checkHandler.deBuffItems[Random.Range(0, 2)], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);

    }

    public void SpawnBuffs()
    {
        Instantiate(checkHandler.buffItems[Random.Range(0, 3)], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);

    }

    /*
    public void Spawn()
    {
        var currentObj = Instantiate(item[Random.Range(0, item.Length-1)], new Vector3(Random.Range(transform.position.x-7, transform.position.x+7), transform.position.y+2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);

    }
    */

    public void SpawnFuel()
    {
        Instantiate(checkHandler.buffItems[2], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
                transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);
    }
    public void SpawnNuke()
    {
        Instantiate(item[5], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
                transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);
    }

}
