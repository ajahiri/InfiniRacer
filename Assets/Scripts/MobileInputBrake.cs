using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputBrake : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private CarController carController;

    public void OnPointerDown(PointerEventData eventData)
    {
        carController.setBraking(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carController.setBraking(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
    }
} 
