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

    public List<Transform> carTransformList;
    private List<Checkpoint> checkpointList = new List<Checkpoint>();
    private List<int> nextCheckpointIndexList = new List<int>();
    private List<int> lastWrongCheckpointIndexList = new List<int>();

    // Target track parent, when used with the spawner, this will be useful
    private Transform trackTarget;
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
        carTransformList.Add(GameObject.FindGameObjectWithTag("Player").transform);
        trackTarget = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (Transform carTransform in carTransformList) {
            nextCheckpointIndexList.Add(0);
            lastWrongCheckpointIndexList.Add(-1);
        }
    }

    private void Awake() {
        // ResetAll();
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
        // NOT USED (here for reference) was not efficient to update indexes
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
        // Soft reset refers to the resetting of 
        nextCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)] = 0;
        lastWrongCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)] = -1;
    }

    public void softResetToCheckpoint(Transform vehicleTransform) {
        if (checkpointList.Count < 4) {
            // Safety as should not run on initial episode start
            return;
        }

        // Get a checkpoint of the middle spawned track piece
        Checkpoint targetCheckpoint = trackTarget.GetComponent<TrackSpawnerController>().getMiddleCheckpoint();



        // Spawn vehicle to correct position according to middle checkpoint |targetCheckpoint.transform.forward|
        vehicleTransform.localRotation = targetCheckpoint.transform.rotation;
        vehicleTransform.localPosition = new Vector3(   
                                                        targetCheckpoint.transform.position.x,
                                                        targetCheckpoint.transform.position.y + UnityEngine.Random.Range(3, 8), // safety to ensure vehicle doesn't spawn inside track
                                                        targetCheckpoint.transform.position.z
                                                    );

        // Set correct checkpoints indexes (note, correct checkpoint is next checkpoint ahead of where we spawned)
        nextCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)] = checkpointList.IndexOf(targetCheckpoint) + 1;
        lastWrongCheckpointIndexList[carTransformList.IndexOf(vehicleTransform)] = -1;
        // Reset momentum
        vehicleTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        vehicleTransform.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void ResetAll() {
        nextCheckpointIndexList.Clear();
        lastWrongCheckpointIndexList.Clear();
        checkpointList.Clear(); 
        // Reset all cars
        foreach (Transform carTransform in carTransformList) {
            // Reset next checkpoint index
            nextCheckpointIndexList.Add(0);
            lastWrongCheckpointIndexList.Add(-1);

            // Reset position
            float carSpacing = 3 * findCarIndex(carTransform);
            float x_axis = carSpacing + 16.2995396f;
            carTransform.localRotation = Quaternion.Euler(0,0,0);
            carTransform.localPosition = new Vector3(x_axis,0.119178146f,1.78931403f);

            // Reset momentum
            carTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            carTransform.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
        int targetCheckpointIndex = checkpointList.IndexOf(checkpoint);
        // Things here might need to change for non looping tracks
        
        if (targetCheckpointIndex == nextCheckpointIndex && nextCheckpointIndex >= 0) {
            // Debug.Log("correct checkpoint");
            if (isLoopingTrack) {
                // special case for looping tracks
                nextCheckpointIndexList[carIdx] = (nextCheckpointIndex + 1) % checkpointList.Count;
            } else {
                nextCheckpointIndexList[carIdx] = (nextCheckpointIndex + 1);
            }
            lastWrongCheckpointIndexList[carIdx] = -1; // reset last wrong
            OnVehicleCorrectCheckpoint?.Invoke(this, new TrackCheckpointEventArgs { vehicleTransform = carTransform});
        } else if (targetCheckpointIndex >= lastWrongCheckpointIndexList[carIdx] && lastWrongCheckpointIndexList[carIdx] < nextCheckpointIndexList[carIdx] && lastWrongCheckpointIndexList[carIdx] != -1) {
            // special case, if car is a few checkpoints behind, dont give neg reward for going in the right direction but 
            // technically going through the wrong checkpoint. treat it as a null case
            // Debug.Log("null checkpoint");
            // Do nothing... or might make this a neg reward but a lot smaller than standard wrong checkpoint
        } else {
            // wrong checkpoint
            // Debug.Log("wrong checkpoint");
            lastWrongCheckpointIndexList[carIdx] = targetCheckpointIndex; // Update last wrong checkpoint index for this car
            OnVehicleWrongCheckpoint?.Invoke(this, new TrackCheckpointEventArgs { vehicleTransform = carTransform});
        }
    }

    public int findCarIndex(Transform transform) {
        return carTransformList.IndexOf(transform);
    }

    public List<Transform> getCarTransforms() {
        return carTransformList;
    }
}
