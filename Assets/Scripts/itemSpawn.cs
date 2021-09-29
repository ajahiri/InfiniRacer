using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] item;
    public int diff;


    private void Start()
    {

        int rand = Random.Range(0, diff+1);
        if(rand == 5)
            Spawn();

    }

    private void Spawn()
    {
        var currentObj = Instantiate(item[Random.Range(0, item.Length)], new Vector3(Random.Range(transform.position.x-7, transform.position.x+7), transform.position.y+2, Random.Range(transform.position.z - 7, transform.position.z + 7)),
            transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);

    }
}
