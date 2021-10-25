using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarDriverAgent : Agent 
{
    private TrackCheckpoints trackCheckpoints;

    private BotCarController botCarController;
    private int lastEpisodeResetCount = 0;
    // counts how many checkpoints have been passed in the current episode
    private int checkpointCount;
    private float currentWheelAngle;
    private float lastWheelAngle;
    private float wheelAngleChange;
    private float currentSpeed;
    private float speedSum;
    private bool leftTurnAhead;
    private bool rightTurnAhead;
    private int stepsStayedInContact;
    private void Awake() {
        Application.runInBackground = true;

        botCarController = GetComponent<BotCarController>();

        trackCheckpoints = GameObject.Find("CheckpointHandler").GetComponent<TrackCheckpoints>();
        trackCheckpoints.OnVehicleCorrectCheckpoint += TrackCheckpoints_OnVehicleCorrectCheckpoint;
        trackCheckpoints.OnVehicleWrongCheckpoint += TrackCheckpoints_OnVehicleWrongCheckpoint;
    }

    private void FixedUpdate()
    {
        // Will ensure every 40 episodes, the track and cars do a hard reset
        int episodes = gameObject.GetComponent<Agent>().CompletedEpisodes;
        if (episodes > 0 && episodes % 30 == 0 && lastEpisodeResetCount != episodes)
        {
            // Do a hard reset
            trackCheckpoints.ResetAll();
            lastEpisodeResetCount = episodes;
        }

        speedSum += currentSpeed;
        
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        rewardHighSpeed();

        currentWheelAngle = botCarController.getCurrentSteeringAngle();
        wheelAngleChange = currentWheelAngle - lastWheelAngle;
        rewardStraightSteering();
        lastWheelAngle = currentWheelAngle;

        leftTurnAhead = isLeftTurnAhead();
        rightTurnAhead = isRightTurnAhead();

        //updateMotorForce();

        //rewardFirstPlace();

        // Reset bot to track if very far from player
        if (GameObject.FindGameObjectWithTag("Player") && !trackCheckpoints.isTraining)
        {
            var playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (Vector3.Distance(transform.position, playerTransform.position) > 2000f)
            {
                trackCheckpoints.softResetToCheckpoint(transform);
            }
        }

        if(trackCheckpoints.isTraining && !trackCheckpoints.isFirst(transform) && trackCheckpoints.getFirstPlace()) {
            Transform firstPlace = trackCheckpoints.getFirstPlace();
            if (Vector3.Distance(transform.position, firstPlace.position) > 2000f)
            {
                //agent is stuck
                trackCheckpoints.softResetToCheckpoint(transform);
            }
        }
    }

    private void updateMotorForce(){
        if(trackCheckpoints.getCarTransforms().Count > 1) {
            float distanceToFirst = 1f;
            foreach(Transform t in trackCheckpoints.getCarTransforms()) {
                if(t != transform && trackCheckpoints.isFirst(t)) {
                    distanceToFirst = Vector3.Distance(transform.position, t.position);
                    float adjustment = distanceToFirst * 10000f;
                    if(adjustment < float.MaxValue) {
                        botCarController.adjustMotorForce(adjustment);
                        Debug.Log("Car " + trackCheckpoints.findCarIndex(transform) + ": " + botCarController.getMotorForce());
                    }
                } else {
                    botCarController.defaultMotorForce();
                }
            }
        }
    }
    private void rewardFirstPlace() {
        trackCheckpoints.EvaluatePlaces();
        if(trackCheckpoints.isFirst(transform)) {
           // Debug.Log("First Place: Car " + trackCheckpoints.findCarIndex(transform));
            AddReward(+1.0e-4f);
        }
    }
    private void rewardStraightSteering() {
        if(!isLeftTurnAhead() && !isRightTurnAhead() && currentSpeed > 5) {
            if(wheelAngleChange == 0f) {
                AddReward(0.1f);        
            } else if(wheelAngleChange > 0f && wheelAngleChange < 45f) {
                AddReward(+ 0.01f / wheelAngleChange); // gives higher rewards for less Angle change (when less than 45)
            } else {
                AddReward(- 0.5f * wheelAngleChange); // gives lower penalty for less Angle change (when more than 45)
            }
        }
    }
    private void rewardHighSpeed() {
        bool goingForward = botCarController.isGoingForward();
        // no reward/penalty for reversing or neutral
        if(goingForward && currentSpeed > 5) {
            // going forward
            AddReward(currentSpeed * 0.1f); // heuristic avg speeds usually range from 16 - 18
        }
    }

    private string TurnAhead() {
        int idx = trackCheckpoints.findCarIndex(transform);
        Checkpoint checkpoint = trackCheckpoints.get_Nth_NextCheckpoint(idx, 2);
        if(checkpoint) {
            string trackpieceTag = checkpoint.transform.parent.transform.parent.tag;
            return trackpieceTag;
        }
        return null;
    }

    private bool isLeftTurnAhead() {
        // if(TurnAhead() == "TrackTurnLeft" && currentSpeed > 10)
        // Debug.Log("LEFT");

        if(currentSpeed > 5){
            return TurnAhead() == "TrackTurnLeft";
        } else {
            return false;
        }
    }

    private bool isRightTurnAhead() {
        // if(TurnAhead() == "TrackTurnRight" && currentSpeed > 10)
        // Debug.Log("RIGHT");

        if(currentSpeed > 5){
            return TurnAhead() == "TrackTurnRight";
        } else {
            return false;
        }
    }
    private void TrackCheckpoints_OnVehicleCorrectCheckpoint(object sender, TrackCheckpoints.TrackCheckpointEventArgs e) {
        // Event captured, check if that event belongs to this vehicle by check transform equality
        if (e.vehicleTransform == transform) {
            AddReward(+1f);
            //Debug.Log("correct checkpoint in agent");
            
            checkpointCount++;
            if(checkpointCount >= 200) {
                Debug.Log("Avg Speed: " + speedSum / StepCount);
                EndEpisode();
            }
            //Debug.Log(checkpointCount);
        }
    }

    // Same as above but with negative reward
    private void TrackCheckpoints_OnVehicleWrongCheckpoint(object sender, TrackCheckpoints.TrackCheckpointEventArgs e) {
        if (e.vehicleTransform == transform) {
            //Debug.Log("wrong checkpoint in agent");   
            AddReward(-1f);
        }
    }

    public override void OnEpisodeBegin() {
        /* Old system where all vehicles are reset (not a good approach)
        trackCheckpoints.ResetAll();
        // Vehicle reset is handled by trackCheckpoints script as it will apply to all vehicles on the track
        // Doesn't make much sense but this is needed to ensure all vehicles on the track are reset properly
        // //float carSpacing = transform.localScale.x * (1 + trackCheckpoints.findCarIndex(transform));
        // float carSpacing = 3 * (trackCheckpoints.findCarIndex(transform));
        // float x_axis = carSpacing + 16.2999992f;
        // // transform.localPosition = new Vector3(UnityEngine.Random.Range(5f, 15f), +0f, 2f);
        // transform.localPosition = new Vector3(x_axis,0.289999992f,1.78999996f);
        // gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // transform.localRotation = Quaternion.Euler(0,0,0);
        */

        /*  
            **New Episode Reset System**
            Reset a particular vehicle to the checkpoint that exists in the middle of the
            procedurally spawned track. Due to episode end, model understands that it is 
            "resetting" to a new state for that agent and substantial negative reward
            reinforces against this outcome.
        */
        checkpointCount = 0;
        lastWheelAngle = 0;
        wheelAngleChange = 0;
        currentSpeed = 0;
        speedSum = 0;
        leftTurnAhead = false;
        rightTurnAhead = false;

        trackCheckpoints.softResetToCheckpoint(transform);
    }

    public override void CollectObservations(VectorSensor sensor) {
        // In addition to the Ray Perception Sensor, this observation will make sure the model learns to 
        // face the same Angle as the checkpoints forward
        // This ensures that it learns to keep itself pointing in the right Angle
        if (trackCheckpoints.GetNumCheckpoints() > 0) {
            var nextCheckpoint = trackCheckpoints.GetNextCheckpoint(transform);
            if (nextCheckpoint != null) {
                Vector3 checkpointForward = nextCheckpoint.transform.TransformDirection(Vector3.forward);
        
                float AngleDot = Vector3.Dot(transform.forward, checkpointForward);
                sensor.AddObservation(AngleDot);
            } else {
                trackCheckpoints.softResetToCheckpoint(transform);
            }
        } else {
            trackCheckpoints.softResetToCheckpoint(transform);
        }

        sensor.AddObservation(wheelAngleChange);

        sensor.AddObservation(currentSpeed);

        sensor.AddObservation(leftTurnAhead);

        sensor.AddObservation(rightTurnAhead);

        // observe distance to other cars on the track (if there are other cars on the track)
        /*
        if(trackCheckpoints.getCarTransforms().Count > 1) {
            float closestDistance = float.MaxValue;
            foreach(Transform t in trackCheckpoints.getCarTransforms()) {
                if(t != transform) {
                    float distance = Vector3.Distance(transform.position, t.position);
                    if(distance < closestDistance) {
                        // Debug.Log(distance);
                        closestDistance = distance;
                    }
                }
            }
            sensor.AddObservation(closestDistance);
        }
        */
    }

    public override void OnActionReceived(ActionBuffers actions) {
        BotCarController targetScript = gameObject.GetComponent<BotCarController>();
        targetScript.SetInputs(actions.ContinuousActions[0], actions.DiscreteActions[0]);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        if(Input.GetKey(KeyCode.UpArrow)) {
            // forward
            discreteActions[0] = 2;
        }
        else if(Input.GetKey(KeyCode.DownArrow)) {
            //reverse
            discreteActions[0] = 0;
        } else {
            //neutral
            discreteActions[0] = 1;
        }
    }

    // 
    private void OnCollisionEnter(Collision other) {
        // Collision Reward
        if(other.collider.tag == "VehicleBody") {
            AddReward(+6f); 
            Debug.Log("Collision Reward!");
            stepsStayedInContact = 0;
        }
    }

    private void OnCollisionStay(Collision other) {
        if(other.collider.tag == "VehicleBody") {
            stepsStayedInContact++;
            if(stepsStayedInContact > 50)
                Debug.Log("TOO MUCH");
                AddReward(-1.5f);
        }    
    }

    // Penalise the agent if it slides/hits the boundary walls of the track
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "End Barrier") {
            AddReward(-5f);
            //Debug.Log("END BARRIER COLLISION TRIGGERED");
            /*
             Do SOFT RESET for vehicle agent, hard reset should only be triggered when 
             BOTH vehicles are to be reset with track checkpoints and track spawner is being reset.
             Reason is to avoid one vehicle hard reset causing reset of both vehicles where in a particular
             case that one vehicle may be doing well in that episode and SHOULD NOT be reset due to failure of other vehicle.
            */
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-2f);
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-2f);
        }
    }
}
