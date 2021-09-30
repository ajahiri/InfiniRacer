using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarDriverAgent : Agent 
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;

    private BotCarController botCarController;
    private int lastEpisodeResetCount = 0;
    // counts how many checkpoints have been passed in the current episode
    private int checkpointCount = 0;
    private void Awake() {
        botCarController = GetComponent<BotCarController>();
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

        trackCheckpoints.EvaluatePlaces();
        if(trackCheckpoints.isFirst(transform)) {
            Debug.Log("First Place: Car " + trackCheckpoints.findCarIndex(transform));
            AddReward(+1.0e-4f);
        }
    }

    private void Start() {
        trackCheckpoints.OnVehicleCorrectCheckpoint += TrackCheckpoints_OnVehicleCorrectCheckpoint;
        trackCheckpoints.OnVehicleWrongCheckpoint += TrackCheckpoints_OnVehicleWrongCheckpoint;
    }

    private void TrackCheckpoints_OnVehicleCorrectCheckpoint(object sender, TrackCheckpoints.TrackCheckpointEventArgs e) {
        // Event captured, check if that event belongs to this vehicle by check transform equality
        if (e.vehicleTransform == transform) {
            AddReward(+1f);
            //Debug.Log("correct checkpoint in agent");
            
            checkpointCount++;
            //Gives higher reward for faster epsiode
            if(checkpointCount >= 200) {
                EndEpisode();
            }
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
        trackCheckpoints.softResetToCheckpoint(transform);
    }

    public override void CollectObservations(VectorSensor sensor) {
        // In addition to the Ray Perception Sensor, this observation will make sure the model learns to 
        // face the same direction as the checkpoints forward
        // This ensures that it learns to keep itself pointing in the right direction
        if (trackCheckpoints.GetNumCheckpoints() > 0) {
            var nextCheckpoint = trackCheckpoints.GetNextCheckpoint(transform);
            if (nextCheckpoint != null) {
                Vector3 checkpointForward = nextCheckpoint.transform.TransformDirection(Vector3.forward);
        
                float directionDot = Vector3.Dot(transform.forward, checkpointForward);
                sensor.AddObservation(directionDot);
            }
        }
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
        targetScript.SetInputs(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2] == 0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = Input.GetKey("space") == true ? 0f : 1f;
    }

    // 
    private void OnCollisionEnter(Collision other) {
        // Collision Reward
        if(other.collider.tag == "VehicleBody") {
            AddReward(+0.7f);
            //Debug.Log("Collision Reward!");
        }
    }

    private void OnCollisionStay(Collision other) {
        if(other.collider.tag == "VehicleBody") {
            AddReward(-0.5f);
        }    
    }

    // Penalise the agent if it slides/hits the boundary walls of the track
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "End Barrier") {
            AddReward(-2f);
            /*
             Do SOFT RESET for vehicle agent, hard reset should only be triggered when 
             BOTH vehicles are to be reset with track checkpoints and track spawner is being reset.
             Reason is to avoid one vehicle hard reset causing reset of both vehicles where in a particular
             case that one vehicle may be doing well in that episode and SHOULD NOT be reset due to failure of other vehicle.
            */
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.5f);
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.2f);
        }
    }
}
