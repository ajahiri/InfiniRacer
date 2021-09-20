using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class itemSpawner : MonoBehaviour
{

    [SerializeField] private GameObject[] item;

    
    private void Start()
    {
        
        Spawn();
        
    }

    private void Spawn()
    {
        var currentObj = Instantiate(item[Random.Range(0, item.Length)],new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        currentObj.transform.parent = gameObject.transform;

    }

}

