using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToBallAgent : Agent 
{
    [SerializeField] private Transform targetTransform;

    private Vector3 StartPosition = new Vector3(10.43f, 0.71f, -12.05f);

    public override void OnEpisodeBegin()
    {
        transform.position = StartPosition;
    }
    // WHAT DOES THIS DO!!!!
    // public override void CollectObservations(VectorSensor sensor)
    // {
    //     sensor.AddObservation(transform.position);
    //     sensor.AddObservation(targetTransform.position);
    // }
    public override void OnActionReceived(ActionBuffers actions) {
        GetComponent<CarController>().SetInputs(actions.ContinuousActions[0], 0f, actions.DiscreteActions[0] != 0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuouseActions = actionsOut.ContinuousActions;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Ball>(out Ball ball)) {
            SetReward(+1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            SetReward(-1f);
            EndEpisode();
        }
    }
}
