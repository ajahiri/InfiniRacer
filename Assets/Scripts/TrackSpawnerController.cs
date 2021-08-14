/*
 * Created 14-08-2021
 * Arian Jahiri 13348469
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
* Prefab Deltas and Track Piece Properties: (How this spawner works)
* Deltas are used to know how much (in units) the track changes from start to finish.
* Spawning of track pieces are done by knowing the coordinate of the bottom left most vertex of the model.
* This applies to both start and finish coordinates. Due to the models we are using having their local origin
* in the centre of the model, we need to know the xyz deltas (in units) for both start and finish of the model.
* With this we can appropriately spawn track pieces that fit together properly.
* 
* NOTE:
* Some pieces have more special properties such as banking factor, bank direction, elevation factor and the elevation direction
* (this is not the same as Quaternion direction as pieces may be pointing in different directions).
* 
* Furthermore, xyz deltas give us the appropriate positioning for the track pieces, rotation on the zx plane is
* also needed for when left or right turn pieces are used as the subsequent pieces will need to be pointing in the
* correct direction. Again this is not the same as banking factor and banking direction.
* 
* Rules are setup for which track pieces can mate to others, pitches and elevation factors need to match in order for pieces
* to transition smoothly. Unfortunately, the asset pack only provides left turns so model changes (or some smart scripting can avoid this). TODO
*/

public class TrackSpawnerController : MonoBehaviour
{
    // Track Piece Prefabs, linked from Unity --> Old
    // Using Resources.Load() instead so prefabs are known (required change as these prefabs will be referenced in the TrackPieceDeltas struct
    // Doing that is not possible with a serliazed field because the prefab is not loaded until serialized causing (
    //[SerializeReference] private Transform straightFlatLongPrefab;
    //[SerializeReference] private Transform cornerFlatPrefab;

    // Origin game object where spawning starts
    private Transform spawnerTransformOrigin;

    // Delta Definitions, these don't change and are dependent on the 3D model of the piece
    private struct TrackPieceDeltas
    {
        // Size deltas (for determining placement of next piece)
        public float xDeltaStart;
        public float yDeltaStart;
        public float zDeltaStart;
        public float xDeltaEnd;
        public float yDeltaEnd;
        public float zDeltaEnd;
        public int bankFactor; // 0, 1, 2
        public string bankDirection; // none, left, right
        public int elevationFactor; // 0, 1, 2
        public string elevationDirection; // none, up, down
        public string orientationDirection; // straight, left, right
        public Transform prefab;

        public TrackPieceDeltas (
                                    float xDeltaStart, float yDeltaStart, float zDeltaStart, float xDeltaEnd, float yDeltaEnd, 
                                    float zDeltaEnd, int bankFactor, string bankDirection, int elevationFactor, string elevationDirection, 
                                    string orientationDirection, Transform prefab
                                )
        {
            this.xDeltaStart = xDeltaStart;
            this.yDeltaStart = yDeltaStart;
            this.zDeltaStart = zDeltaStart;
            this.xDeltaEnd = xDeltaEnd;
            this.yDeltaEnd = yDeltaEnd;
            this.zDeltaEnd = zDeltaEnd;
            this.bankFactor = bankFactor;
            this.bankDirection = bankDirection;
            this.elevationFactor = elevationFactor;
            this.elevationDirection = elevationDirection;
            this.orientationDirection = orientationDirection;
            this.prefab = prefab;
        }
    }

    // Track Delta definitions, xyz deltas need to be measured for each piece
    //private TrackPieceDeltas straightFlatLongDeltas = ;
    //private TrackPieceDeltas cornerFlatDeltas = ;

    // TrackPiece struct for use in list to track piece positions
    // Instantiations of track pieces need to have some of their properties accessible for spawning new pieces
    private struct TrackPiece
    {
        // Positions of track piece transform (relative to unity universe)
        public float xPos;
        public float yPos;
        public float zPos;
        public int deltasIndex; // Index in deltas array that corresponds to the correct track piece delta
        public Quaternion orientation;

        public TrackPiece(float xPos, float yPos, float zPos, int deltasIndex, Quaternion orientation)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.zPos = zPos;
            this.deltasIndex = deltasIndex;
            this.orientation = orientation;
        }
    }

    /// <summary>
    /// Builds an array of all possible track deltas and loads the respective track piece prefab.
    /// </summary>
    /// <returns>
    /// Array of track piece deltas, see struct for shape.
    /// </returns>
    TrackPieceDeltas[] BuildDeltaDefinitions() {
        // Build the delta definitions within an array using Resourced.Load to load the prefab transforms
        TrackPieceDeltas[] trackDeltaDefs =  
        {
            new TrackPieceDeltas(-6.4f, -0.2f, -4f, -6.4f, -0.2f, 4f, 0, "none", 0, "none", "straight", Resources.Load<Transform>("TrackPieces/Straight_Flat_Long")),
            new TrackPieceDeltas(-6.4f, -0.2f, 0f, -32f, -0.2f, 25.6f, 0, "none", 0, "none", "left", Resources.Load<Transform>("TrackPieces/Curve_Flat"))
        };
        return trackDeltaDefs;
    }

    void Start()
    {
        var deltaDefinitions = BuildDeltaDefinitions();
        /* 
         * List for track pieces, might be better to have some fixed size data type and 
         * only remember track pieces that are currently rendered in the view.
         */
        var trackPieceMemory = new Stack<TrackPiece>();

        spawnerTransformOrigin = GetComponent<Transform>();

        // Instantiate the initial starting track piece where others will connect to
        Instantiate(deltaDefinitions[0].prefab, new Vector3(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z), Quaternion.identity);
        var firstPiece = new TrackPiece(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z, 0, Quaternion.identity);
        trackPieceMemory.Push(firstPiece);

        for (int i = 0; i < 1; i++)
        {
            var finalPiece = trackPieceMemory.Peek(); // Get last track piece

            //int rand = Random.Range(0, 1);
            int rand = 1;
            var targetTrackDelta = deltaDefinitions[rand];

            // Adjust new piece positions based on last pos and end deltas
            // ADD END delta of final piece type to final piece pos and ADD START delta of new piece type aswell to get the correct origin coords for new piece
            float newXPos = finalPiece.xPos + deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd - targetTrackDelta.xDeltaStart;
            float newYPos = finalPiece.yPos + deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
            float newZPos = finalPiece.zPos + deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd - targetTrackDelta.zDeltaStart;

            TrackPiece newPiece = new TrackPiece(newXPos, newYPos, newZPos, rand, Quaternion.identity);

            // Add new piece to the list
            trackPieceMemory.Push(newPiece);

            // Instantiate the prefab
            Instantiate(targetTrackDelta.prefab, new Vector3(newXPos, newYPos, newZPos), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
