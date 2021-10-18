using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputBrake : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    CarController carController;
    public string PrefCar;
    bool isBraking = false;

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
        PrefCar = GameObject.Find("SetupForCar").gameObject.GetComponent<InstantiateCar>().PrefCar;
        carController = GameObject.Find(PrefCar + "(Clone)").GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBraking)
        {
            carController.isBraking = true;
            carController.currentBrakeForce = carController.brakeForce;
            carController.HandleMotor();
            if (carController.vehicleRigidBody.velocity.magnitude > 0.5)
            {
                carController.motorForce = -15000;
                Debug.Log(carController.vehicleRigidBody.velocity.magnitude);
            }

            else
            {
                carController.vehicleRigidBody.velocity = Vector3.zero;
                carController.vehicleRigidBody.angularVelocity = Vector3.zero;
                Debug.Log(carController.vehicleRigidBody.velocity.magnitude);
            } 
        } 
        if (!isBraking)
        {
            carController.isBraking = false;
            carController.motorForce = 5000;
        }
    }
} 
