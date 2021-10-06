using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelectedCar : MonoBehaviour
{
    // Start is called before the first frame update
    public string car;
    public static SaveSelectedCar instance;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        { Destroy(gameObject); };

        DontDestroyOnLoad(gameObject);

    }

    public void setCar(string pcar)
    {
        car = pcar;
    }
}
