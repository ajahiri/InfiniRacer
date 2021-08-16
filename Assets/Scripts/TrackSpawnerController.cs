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
    // Delta Definitions, these don't change and are dependent on the 3D model of the piece
    private struct TrackPieceDeltas
    {
        // Size deltas (for determining placement of next piece)
        public int bankFactor; // 0, 1, 2
        public string bankDirection; // none, left, right
        public int elevationFactor; // 0, 1, 2
        public string elevationDirection; // none, up, down
        public string orientationDirection; // straight, left, right
        public Transform prefab;

        public TrackPieceDeltas (
                                    int bankFactor, string bankDirection, int elevationFactor, string elevationDirection, 
                                    string orientationDirection, Transform prefab
                                )
        {
            this.bankFactor = bankFactor;
            this.bankDirection = bankDirection;
            this.elevationFactor = elevationFactor;
            this.elevationDirection = elevationDirection;
            this.orientationDirection = orientationDirection;
            this.prefab = prefab;
        }
    }

    // TrackPiece struct for use in list to track piece positions
    // Instantiations of track pieces need to have some of their properties accessible for spawning new pieces
    private struct TrackPiece
    {
        // Positions of track piece transform (relative to unity universe)
        public Vector3 position;
        public int deltasIndex; // Index in deltas array that corresponds to the correct track piece delta
        public Quaternion orientation;

        public TrackPiece(Vector3 position, int deltasIndex, Quaternion orientation)
        {
            this.position = position;
            this.deltasIndex = deltasIndex;
            this.orientation = orientation;
        }
    }

    /* 
    * List for track pieces, might be better to have some fixed size data type and 
    * only remember track pieces that are currently rendered in the view.
    * Will utilise a stack for now.
    */
    private Stack<TrackPiece> trackPieceMemory = new Stack<TrackPiece>();

    /// <summary>
    /// Builds an array of all possible track deltas and loads the respective track piece prefab.
    /// </summary>
    /// <returns>
    /// Array of track piece deltas, see struct for shape.
    /// </returns>
    TrackPieceDeltas[] BuildDeltaDefinitions() {
        // Build the delta definitions within an array using Resourced.Load to load the prefab transforms
        // Track Delta definitions, xyz deltas need to be measured for each piece
        TrackPieceDeltas[] trackDeltaDefs =  
        {
            new TrackPieceDeltas(   0, "none", 0, "none", "straight", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Straight_Flat_Long_Parent")
                                ),
            new TrackPieceDeltas(   0, "none", 0, "none", "left", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Flat_Parent")
                                ),
            new TrackPieceDeltas(   0, "none", 0, "none", "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Flat_Right_Parent")
                                ),
            new TrackPieceDeltas(   0, "none", 0, "none", "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Flat_Left_Parent")
                                ),
            new TrackPieceDeltas(   0, "none", 0, "none", "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Flat_Right_Parent")
                                ),
        };
        return trackDeltaDefs;
    }

    void Start()
    {
        var deltaDefinitions = BuildDeltaDefinitions();

        // Origin game object where spawning starts
        var spawnerTransformOrigin = GetComponent<Transform>();

        // Instantiate the initial starting track piece where subsequent pieces will connect to
        var currentObj = Instantiate(deltaDefinitions[0].prefab, new Vector3(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z), Quaternion.identity, spawnerTransformOrigin);
        
        // Get second order child of object (this contains an object of which transform is at "bottom_left_end" of piece)
        // TransformPoint(...) will get us this transform in the global coordinate context
        var latestTrackPoint = currentObj.transform.GetChild(0).GetChild(0).transform.TransformPoint(0, 0, 0);

        var firstPiece1 = new TrackPiece(spawnerTransformOrigin.position, 0, Quaternion.identity);
        trackPieceMemory.Push(firstPiece1);

        for (int i = 0; i < 12; i++)
        {
            var finalPiece = trackPieceMemory.Peek(); // Get last track piece

            int rand = Random.Range(0, deltaDefinitions.Length); // Tweak for conditionals
            //int rand = 1;
            //int rand = test[i];
            var targetTrackDelta = deltaDefinitions[rand];

            // Determine the additional rotation to be added to new piece as a result of a turning piece            
            // Rotations are compound based on last piece's orientation
            Quaternion newPieceRotation = finalPiece.orientation;
            var orientationDirection = deltaDefinitions[finalPiece.deltasIndex].orientationDirection;

            switch (orientationDirection)
            {
                case "left":
                    newPieceRotation *= Quaternion.Euler(0, -90, 0);
                    break;
                case "right":
                    newPieceRotation *= Quaternion.Euler(0, 90, 0);
                    break;
                case "straight":
                    break;
                default:
                    break;
            }

            // Position transform to last known "latestTrackPoint" after orientation has changed
            TrackPiece newPiece = new TrackPiece(latestTrackPoint, rand, newPieceRotation);

            // Add new piece to the list
            trackPieceMemory.Push(newPiece);

            // Instantiate the prefab
            var latestObject = Instantiate(targetTrackDelta.prefab, latestTrackPoint, newPieceRotation, spawnerTransformOrigin);
            latestTrackPoint = latestObject.transform.GetChild(0).GetChild(0).transform.TransformPoint(0, 0, 0); // Update latest point (where next track piece will spawn)
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
