using System;
using System.Collections;
using System.Linq;
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
    private List<int> lastHitCheckpoint = new List<int>();
    private List<int> tie = new List<int>();
    private int firstPlace = -1;

    private List<int> carPlacementList = new List<int>();
    
    // Target track parent, when used with the spawner, this will be useful
    public Transform trackTarget;
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
        GameObject[] botObjects = GameObject.FindGameObjectsWithTag("Car");
        foreach(GameObject bot in botObjects){
            carTransformList.Add(bot.transform);
        }
        var player = GameObject.FindWithTag("Player");
        if (player)
        {
            carTransformList.Add(player.transform);
        }
        

        Debug.Log("num of car transforms: " + carTransformList.Count);

        foreach (Transform carTransform in carTransformList) {
            nextCheckpointIndexList.Add(0);
            lastWrongCheckpointIndexList.Add(-1);
            lastHitCheckpoint.Add(0);
            carPlacementList.Add(0);
        }

        StartCoroutine(updatePlacements());
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
        lastHitCheckpoint[carTransformList.IndexOf(vehicleTransform)] = 0;
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
        Debug.Log("RESET ALL");
        nextCheckpointIndexList.Clear();
        lastWrongCheckpointIndexList.Clear();
        checkpointList.Clear(); 
        // Reset all cars
        foreach (Transform carTransform in carTransformList) {
            // Reset next checkpoint index
            nextCheckpointIndexList.Add(0);
            lastWrongCheckpointIndexList.Add(-1);
            lastHitCheckpoint.Add(0);
            carPlacementList.Add(0);

            // Reset position and momentum
            trackTarget.GetComponent<TrackSpawnerController>().ResetVehicles();
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

    public Checkpoint GetCheckpoint(int index) {
        return checkpointList[index];
    }

    public void CarThroughCheckpoint(Checkpoint checkpoint, Transform carTransform) {
        //Debug.Log("car through checkpoint");

        int carIdx = carTransformList.IndexOf(carTransform);
        int nextCheckpointIndex = nextCheckpointIndexList[carIdx];
        int targetCheckpointIndex = checkpointList.IndexOf(checkpoint);
        lastHitCheckpoint[findCarIndex(carTransform)] = targetCheckpointIndex;
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
        } else if (targetCheckpointIndex >= lastWrongCheckpointIndexList[carIdx] && lastWrongCheckpointIndexList[carIdx]
                   < nextCheckpointIndexList[carIdx] && lastWrongCheckpointIndexList[carIdx] != -1) {
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

    // Placement system ARIAN
    private List<PlacementObj> vehiclePlacementList = new List<PlacementObj>();
    public struct PlacementObj
    {
        public PlacementObj(int x, int y, float z)
        {
            carIDX = x;
            lastHitCheckpoint = y;
            distanceToNextCheckpoint = z;
        }

        public int carIDX;
        public int lastHitCheckpoint;
        public float distanceToNextCheckpoint;
    }

    public bool isFirstPlace(Transform transform)
    {
        var vehicleIndex = findCarIndex(transform);
        return vehiclePlacementList.Count > 0 ? vehiclePlacementList[0].carIDX == vehicleIndex : false;
    }

    IEnumerator updatePlacements()
    {
        // Placement object
        // [CarIDX | lastHitCheckpoint | distanceToNextCheckpoint]
        for(; ; )
        {
            var placementList = new List<PlacementObj>();
            for (int i = 0; i < carTransformList.Count; i++)
            {
                if (checkpointList.Count > 1)
                {
                    var carTransform = carTransformList[i];

                    // Get next checkpoint distance
                    var nextCheckpoint = checkpointList[nextCheckpointIndexList[i]];
                    var nextDistance = nextCheckpoint ? Vector3.Distance(nextCheckpoint.transform.position, carTransform.position) : 9999f;

                    var listItem = new PlacementObj(i, lastHitCheckpoint[i], nextDistance);
                    placementList.Add(listItem);
                    //Debug.Log("Car IDX: " + i + " LastHit: " + lastHitCheckpoint[i] + " Distance: " + nextDistance);
                }
            }

            // Sort placement list based on last hit checkpoint THEN checkpoint distance
            var orderedList = placementList.OrderByDescending(x => x.lastHitCheckpoint).ThenBy(x => x.distanceToNextCheckpoint);

            //Debug.Log("First Place " + orderedList.ToList()[0].carIDX);

            vehiclePlacementList = orderedList.ToList();
            
            yield return new WaitForSeconds(.1f);
        }
    }

    // End Placement System ARIAN

    public int findCarIndex(Transform transform) {
        return carTransformList.IndexOf(transform);
    }

    public Transform findCarTransform(int carIdx) {
        return carTransformList[carIdx];
    }

    public List<Transform> getCarTransforms() {
        return carTransformList;
    }

    public bool isFirst(Transform car) {
        int carIdx = findCarIndex(car);

        if(firstPlace == carIdx) {
            return true;
        } else {
            return isTieFirst(car);
        }
    }
    
    // is there a tie for first place?
    public bool isTieFirst(Transform car) {
        int carIdx = findCarIndex(car);
        if(tie.Count > 1) {
            foreach(int vehicle in tie) {
                if(carIdx == vehicle) {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Transform> getTie() {
        List<Transform> tieList = new List<Transform>();

        foreach(int carIdx in tie) {
            Transform carTransform = findCarTransform(carIdx);
            tieList.Add(carTransform);
        }
        return tieList;
    }

    // runs every update
    public void EvaluatePlaces() {
        int highestNextCheckpoint = -1; 
        int highestLastCheckpoint = -1;
        List<int> leaders = new List<int>();
        tie.Clear();
        firstPlace = -1;

        foreach(int nextCheckpoint in nextCheckpointIndexList) {
            int vehicleIndex = nextCheckpointIndexList.IndexOf(nextCheckpoint);
            int lastCheckpoint = lastHitCheckpoint[vehicleIndex];
            if(nextCheckpoint > highestNextCheckpoint && lastCheckpoint > highestLastCheckpoint) {
                highestNextCheckpoint = nextCheckpoint;
                highestLastCheckpoint = lastCheckpoint;
                leaders.Clear();
                leaders.Add(vehicleIndex);
                firstPlace = vehicleIndex;
            }
            else if(nextCheckpoint == highestNextCheckpoint && lastCheckpoint == highestLastCheckpoint) {
                leaders.Add(vehicleIndex);
            }
        }

        if(leaders.Count > 1) {
            float closestDist = -1f;
            foreach(int leaderCarIdx in leaders) {
                Transform car = carTransformList[leaderCarIdx];
                Transform checkpoint = checkpointList[highestNextCheckpoint].transform;
                float dist = Vector3.Distance(checkpoint.position , car.position);

                if(dist > closestDist) {
                    firstPlace = leaderCarIdx;
                    tie.Clear();
                    tie.Add(leaderCarIdx);
                }
                else if(dist == closestDist) {
                    //tie
                    tie.Add(leaderCarIdx);
                }
            }

            if(tie.Count > 1) {
                firstPlace = -1;
            }      
        }
    }
}
