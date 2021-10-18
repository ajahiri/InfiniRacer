/*
 * Created 14-08-2021
 * Arian Jahiri 13348469
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
* New Explanation (Changed due to architecture of this track spawner changing)
* Each track piece prefab is a child of some parent of which its origin is aligned
* to the bottom left most vertex that defines the start of the track (Orientation is important, check prefabs).
* Then another empty object that is a child of the prefab itself, (2nd order object) defines the bottom_left
* most vertice that defines the END of the track piece.
*
* With this information and more meta data about the start and end pitch/elevation, we can effectively
* spawn any of the 30+ track pieces while making sure they fit together as intended.
*
* Making the spawning procedural is harder, some extra steps are taken to add and remove track pieces from the
* game as the player drives the vehicle. Furthermore, the addition of track pieces must be constrained in a way
* that makes sure the track doesn't run into itself and essentially break the game for the player.
*
* Note:
* Start and end points were not eyeballed based on the unity prefab editor, vertices were instead measured in terms
* of there distance from the origin using Blender in order to precisely offset the track pieces.
*/

public class TrackSpawnerController : MonoBehaviour
{
    public List<GameObject> vehicleObjects;
    [SerializeField] GameObject checkpointHandlerObject;
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
    /// ID1
    /// </summary>
    /// <returns>
    /// Array of track piece deltas, see struct for shape.
    /// </returns>
    TrackPieceDefinition[][] LoadTrackPieceDefinitions() {
        // Build the piece definitions within a 2D array using Resourced.Load to load the prefab transforms
        
        // Tracks STARTING with pitch of 0 or elevation of 0
        TrackPieceDefinition[] flatPieces =  
        {
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
            new TrackPieceDefinition(   0, 0, 0, 0, "straight", 
                                    Resources.Load<Transform>("TrackPieces/Adjusted_Heirarchy_Pieces/Straight_Flat_Long_Parent")
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

    // Distances, required for conditional spawning/deletion
    float minDistanceToEnd = Mathf.Infinity;
    float minDistanceToStart = Mathf.Infinity;

    // Origin game object where spawning starts
    private Transform spawnerTransformOrigin;
    [SerializeField] public int numBotsToLoad = 1;
    private List<Vector3> spawnPositions = new List<Vector3>(); 

    public bool isTraining = false;

    void Start()
    {
        // Load bots
        saveBotNum BotNum = GameObject.FindObjectOfType<saveBotNum>();
        if (BotNum != null)
        {
            numBotsToLoad = BotNum.botNum;
        }
        LoadBots(numBotsToLoad);

        if(isTraining) {    // set camera to first bot
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.SetParent(vehicleObjects[0].transform);
        }

        // Get transform of spawner object
        var player = GameObject.FindWithTag("Player");
        if (player)
        {
            vehicleObjects.Add(player);
        }
        spawnerTransformOrigin = GetComponent<Transform>();

        // Load track definitions
        trackPieceDefinitions = LoadTrackPieceDefinitions();

        InitialiseTrack();
    }
    public void LoadBots(int numToLoad) {
        float startXPos = 9.625f;
        float spacing = 2.6875f;
        float xPos = startXPos;
        int itt = 0;
        while(itt < numToLoad) {
            GameObject BotPrefab = Resources.Load<GameObject>("Vehicle Bot"); //move this out of while loop
            if(itt > 0 && itt <= 2){
                if(itt % 2 == 0) {
                    xPos = startXPos + spacing;
                } else {
                    xPos = startXPos - spacing;
                }
            } else if (itt > 0 && itt > 2) {
                if(itt % 2 == 0){
                    xPos = startXPos + (2 * spacing);
                } else {
                    xPos = startXPos - (2 * spacing);
                }
            }
            Vector3 spawnPos = new Vector3(xPos, 0.08440538f, 50f);
            GameObject newVehicle = Instantiate(BotPrefab, spawnPos, Quaternion.identity); //init new vehcile
            spawnPositions.Add(spawnPos);
            vehicleObjects.Add(newVehicle);
            itt++;
        }
    }

    public void ResetVehicles() {
        for(int idx = 0; idx < vehicleObjects.Count; idx++) {
            GameObject bot = vehicleObjects[idx];
            bot.transform.SetPositionAndRotation(spawnPositions[idx], Quaternion.Euler(0,0,0));
            bot.GetComponent<Rigidbody>().velocity = Vector3.zero;
            bot.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    void InitialiseTrack() {
        // Instantiate the initial starting track piece where subsequent pieces will connect to
        var currentObj = Instantiate(trackPieceDefinitions[0][6].prefab, new Vector3(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z), Quaternion.identity, spawnerTransformOrigin);
        
        // Get second order child of object (this contains an object of which transform is at "bottom_left_end" of piece)
        // TransformPoint(...) will get us this transform in the global coordinate context
        latestTrackPoint = currentObj.transform.GetChild(0).GetChild(0).transform.TransformPoint(0, 0, 0);
        trackPieceMemory.Add(new TrackPieceObject(spawnerTransformOrigin.position, 0, 6, Quaternion.identity, currentObj));

        checkpointHandlerObject.GetComponent<TrackCheckpoints>().AddCheckpoints(currentObj);
    }

    // Checks last 5 placed track pieces for number of turns
    int CheckNumTurns() {
        int numTurns = 0;
        if (trackPieceMemory.Count < 6) {
            return 99;
        };
        for (int i = trackPieceMemory.Count - 1; i >= 0; i--)
        {
            var piece = trackPieceMemory[i];
            var definition = trackPieceDefinitions[piece.pieceDefinitionX][piece.pieceDefinitionY];
            if (definition.orientationDirection.Equals("left")  || definition.orientationDirection.Equals("right")) {
                numTurns += 1;
            }
        }
        return numTurns;
    }

    public void ResetSpawner() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        trackPieceMemory.Clear();
        InitialiseTrack();
    }

    void TrackSpawner(int numTurns) {
        var finalPiece = trackPieceMemory[trackPieceMemory.Count - 1]; // Get last track piece
        var firstPiece = trackPieceMemory[0]; 

        // Track cleaner
        // if (trackPieceMemory.Count > 12) {
        if (minDistanceToStart > 50f) {
            // Update checkpoint handler on track piece removal
            checkpointHandlerObject.GetComponent<TrackCheckpoints>().RemoveCheckpoints(firstPiece.targetObject);
            Destroy(firstPiece.targetObject.gameObject);
            trackPieceMemory.RemoveAt(0);
        }
        
        // To avoid invalid track placement, if too many turns spawned, restrict to straight
        // Only checks last 5 pieces
        bool restrictTurnPiece = numTurns >= 2;

        var finalPieceDefinition = trackPieceDefinitions[finalPiece.pieceDefinitionX][finalPiece.pieceDefinitionY];

        int indexNewPieceX, indexNewPieceY;

        // Bias to straight piece
        if (Random.Range(0,3) % 2 == 0 && finalPieceDefinition.endPitch == 0) {
            indexNewPieceX = 0;
            indexNewPieceY = 6;
        } else {
            // Selecting for correct pitch
            switch (finalPieceDefinition.endPitch)
            {
                case 0:
                    // Can select any piece starting with 0 pitch unless restricted to straight
                    indexNewPieceX = 0;
                    indexNewPieceY = restrictTurnPiece ? Random.Range(6,13) : Random.Range(0,13);
                    break;
                case -1:
                    // Can select any piece starting with -1 pitch unless restricted to straight
                    indexNewPieceX = 1;
                    indexNewPieceY = restrictTurnPiece ? 3 : Random.Range(0, 4);
                    break;
                case -2:
                    // Can select any piece starting with -2 pitch unless restricted to straight
                    indexNewPieceX = 2;
                    indexNewPieceY = restrictTurnPiece ? 3 : Random.Range(0, 4);
                    break;
                case 1:
                    // Can select any piece starting with 1 pitch unless restricted to straight
                    indexNewPieceX = 1;
                    indexNewPieceY = restrictTurnPiece ? 7 : Random.Range(4, 8);
                    break;
                case 2:
                    // Can select any piece starting with 2 pitch unless restricted to straight
                    indexNewPieceX = 2;
                    indexNewPieceY = restrictTurnPiece ? 7 : Random.Range(4, 8);
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

        checkpointHandlerObject.GetComponent<TrackCheckpoints>().AddCheckpoints(latestObject); // Append checkpoints relating to this piece

        // Position transform to last known "latestTrackPoint" after orientation has changed
        // Add new piece to the list
        trackPieceMemory.Add(new TrackPieceObject(latestTrackPoint, indexNewPieceX, indexNewPieceY, newPieceRotation, latestObject));

        latestTrackPoint = latestObject.transform.GetChild(0).GetChild(0).transform.TransformPoint(0, 0, 0); // Update latest point (where next track piece will spawn)  
    }

    public Checkpoint getMiddleCheckpoint() {
        int middleTrackIndex = trackPieceMemory.Count/2;
        // Get target piece close to middle with some randomness to avoid case where multiple vehicle will spawn in same place
        Transform targetTrack = trackPieceMemory[middleTrackIndex + Random.Range(- 1, trackPieceMemory.Count > 2 ? + 2 : 1)].targetObject;
        Transform trackPieceCheckpoints = targetTrack.Find("Checkpoints"); // Get checkpoints from track piece
        return trackPieceCheckpoints.GetChild(0).GetComponent<Checkpoint>(); // Return first checkpoint of track piece, at least 1 will always exist
    }

    void FixedUpdate()
    {
        minDistanceToStart = Mathf.Infinity;
        minDistanceToEnd = Mathf.Infinity;
        // Calculate min.max distances
        for (int i = 0; i < vehicleObjects.Count; i++)
        {
            GameObject currentObj = vehicleObjects[i];
            float vehicleDistanceToEnd = Vector3.Distance(latestTrackPoint, currentObj.transform.position); // Distance to last track piece
            float vehicleDistanceFromStart = Vector3.Distance(trackPieceMemory[0].position, currentObj.transform.position); // Distance to first track piece
            if (vehicleDistanceToEnd < minDistanceToEnd) {
                minDistanceToEnd = vehicleDistanceToEnd;
            }
            if (vehicleDistanceFromStart < minDistanceToStart) {
                minDistanceToStart = vehicleDistanceFromStart;
            }
        }

        // Conditional spawning interation loop
        if (minDistanceToEnd < 50) {
            // Debug.Log("in distance");
            var numTurns = CheckNumTurns();
            TrackSpawner(numTurns);
        }
    }
}
