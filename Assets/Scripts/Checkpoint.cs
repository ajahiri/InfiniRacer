using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VehicleBody")
        {
            Transform parentTransform = other.transform.parent;
            //Debug.Log(parentTransform);
            trackCheckpoints.CarThroughCheckpoint(this, parentTransform);
        }
    }
    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }

    private void Update()
    {
        // Debug mode render the "forward" direction of the checkpoint
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 4;
        Debug.DrawRay(transform.position, forward, Color.green);
    }
}
