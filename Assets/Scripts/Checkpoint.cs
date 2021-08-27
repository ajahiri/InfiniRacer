using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "VehicleBody") {
            trackCheckpoints.PlayerThroughCheckpoint(this);
        }
    }
    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }
}
