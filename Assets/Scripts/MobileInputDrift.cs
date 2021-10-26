using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputDrift : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private CarController carController;

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
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
    }
}
