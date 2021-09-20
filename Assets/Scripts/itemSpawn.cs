using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] item;


    private void Start()
    {

        Spawn();

    }

    private void Spawn()
    {
        var currentObj = Instantiate(item[Random.Range(0, item.Length)], new Vector3(Random.Range(transform.position.x-7, transform.position.x+7), transform.position.y+2, transform.position.z), Quaternion.identity);

    }
}
