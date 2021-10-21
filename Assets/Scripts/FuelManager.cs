using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelManager : MonoBehaviour
{
    float checkFuel;
    public bool refil = false;

    
    public void Update()
    {
        checkFuel = GameObject.Find("FuelBar").GetComponent<FuelSystem>().Fuel;

        if (checkFuel < 5)
        {
            if (refil == false)
            {
                refil = true;
                var playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                Checkpoint[] nextCheckpoints = GameObject.Find("CheckpointHandler").GetComponent<TrackCheckpoints>().GetNextFourCheckpoints(playerTransform);
                nextCheckpoints[2].GetComponent<itemSpawn>().SpawnFuel();
            }
        }
        else refil = false;
    }
}
