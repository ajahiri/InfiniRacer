using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarDriverAgent : Agent 
{
    [SerializeField] private Transform targetTransform;
    //[SerializeField] private Material winMaterial;
    //[SerializeField] private Material loseMaterial;
    //[SerializeField] private MeshRenderer floorMeshRenderer;

    public override void OnEpisodeBegin() {
        transform.localPosition = new Vector3(Random.Range(+10f, -11f), +4f, -14f);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0,0,0);
        targetTransform.localPosition = new Vector3(Random.Range(-12f, +9.5f), +4.5f, Random.Range(+12f, +0f));
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        BotCarController targetScript = gameObject.GetComponent<BotCarController>();
        targetScript.SetInputs(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2] == 0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = 0f;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Checkpoint>(out Checkpoint goal)) {
            SetReward(+1f);
            //floorMeshRenderer.material = winMaterial;
        }
        if (other.TryGetComponent<Wall>(out Wall wall)) {
            SetReward(-1f);
            //floorMeshRenderer.material = loseMaterial;
        }
    }
}
