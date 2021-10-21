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
            if (carController.vehicleRigidBody.velocity.magnitude > 6)
            {
                
                carController.motorForce = -6500;
                Debug.Log(carController.vehicleRigidBody.velocity.magnitude);
                return;
            }
            if (carController.vehicleRigidBody.velocity.magnitude <= 6)
            {
                Debug.Log("Second if loop " + carController.vehicleRigidBody.velocity.magnitude);
                carController.motorForce = -3;
                for (int i = 0; i < carController.smoke.Length; i++)
                {
                    carController.smoke[i].Pause();
                }
                return;
            } 
        } 
        if (!isBraking)
        {
            carController.isBraking = false;
            carController.motorForce = 5000;
        }
    }
} 
