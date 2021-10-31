// Arian Jahiri 13348469 (10/08/2021)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotCarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool isBraking;
    [SerializeField] private float motorForce;
    private float appliedForce;
    private float initialMotorForce;
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

    [SerializeField] private Vector3 customCenterofMass = Vector3.zero;

    private float currentSpeed;
    private float speedLimit;
    private float initialSpeedLimit;

    public float getMotorForce() {
        return motorForce;
    }
    public void adjustMotorForce(float adjustment) {
        motorForce = initialMotorForce + adjustment;
    }
    public void defaultMotorForce(){
        motorForce = initialMotorForce;
    }
    public float getCurrentSteeringAngle() {
        return currentSteeringAngle;
    }

    public bool isReversing() {
        if(appliedForce == -motorForce){
            return true;
        }
        return false;
    }

    public bool isGoingForward() {
        if(appliedForce == motorForce) {
            return true;
        }
        return false;
    }

    public bool isNeutral() {
        if(appliedForce == 0f) {
            return true;
        }
        return false;
    }
    public float getSpeedLimit() {
        return speedLimit;
    }
    public void setSpeedLimit(float newLimit) {
        speedLimit = newLimit;
    }

    public void defaultSpeedLimit() {
        speedLimit = initialSpeedLimit;
    }
    private void Awake() {
        initialMotorForce = motorForce;
        currentSpeed = 0;
        isBraking = false;

        int difficulty = (int)PlayerPrefs.GetFloat("GlobalDifficulty", 3);
        initialSpeedLimit = 14 + (difficulty - 1); // | difficulty : speedLimit | 1 : 12 | 2 : 14 | 3 : 16 | 4 : 18 | 5 : 20 |
        speedLimit = initialSpeedLimit;
    }
    private void Start() {
        // Set custom center of mass to fix flipping issue
        vehicleRigidBody = GetComponent<Rigidbody>();
        vehicleRigidBody.centerOfMass = customCenterofMass;
    }
    private void FixedUpdate()
    {
        // Scale the vehicle's mass with speed (downforce simulation) for high speed cornering
        vehicleRigidBody = GetComponent<Rigidbody>();
        currentSpeed = vehicleRigidBody.velocity.magnitude;
        // vehicleRigidBody.mass = vehicleStandardMass + (10f * vehicleRigidBody.velocity.magnitude);
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    public void SetInputs(float newHorizontal, int accelerationValue) {
        horizontalInput = newHorizontal;
        isBraking = false;
        //Debug.Log("Bot Speed: " + currentSpeed);
        if(accelerationValue == 2) {
            //forward
            if(currentSpeed <= speedLimit) { //speed limiter
                appliedForce = motorForce;
            } else {
                appliedForce = 0f;
                isBraking = true;
            }
        } else if(accelerationValue == 1) {
            //neutral
            appliedForce = 0f;
        } else if(accelerationValue == 0) {
            //reverse
            appliedForce = -motorForce;
        }

        //isBraking = brakingInput;
        //Debug.Log("setting inputs " + horizontalInput + " " + verticalInput + " " + isBraking);
        //Debug.Log("vertical input: " + verticalInput);
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = appliedForce;
        rearRightWheelCollider.motorTorque = appliedForce;

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
