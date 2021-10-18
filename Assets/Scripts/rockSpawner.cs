using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] item;
    public static int diff;


    private void Start()
    {

        int StandartDifficulty = 3;
        diff = (int)PlayerPrefs.GetFloat("GlobalDifficulty", 3);
        int rand = Random.Range(0, (StandartDifficulty+ diff));
        if (rand == StandartDifficulty)
        {
            Spawn();
        }
        


    }

    private void Spawn()
    {
        var currentObj = Instantiate(item[Random.Range(0, item.Length)], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 3, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);
    }
}
