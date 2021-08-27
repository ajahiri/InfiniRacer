using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public bool isLoopingTrack;
    private List<Checkpoint> checkpointList = new List<Checkpoint>();
    private int nextCheckpointIndex;
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

        nextCheckpointIndex = 0;
    }

    public void PlayerThroughCheckpoint(Checkpoint checkpoint) {
        // Things here might need to change for non looping tracks
        
        if (checkpointList.IndexOf(checkpoint) == nextCheckpointIndex) {
            // corretn checkpoint
            Debug.Log("correct checkpoint");
            // Ensures when looping next index starts at beginning of track
            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpointList.Count;
        } else {
            // wrong checkpoint
            Debug.Log("wrong checkpoint");
        }
    }
}
