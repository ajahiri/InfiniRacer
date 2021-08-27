using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    // Events for crrect/wrong checkpoints with event args class to hold vehicle transform
    public event EventHandler<TrackCheckpointEventArgs> OnVehicleCorrectCheckpoint;
    public event EventHandler<TrackCheckpointEventArgs> OnVehicleWrongCheckpoint;

    public class TrackCheckpointEventArgs : EventArgs {
        public Transform vehicleTransform;
    }
    public bool isLoopingTrack;

    [SerializeField] private List<Transform> carTransformList;
    private List<Checkpoint> checkpointList = new List<Checkpoint>();
    private List<int> nextCheckpointIndexList = new List<int>();
    
    // Target track parent, when used with the spawner, this will be useful
    [SerializeField] Transform trackTarget;
    private void Awake() {
        //  Grab all the checkpoints from nested track pieces
        foreach (Transform trackPiece in trackTarget) {
            Transform trackPieceCheckpoints = trackPiece.Find("Checkpoints");
            foreach (Transform checkpointTansform in trackPieceCheckpoints) {
                Checkpoint checkpoint = checkpointTansform.GetComponent<Checkpoint>();
                checkpoint.SetTrackCheckpoints(this);
                checkpointList.Add(checkpoint);
            }
        }

        foreach (Transform carTransform in carTransformList) {
            nextCheckpointIndexList.Add(0);
        }
    }

    public void ResetCheckpoints(Transform vehicleTransform) {
        nextCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)] = 0;
    }

    // According to the checkpoint list and the entry in nextCheckpointIndexList
    // Get the NEXT checkpoint based on the passed in vehicleTransform
    public Checkpoint GetNextCheckpoint(Transform vehicleTransform) {
        return checkpointList[nextCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)]];
    }

    public void CarThroughCheckpoint(Checkpoint checkpoint, Transform carTransform) {
        int nextCheckpointIndex = nextCheckpointIndexList[carTransformList.IndexOf(carTransform)];
       
        // Things here might need to change for non looping tracks
        
        if (checkpointList.IndexOf(checkpoint) == nextCheckpointIndex) {
            //Debug.Log("correct checkpoint");
            nextCheckpointIndexList[carTransformList.IndexOf(carTransform)] = (nextCheckpointIndex + 1) % checkpointList.Count;
            OnVehicleCorrectCheckpoint?.Invoke(this, new TrackCheckpointEventArgs { vehicleTransform = carTransform});
        } else {
            // wrong checkpoint
            //Debug.Log("wrong checkpoint");
            OnVehicleWrongCheckpoint?.Invoke(this, new TrackCheckpointEventArgs { vehicleTransform = carTransform});
        }
    }
}
