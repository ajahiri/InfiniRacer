using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] item;
    public static int diff;

    private TrackCheckpoints checkHandler;

    private void Start()
    {
        // Get loaded item references for spawning items quickly
        checkHandler = GameObject.Find("CheckpointHandler").GetComponent<TrackCheckpoints>();

        diff = (int)PlayerPrefs.GetFloat("GlobalDifficulty", 3);

        int buffLikelyness = Random.Range(0, 6);

        if (GameObject.FindGameObjectWithTag("Player") && checkHandler.buffItems.Length > 0 && checkHandler.deBuffItems.Length > 0 && checkHandler.rockItems.Length > 0)
        {
            // More rocks should spawn as difficulty increases
            if (buffLikelyness < diff)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        Instantiate(checkHandler.rockItems[Random.Range(0, 4)], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 3, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);
    }

}
