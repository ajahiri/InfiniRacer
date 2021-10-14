using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MobileDriftInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    CarController carController;
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
        carController = GameObject.Find("lambo").GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrifting)
        {
            carController.isDrifting = true;
            carController.UpdateDrift();
        }
        else
        {
            carController.isDrifting = false;
            carController.UpdateDrift();
        }
    }
}
