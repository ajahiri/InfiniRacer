using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] item;
    public static int diff;


    public void Start()
    {
        
        diff = (int)PlayerPrefs.GetFloat("GlobalDifficulty", 3);
        int rand = Random.Range(0, diff+1);
        
        
        if (rand == diff)
        {
            Spawn();
        }
        

    }
    
    public void Spawn()
    {
        var currentObj = Instantiate(item[Random.Range(0, item.Length-1)], new Vector3(Random.Range(transform.position.x-7, transform.position.x+7), transform.position.y+2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);

    }
    public void SpawnFuel()
    {
        var GiveMoreFuel = Instantiate(item[1], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
                transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);

    }
    public void SpawnNuke()
    {
        var GiveMorebombs = Instantiate(item[5], new Vector3(Random.Range(transform.position.x - 7, transform.position.x + 7), transform.position.y + 2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
                transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);


    }

}
