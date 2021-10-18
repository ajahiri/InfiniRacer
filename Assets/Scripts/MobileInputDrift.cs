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
        isDrifting = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrifting = false;
    }

    // Start is called before the first frame update
    void Start()
    {       
        PrefCar = GameObject.Find("SetupForCar").gameObject.GetComponent<InstantiateCar>().PrefCar;
        carController = GameObject.Find(PrefCar + "(Clone)").GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Mobile Drift " + carController.isDrifting);
        if (isDrifting)
        {
            carController.DriftButtonPressed = true;
            carController.enableDrifting();
        } 
        if (!isDrifting)
        {
            carController.DriftButtonPressed = false;
            carController.disableDrifting();
        }
    }
}
