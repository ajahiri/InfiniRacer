using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeManager : MonoBehaviour
{
    [SerializeField] private GameObject Nuke;
    List<GameObject> checkpoints = new List<GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        foreach (GameObject go in gos)
        {
            if (go.layer.Equals("Checkpoints"))
            {
                checkpoints.Add(go);
            }
        }

        //Debug.Log(checkpoints.Count);
        if (checkpoints.Count > 101)
        {
            Debug.Log(checkpoints.Count);

            //var GiveMoreFuel = Instantiate(Nuke, new Vector3(Random.Range(checkpoints[checkpoints.Count].transform.position.x - 7, checkpoints[checkpoints.Count].transform.position.x + 7), checkpoints[checkpoints.Count].transform.position.y + 2, Random.Range(checkpoints[checkpoints.Count].transform.position.z - 7, checkpoints[checkpoints.Count].transform.position.z + 7)),
            //   checkpoints[checkpoints.Count].transform.parent.parent.rotation, GameObject.Find("TrackSpawner").transform);
        }
    }
}
