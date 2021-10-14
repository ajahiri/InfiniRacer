using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileBrakeInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    CarController carController;
    bool isBraking;

    public void OnPointerDown(PointerEventData eventData)
    {
        isBraking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isBraking = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        carController = GameObject.Find("lambo").GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBraking)
        {
            carController.isBraking = true;
            carController.HandleMotor();
            if (carController.vehicleRigidBody.velocity.magnitude > 0.7)
            {
                carController.motorForce = -20000;
            }

            else 
            {
                carController.vehicleRigidBody.velocity = Vector3.zero;
                carController.vehicleRigidBody.angularVelocity = Vector3.zero;
            }
        }
    }

    
}
