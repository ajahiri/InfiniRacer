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

    private void Awake() {
        botCarController = GetComponent<BotCarController>();
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
        trackCheckpoints.ResetAll();
        // transform.localPosition = new Vector3(UnityEngine.Random.Range(5f, 15f), +0f, 2f);
        transform.localPosition = new Vector3(16.2999992f,0.289999992f,1.78999996f);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0,0,0);
    }

    public override void CollectObservations(VectorSensor sensor) {
        // In addition to the Ray Perception Sensor, this observation will make sure the model learns to face the same direction as the checkpoints forward
        // This ensures that it learns to keep itself pointing in the right direction
        //Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        if (trackCheckpoints.GetNumCheckpoints() > 0) {
            var nextCheckpoint = trackCheckpoints.GetNextCheckpoint(transform);
            if (nextCheckpoint != null) {
                Vector3 checkpointForward = nextCheckpoint.transform.TransformDirection(Vector3.forward);
        
                float directionDot = Vector3.Dot(transform.forward, checkpointForward);
                sensor.AddObservation(directionDot);
            }
        }
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


    // Penalise the agent if it slides/hits the boundary walls of the track
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "End Barrier") {
            AddReward(-3f);
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
