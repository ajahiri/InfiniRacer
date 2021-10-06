// Arian Jahiri 13348469 (10/08/2021)
// Wai Yan 13334483 (14/09/2021) implemented codes for Drifting and Smoke particle system, UpdateDrift() & DriftSingleWheel()
// Drifting function is a little bit tedious, however the code can be used for when the player bump into obstacles as
// it's making the car sliping rather than drifting

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



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
    private bool isDrifting;
    public float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float vehicleStandardMass;
    [SerializeField] private bool tiltCont;

    private Rigidbody vehicleRigidBody;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    public ParticleSystem[] smoke;
    FuelSystem fuel;
    private bool tepeat;
         

    [SerializeField] private Vector3 customCenterofMass = Vector3.zero;


 

    private void Start()
    {
        tepeat = false;
        //gameOver = GameObject.Find("GameOver").gameObject.transform.GetComponent<GameOver>();
        // Set custom center of mass to fix flipping issue
        vehicleRigidBody = GetComponent<Rigidbody>();
        vehicleRigidBody.centerOfMass = customCenterofMass;
        fuel = GameObject.Find("FuelBar").gameObject.transform.GetComponent<FuelSystem>();
        //gameOver.Disable();

    }
    public void Update()
    {
        Vector3 tilt = Input.acceleration;

        if (isFlat)
            tilt = Quaternion.Euler(90, 0, 0) * tilt;


        //vehicleRigidBody.AddForce(Input.acceleration);
        Debug.DrawRay(transform.position + Vector3.up, tilt, Color.cyan);

        if (fuel.Fuel == 0 && tepeat == false)
        {
            tepeat = true;
            GameoverSeq();
        }
    }
    public void GameoverSeq()
    {
        motorForce = 0;
        GetBoost(0f);
        FindObjectOfType<AudioManager>().Stop("CarEngine");
        FindObjectOfType<AudioManager>().Play("Game Over");
        Debug.Log("motorForce is " + motorForce);
        SceneManager.LoadScene("GameOver");
    }

    private void FixedUpdate()
    {
        // Scale the vehicle's mass with speed (downforce simulation) for high speed cornering
        //vehicleRigidBody.mass = vehicleStandardMass + (10f * vehicleRigidBody.velocity.magnitude);
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateDrift();
    }

    private void GetInput()
    {
        if (tiltCont)
        {
            horizontalInput = Input.acceleration.x;
            isBraking = Input.touchCount > 0;
        }
        else
        {
            horizontalInput = Input.GetAxis(HORIZONTAL);
            isBraking = Input.GetKey(KeyCode.Space);
        }

        if (Input.GetKey(KeyCode.K)) isDrifting = true;
        if (Input.GetKey(KeyCode.L)) isDrifting = false;

        verticalInput = Input.GetAxis(VERTICAL);

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
        if (verticalInput == -1)
        {
            rearLeftWheelCollider.motorTorque = -motorForce;
            rearRightWheelCollider.motorTorque = -motorForce;
        }
        else
        {
            rearLeftWheelCollider.motorTorque = isBraking ? 0 : motorForce;
            rearRightWheelCollider.motorTorque = isBraking ? 0 : motorForce;
        }

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    public void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
        if (!isBraking)
        {
            for (int i = 0; i < smoke.Length; i++)
            {
                smoke[i].Pause();
            }

        }
        else if (isBraking)
        {
            for (int i = 0; i < smoke.Length; i++)
            {
                smoke[i].Play();
            }
        }
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

    public void UpdateDrift()
    {
        if (isDrifting)
        {
            DriftSingleWheel(rearLeftWheelCollider);
            DriftSingleWheel(rearRightWheelCollider);
            for (int i = 0; i < smoke.Length; i++)
            {
                smoke[i].Play();
            }

        }
        else
        {
            DriftSingleWheel(rearLeftWheelCollider);
            DriftSingleWheel(rearRightWheelCollider);
        }
    }

    private void DriftSingleWheel(WheelCollider wheelCollider)
    {
        WheelHit wheelHit;

        if (wheelCollider.GetGroundHit(out wheelHit))
        {
            if (isDrifting)
            {
                WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
                forwardFriction.stiffness = 0.7f;
                forwardFriction.asymptoteValue = 1.0f;
                wheelCollider.forwardFriction = forwardFriction;

                WheelFrictionCurve sidewayFriction = wheelCollider.sidewaysFriction;
                float tempoSidewayFriction = sidewayFriction.asymptoteValue;
                sidewayFriction.stiffness = 0.7f;
                sidewayFriction.asymptoteValue = 0.8f;
                wheelCollider.sidewaysFriction = sidewayFriction;

                motorForce = motorForce / 4;

            }
            else
            {
                WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
                forwardFriction.stiffness = 1f;
                forwardFriction.asymptoteValue = 0.5f;
                wheelCollider.forwardFriction = forwardFriction;

                WheelFrictionCurve sidewayFriction = wheelCollider.sidewaysFriction;
                float tempoSidewayFriction = sidewayFriction.asymptoteValue;
                sidewayFriction.stiffness = 1f;
                sidewayFriction.asymptoteValue = 1.3f;
                wheelCollider.sidewaysFriction = sidewayFriction;

                motorForce = 10000;
                
            }
        }
    }

    
}
