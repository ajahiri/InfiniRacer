// Arian Jahiri 13348469 (10/08/2021)
// Wai Yan 13334483 (14/09/2021) implemented codes for Drifting and Smoke particle system, UpdateDrift() & DriftSingleWheel()
// Drifting function is a little bit tedious, however the code can be used for when the player bump into obstacles as
// it's making the car sliping rather than drifting

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 



public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    public bool isFlat = true;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteeringAngle;
    public float currentBrakeForce;
    public bool isBraking;
    public bool isDrifting;
    public float motorForce;

    // Remembering original values for proper use of speed up and drift
    public float originalMotorForce;
    public WheelFrictionCurve originalForwardFriction;
    public WheelFrictionCurve originalSidewayFriction;

    private TrackCheckpoints trackCheckpointsScript;

    [SerializeField] public float brakeForce;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float vehicleStandardMass;
    [SerializeField] private bool tiltCont;

    public bool DriftButtonPressed;
    public Rigidbody vehicleRigidBody;

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
    private GameObject wrongway;
         

    [SerializeField] private Vector3 customCenterofMass = Vector3.zero;


 

    private void Start()
    {
        tepeat = false;
        tiltCont = true;
        DriftButtonPressed = false;
        //gameOver = GameObject.Find("GameOver").gameObject.transform.GetComponent<GameOver>();
        // Set custom center of mass to fix flipping issue
        vehicleRigidBody = GetComponent<Rigidbody>();
        vehicleRigidBody.centerOfMass = customCenterofMass;
        fuel = GameObject.Find("FuelBar").gameObject.transform.GetComponent<FuelSystem>();
        //gameOver.Disable();

        trackCheckpointsScript = GameObject.Find("CheckpointHandler").GetComponent<TrackCheckpoints>();

        originalForwardFriction = rearLeftWheelCollider.forwardFriction;
        originalSidewayFriction = rearLeftWheelCollider.sidewaysFriction;
        originalMotorForce = motorForce;

        wrongway = GameObject.FindWithTag("WrongWay");
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
            GameoverSeq(1);
        }

        CheckWrongWay();
    }

    private void CheckWrongWay()
    {
        var nextCheckpoint = trackCheckpointsScript.GetNextCheckpoint(transform);
        var checkpointForward = nextCheckpoint.transform.forward;
        var vehicleForward = transform.forward;
        var dotProd = Vector3.Dot(checkpointForward, vehicleForward);
        if (dotProd > 0)
        {
            wrongway.GetComponent<Text>().enabled = false;

        } else
        {
            wrongway.GetComponent<Text>().enabled = true;

        }
    }

    public void GameoverSeq(int dfq)
    {
        if (dfq == 1)
        {
            motorForce = 0;
            GetBoost(0f);
            FindObjectOfType<AudioManager>().Stop("CarEngine");
            FindObjectOfType<AudioManager>().Play("Game Over");
            SceneManager.LoadScene("GameOver");
        }else if(dfq == 2)
        {
            motorForce = 0;
            GetBoost(0f);
            FindObjectOfType<AudioManager>().Stop("CarEngine");
            FindObjectOfType<AudioManager>().Play("BIIIIIIGBOOOOM");
            SceneManager.LoadScene("GameOver");
        }
    }

    private void FixedUpdate()
    {
        // Scale the vehicle's mass with speed (downforce simulation) for high speed cornering
        //vehicleRigidBody.mass = vehicleStandardMass + (10f * vehicleRigidBody.velocity.magnitude);
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private bool isPressingBrake = false;
    public void setBraking(bool newBraking)
    {
        isPressingBrake = newBraking;
    }

    private void GetInput()
    {
        if (tiltCont)
        {
            horizontalInput = Input.acceleration.x;
            //isBraking = Input.touchCount > 0;
            isBraking = isPressingBrake;
        }
        else
        {
            horizontalInput = Input.GetAxis(HORIZONTAL);
            isBraking = Input.GetKey(KeyCode.Space);
        }

        // if (Input.GetKey(KeyCode.K)) isDrifting = true;
        // if (Input.GetKey(KeyCode.L)) isDrifting = false;
        if (Input.GetKey(KeyCode.K))
        {
            enableDrifting();
        }
        if (Input.GetKey(KeyCode.L))
        {
            disableDrifting();
        }

        verticalInput = Input.GetAxis(VERTICAL);

    }

    public void GetBoost(float x)
    {
        StartCoroutine(BoostCoroutine(x));
    }

    IEnumerator BoostCoroutine(float boost)
    {
        motorForce = originalMotorForce * boost;
        var newAsymptote = originalForwardFriction.asymptoteValue * 3;
        var newExtreme = originalForwardFriction.extremumValue * 3;
        var newFriction = originalForwardFriction;
        newFriction.asymptoteValue = newAsymptote;
        newFriction.extremumValue = newExtreme;
        rearLeftWheelCollider.forwardFriction = newFriction;
        rearRightWheelCollider.forwardFriction = newFriction;
        yield return new WaitForSeconds(2);
        motorForce = originalMotorForce;
        rearLeftWheelCollider.forwardFriction = originalForwardFriction;
        rearRightWheelCollider.forwardFriction = originalForwardFriction;
        yield break;
    }

    public void HandleMotor()
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

        if (isBraking)
        {
            if (vehicleRigidBody.velocity.magnitude < 3)
            {
                // Do reverse not brake
                currentBrakeForce = 0f;
                rearLeftWheelCollider.motorTorque = -motorForce;
                rearRightWheelCollider.motorTorque = -motorForce;
            } else
            {
                // Do normal brake
                currentBrakeForce = brakeForce;
            }
        } else
        {
            currentBrakeForce = 0f;
        }
        ApplyBraking();
    }

    public void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
        if (!isBraking && !isDrifting)
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

    public void enableDrifting()
    {
        isDrifting = true;
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Play();
        }
        // Left rear
        WheelFrictionCurve forwardFriction = rearLeftWheelCollider.forwardFriction;
        forwardFriction.stiffness = 0.7f;
        forwardFriction.asymptoteValue = 1.0f;
        WheelFrictionCurve sidewayFriction = rearLeftWheelCollider.sidewaysFriction;
        sidewayFriction.stiffness = 0.7f;
        sidewayFriction.asymptoteValue = 0.8f;

        rearLeftWheelCollider.forwardFriction = forwardFriction;
        rearLeftWheelCollider.sidewaysFriction = sidewayFriction;
        rearRightWheelCollider.forwardFriction = forwardFriction;
        rearRightWheelCollider.sidewaysFriction = sidewayFriction;

        motorForce = originalMotorForce / 4;
    }
    public void disableDrifting()
    {
        isDrifting = false;

        rearLeftWheelCollider.forwardFriction = frontLeftWheelCollider.forwardFriction;
        rearLeftWheelCollider.sidewaysFriction = frontLeftWheelCollider.sidewaysFriction;

        rearRightWheelCollider.forwardFriction = frontLeftWheelCollider.forwardFriction;
        rearRightWheelCollider.sidewaysFriction = frontLeftWheelCollider.sidewaysFriction;

        /*rearLeftWheelCollider.forwardFriction = originalForwardFriction;
        rearLeftWheelCollider.sidewaysFriction = originalSidewayFriction;
        rearRightWheelCollider.forwardFriction = originalForwardFriction;
        rearRightWheelCollider.sidewaysFriction = originalSidewayFriction; */

        motorForce = originalMotorForce;
    }
}
