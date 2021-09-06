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

    private bool editingCheckpoint = false;

    [SerializeField] private List<Transform> carTransformList;
    private List<Checkpoint> checkpointList = new List<Checkpoint>();
    private List<int> nextCheckpointIndexList = new List<int>();
    
    // Target track parent, when used with the spawner, this will be useful
    [SerializeField] Transform trackTarget;
    private void Start() {
        // Checkpoints should be added using AddCheckpoint
        //  Grab all the checkpoints from nested track pieces
        // foreach (Transform trackPiece in trackTarget) {
        //     Transform trackPieceCheckpoints = trackPiece.Find("Checkpoints");
        //     foreach (Transform checkpointTansform in trackPieceCheckpoints) {
        //         Checkpoint checkpoint = checkpointTansform.GetComponent<Checkpoint>();
        //         checkpoint.SetTrackCheckpoints(this);
        //         checkpointList.Add(checkpoint);
        //     }
        // }

        foreach (Transform carTransform in carTransformList) {
            nextCheckpointIndexList.Add(0);
        }
    }

    public void AddCheckpoints(Transform trackPiece) {
        // Debug.Log("ADD CHECKPOINT TRIGGERED", trackPiece);
        Transform trackPieceCheckpoints = trackPiece.Find("Checkpoints");
        foreach (Transform checkpointTansform in trackPieceCheckpoints) {
            Checkpoint checkpoint = checkpointTansform.GetComponent<Checkpoint>();
            checkpoint.SetTrackCheckpoints(this);
            checkpointList.Add(checkpoint);
        }
    }

    public void RemoveCheckpoints(Transform trackPiece) {
        // editingCheckpoint = true;
        // // Debug.Log("REMOVE CHECKPOINTS TRIGGERED", trackPiece);
        // Transform trackPieceCheckpoints = trackPiece.Find("Checkpoints");
        // if (trackPieceCheckpoints != null) {
        //     int numToRemove = trackPieceCheckpoints.childCount;

        //     nextCheckpointIndexList.ForEach(item => item -= numToRemove);

        //     // Remove checkpoints from the beginning of the list
        //     checkpointList.RemoveRange(0, numToRemove);
        //     editingCheckpoint = false;
        // }
    }

    public void ResetCheckpoints(Transform vehicleTransform) {
        nextCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)] = 0;
    }

    public void ResetAll() {
        nextCheckpointIndexList.Clear();
        checkpointList.Clear(); 
        foreach (Transform carTransform in carTransformList) {
            nextCheckpointIndexList.Add(0);
        }
        trackTarget.GetComponent<TrackSpawnerController>().ResetSpawner();
    }

    public int GetNumCheckpoints() {
        return checkpointList.Count;
    }

    // According to the checkpoint list and the entry in nextCheckpointIndexList
    // Get the NEXT checkpoint based on the passed in vehicleTransform
    public Checkpoint GetNextCheckpoint(Transform vehicleTransform) {
        if (checkpointList.Count > 0) {
            return checkpointList[nextCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)]];
        }
        return null;
    }

    public void CarThroughCheckpoint(Checkpoint checkpoint, Transform carTransform) {
        int carIdx = carTransformList.IndexOf(carTransform);
        int nextCheckpointIndex = nextCheckpointIndexList[carIdx];
       
        // Things here might need to change for non looping tracks
        
        if (checkpointList.IndexOf(checkpoint) == nextCheckpointIndex && nextCheckpointIndex >= 0) {
            Debug.Log("correct checkpoint");
            if (isLoopingTrack) {
                nextCheckpointIndexList[carIdx] = (nextCheckpointIndex + 1) % checkpointList.Count;
            } else {
                nextCheckpointIndexList[carIdx] = (nextCheckpointIndex + 1);
            }
            OnVehicleCorrectCheckpoint?.Invoke(this, new TrackCheckpointEventArgs { vehicleTransform = carTransform});
        } else {
            // wrong checkpoint
            Debug.Log("wrong checkpoint");
            OnVehicleWrongCheckpoint?.Invoke(this, new TrackCheckpointEventArgs { vehicleTransform = carTransform});
        }
    }
}
