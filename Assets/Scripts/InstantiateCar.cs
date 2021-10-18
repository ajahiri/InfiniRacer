using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> vehicleObjects;
    public static InstantiateCar instance;
    public string PrefCar;

    void Awake()
    {
        PrefCar = GameObject.Find("PlayerPref").gameObject.GetComponent<SaveSelectedCar>().car;
        foreach(GameObject car in vehicleObjects)
        {
            if(car.name == PrefCar)
            {
                Instantiate(car, GameObject.Find("RaceArea01").transform);
            }
        }
        
    }

}
