using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputDrift : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    CarController carController;
    public string PrefCar;
    bool isDrifting;

    public void OnPointerDown(PointerEventData eventData)
    {
        carController.enableDrifting();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carController.disableDrifting();
    }

    // Start is called before the first frame update
    void Start()
    {       
        PrefCar = GameObject.Find("SetupForCar").gameObject.GetComponent<InstantiateCar>().PrefCar;
        carController = GameObject.Find(PrefCar + "(Clone)").GetComponent<CarController>();
    }
}
