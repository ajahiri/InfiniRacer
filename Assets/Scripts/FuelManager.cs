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

        if (checkFuel < 10)
        {
            if (refil == false)
            {
                refil = true;
                GameObject[] gas = FindObjectsOfType(typeof(GameObject)) as GameObject[];
                foreach (GameObject go in gas)
                {
                    if (go.layer == 6)
                    {
                        go.GetComponent<itemSpawn>().SpawnFuel();
                    }
                }
            }
        }
        else refil = false;
    }
}
