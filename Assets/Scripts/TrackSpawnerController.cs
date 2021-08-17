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
    [SerializeField] GameObject playerVehicleObject;
    private struct TrackPieceDefinition
    {
        // Object Prefab (From resources directory)
        public Transform prefab;
        public int startPitch, endPitch, startElevation, endElevation;

        public string orientationDirection; // straight, left, right

        /// <summary>
        /// Pitch Vals => Left: -2, -1 | Right: 1, 2 | None: 0
        /// Elevation Vals => Down: -2, -1 | Up: 1, 2 | None: 0
        /// </summary>
        /// <returns>
        /// TrackPieceDefinition
        /// </returns>
        public TrackPieceDefinition (
                                    int startPitch, int endPitch, int startElevation, int endElevation, 
                                    string orientationDirection, Transform prefab
                                )
        {
            this.prefab = prefab;
            this.orientationDirection = orientationDirection;
            this.startPitch = startPitch; 
            this.endPitch = endPitch;
            this.startElevation = startElevation;
            this.endElevation = endElevation;
        }
    }

    /// <summary>
    /// Builds an array of all possible track deltas and loads the respective track piece prefab.
    /// </summary>
    /// <returns>
    /// Array of track piece deltas, see struct for shape.
    /// </returns>
    TrackPieceDefinition[][] LoadTrackPieceDefinitions() {
        // Build the piece definitions within a 2D array using Resourced.Load to load the prefab transforms
        
        // Tracks STARTING with pitch of 0 or elevation of 0
        TrackPieceDefinition[] flatPieces =  
        {
            new TrackPieceDefinition(   0, 0, 0, 0, "straight", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Straight_Flat_Long_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "left", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Flat_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Flat_Right_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Flat_Left_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Flat_Right_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "left", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Corner_Flat_Left_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "right", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Corner_Flat_Right_Parent")
                                ),
            new TrackPieceDefinition(   0, -1, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Left_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   0, -2, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Left_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   0, 1, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Right_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   0, 2, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Right_Pitch2_Parent")
                                ),
            // The following 2 rollers say pitch in the name but they just change elevation smoothly and remain flat start and end
            new TrackPieceDefinition(   0, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Roller_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Roller_Pitch2_Parent")
                                ),
        };

        // Tracks STARTING with pitch of 1
        TrackPieceDefinition[] pitch1Pieces = {
            new TrackPieceDefinition(   -1, -1, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Left_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   -1, -1, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Corner_Left_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   -1, -1, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Left_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   -1, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Left_Pitch1_Inverse_Parent")
                                ),
            new TrackPieceDefinition(   1, 1, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Right_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   1, 1, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Corner_Right_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   1, 1, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Right_Pitch1_Parent")
                                ),
            new TrackPieceDefinition(   1, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Right_Pitch1_Inverse_Parent")
                                ),
        };

        // Tracks STARTING with pitch of 2
        TrackPieceDefinition[] pitch2Pieces = {
            new TrackPieceDefinition(   -2, -2, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Left_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   -2, -2, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Left_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   -2, -2, 0, 0, "left",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Corner_Left_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   -2, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Left_Pitch2_Inverse_Parent")
                                ),
            new TrackPieceDefinition(   2, 2, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Bend_Right_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   2, 2, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Corner_Right_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   2, 2, 0, 0, "right",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Curve_Right_Pitch2_Parent")
                                ),
            new TrackPieceDefinition(   2, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Transition_Right_Pitch2_Inverse_Parent")
                                ),
        };

        // Tracks STARTING with elevation pitch 1
        TrackPieceDefinition[] elevation1Pieces = {
        };

        // Tracks STARTING with elevation pitch 2
        TrackPieceDefinition[] elevation2Pieces = {
        };

        // Pieces that should be spawned rarely (pretty much only loops)
        // These are weird pieces that could be hard to drive on so only spawn rarely
        TrackPieceDefinition[] rarePieces = {
            new TrackPieceDefinition(   0, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Loop_Left_Parent")
                                ),
            new TrackPieceDefinition(   0, 0, 0, 0, "straight",
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Loop_Right_Parent")
                                ),
        };

        TrackPieceDefinition[][] output = {flatPieces, pitch1Pieces, pitch2Pieces, elevation1Pieces, elevation2Pieces, rarePieces};
        return output;
    }

    // TrackPiece struct for use in list to track piece positions
    // Instantiations of track pieces need to have some of their properties accessible for spawning new pieces
    private struct TrackPieceObject
    {
        // Positions of track piece transform (relative to unity universe)
        public Vector3 position;
        public int pieceDefinitionX, pieceDefinitionY;
        public Quaternion orientation;
        public Transform targetObject;

        public TrackPieceObject(Vector3 position, int pieceDefinitionX, int pieceDefinitionY, Quaternion orientation, Transform targetObject)
        {
            this.position = position;
            this.pieceDefinitionX = pieceDefinitionX;
            this.pieceDefinitionY = pieceDefinitionY;
            this.orientation = orientation;
            this.targetObject = targetObject;
        }
    }

    // Important for use in both Start() and FixedUpdate()
    private TrackPieceDefinition[][] trackPieceDefinitions;
    private Vector3 latestTrackPoint;

    /* 
    * List for track pieces, might be better to have some fixed size data type and 
    * only remember track pieces that are currently rendered in the view.
    * Will utilise a stack for now.
    * EDIT: Moved to list since I need access to start and end.
    */    
    private List<TrackPieceObject> trackPieceMemory = new List<TrackPieceObject>();

    // Origin game object where spawning starts
    private Transform spawnerTransformOrigin;
    void Start()
    {
        // Get transform of spawner object
        spawnerTransformOrigin = GetComponent<Transform>();

        // Load track definitions
        trackPieceDefinitions = LoadTrackPieceDefinitions();

        // Instantiate the initial starting track piece where subsequent pieces will connect to
        var currentObj = Instantiate(trackPieceDefinitions[0][0].prefab, new Vector3(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z), Quaternion.identity, spawnerTransformOrigin);
        
        // Get second order child of object (this contains an object of which transform is at "bottom_left_end" of piece)
        // TransformPoint(...) will get us this transform in the global coordinate context
        latestTrackPoint = currentObj.transform.GetChild(0).GetChild(0).transform.TransformPoint(0, 0, 0);
        trackPieceMemory.Add(new TrackPieceObject(spawnerTransformOrigin.position, 0, 0, Quaternion.identity, currentObj));
    }

    void TrackSpawner() {
        var finalPiece = trackPieceMemory[trackPieceMemory.Count - 1]; // Get last track piece
        var firstPiece = trackPieceMemory[0]; 

        // Track cleaner
        if (trackPieceMemory.Count > 5) {
            Destroy(firstPiece.targetObject.gameObject);
            trackPieceMemory.RemoveAt(0);
        }

        // Raycast Collision detector 
        bool constrainRight = trackPieceDefinitions[finalPiece.pieceDefinitionX][finalPiece.pieceDefinitionY].orientationDirection == "left";
        bool constrainFront = Physics.Raycast(latestTrackPoint, finalPiece.targetObject.transform.TransformDirection(Vector3.forward));

        var finalPieceDefinition = trackPieceDefinitions[finalPiece.pieceDefinitionX][finalPiece.pieceDefinitionY];

        int indexNewPieceX, indexNewPieceY;

        // Basic last couple piece check (avoids loops that collide)


        // Bias to straight piece
        if (Random.Range(1,5) % 2 == 0 && finalPieceDefinition.endPitch == 0) {
            indexNewPieceX = 0;
            indexNewPieceY = 0;
        } else {
            // Selecting for correct pitch
            switch (finalPieceDefinition.endPitch)
            {
                case 0:
                    // Can select any piece starting with 0 pitch
                    indexNewPieceX = 0;
                    indexNewPieceY = constrainRight ? 0 : (constrainFront ? 5 : Random.Range(0, trackPieceDefinitions[0].Length));
                    break;
                case -1:
                    // Can select any piece starting with -1 pitch
                    indexNewPieceX = 1;
                    indexNewPieceY = constrainRight ? 3 : (constrainFront ? 1 : Random.Range(0, 4));
                    break;
                case -2:
                    // Can select any piece starting with -2 pitch
                    indexNewPieceX = 2;
                    indexNewPieceY = constrainRight ? 3 : (constrainFront ? 1 : Random.Range(0, 4));
                    break;
                case 1:
                    // Can select any piece starting with 1 pitch
                    indexNewPieceX = 1;
                    indexNewPieceY = constrainRight ? 8 : Random.Range(4, 8);
                    break;
                case 2:
                    // Can select any piece starting with 2 pitch
                    indexNewPieceX = 2;
                    indexNewPieceY = constrainRight ? 8 : Random.Range(4, 8);
                    break;
                default:
                    return; // Invalid data, continue to next iteration
            };
        }

        // The target prefab for our new piece
        TrackPieceDefinition newPieceDefinition = trackPieceDefinitions[indexNewPieceX][indexNewPieceY];

        // Determine the additional rotation to be added to new piece as a result of a turning piece            
        // Rotations are compound based on last piece's orientation
        Quaternion newPieceRotation = finalPiece.orientation;

        switch (finalPieceDefinition.orientationDirection)
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
                return; // Some invalid data, kill this iteration
        }

        // Instantiate the prefab
        var latestObject = Instantiate(newPieceDefinition.prefab, latestTrackPoint, newPieceRotation, spawnerTransformOrigin); // Spawn as child of spawner

        // Position transform to last known "latestTrackPoint" after orientation has changed
        // Add new piece to the list
        trackPieceMemory.Add(new TrackPieceObject(latestTrackPoint, indexNewPieceX, indexNewPieceY, newPieceRotation, latestObject));

        latestTrackPoint = latestObject.transform.GetChild(0).GetChild(0).transform.TransformPoint(0, 0, 0); // Update latest point (where next track piece will spawn)  
    }

    void FixedUpdate()
    {
        // Conditional spawning interation loop
        if (Vector3.Distance(latestTrackPoint, playerVehicleObject.transform.position) < 20) {
            TrackSpawner();
        }
    }
}
