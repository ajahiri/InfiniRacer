using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeManager : MonoBehaviour
{
    bool nukeSpawned = false;

    // Update is called once per frame
    private void Start()
    {
        StartCoroutine(WaitAndPrint(3.0f));
    }
    private IEnumerator WaitAndPrint(float waitTime)
    {
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> checkpoints = new List<GameObject>();
        foreach (GameObject go in gos)
        {
            //Debug.Log(go.name);
            if (go.layer == 6)
            {
                checkpoints.Add(go);
            }
        }
        if (checkpoints.Count > 50)
        {
            if (nukeSpawned == false)
            {
                int ChosenOne = ((int)checkpoints.Count) - 1;
                nukeSpawned = true;
                checkpoints[0].GetComponent<itemSpawn>().SpawnNuke();
                Debug.Log("Spawned");
            }

        }
        yield return new WaitForSeconds(waitTime);
        if (nukeSpawned == false) { StartCoroutine(WaitAndPrint(3.0f)); }
        else if (nukeSpawned == true) { StartCoroutine(justWait(60.0f)); }
    

    }

    private IEnumerator justWait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nukeSpawned = false;
        StartCoroutine(WaitAndPrint(3.0f));
    }

}
