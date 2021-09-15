// Arian Jahiri 13348469 (10/08/2021)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    public bool isFlat = true;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool isBraking;
    
    [SerializeField] private bool tiltCont;


    public float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float vehicleStandardMass;

    private Rigidbody vehicleRigidBody;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void Start()
    {
        vehicleRigidBody = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        Vector3 tilt = Input.acceleration;

        if (isFlat)
            tilt = Quaternion.Euler(90, 0, 0) * tilt;


        //vehicleRigidBody.AddForce(Input.acceleration);
        Debug.DrawRay(transform.position + Vector3.up, tilt, Color.cyan);
    }

    private void FixedUpdate()
    {
        // Scale the vehicle's mass with speed (downforce simulation) for high speed cornering
        vehicleRigidBody.mass = vehicleStandardMass + (10f * vehicleRigidBody.velocity.magnitude);
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        if (tiltCont)
        {
            horizontalInput = Input.acceleration.x;
        }
        else { horizontalInput = Input.GetAxis(HORIZONTAL); }
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    public void GetBoost(float x)
    {
        BoostCoroutine(x);
    }

    IEnumerator BoostCoroutine(float boost)
    {
        motorForce *= boost;
        yield return new WaitForSeconds(2);
        motorForce /= boost;


    }

    private void HandleMotor()
    {
        if (verticalInput == -1) {
            rearLeftWheelCollider.motorTorque = -motorForce;
            rearRightWheelCollider.motorTorque = -motorForce;
        } else {
            rearLeftWheelCollider.motorTorque = isBraking ? 0 : motorForce;
            rearRightWheelCollider.motorTorque = isBraking ? 0 : motorForce;
        }
        
        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteeringAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.SetPositionAndRotation(pos, rot);
    }
}
