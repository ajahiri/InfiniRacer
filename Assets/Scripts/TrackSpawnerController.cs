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
            new TrackPieceDeltas(-6.4f, -0.2f, -4f, -6.4f, -0.2f, 4f, 0, "none", 0, "none", "straight", Resources.Load<Transform>("TrackPieces/Straight_Flat_Long")),
            new TrackPieceDeltas(-6.4f, -0.2f, 0f, -32f, -0.2f, 25.6f, 0, "none", 0, "none", "left", Resources.Load<Transform>("TrackPieces/Curve_Flat"))
        };
        return trackDeltaDefs;
    }

    void Start()
    {
        var deltaDefinitions = BuildDeltaDefinitions();

        // Origin game object where spawning starts
        var spawnerTransformOrigin = GetComponent<Transform>();

        // Instantiate the initial starting track piece where subsequent pieces will connect to
        Instantiate(deltaDefinitions[0].prefab, new Vector3(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z), Quaternion.identity);
        var firstPiece1 = new TrackPiece(spawnerTransformOrigin.position.x, spawnerTransformOrigin.position.y, spawnerTransformOrigin.position.z, 0, Quaternion.identity);
        trackPieceMemory.Push(firstPiece1);

        int[] test = { 1, 0, 0, 1 , 0, 0, 1, 0, 0, 1 };

        for (int i = 0; i < 6; i++)
        {
            var finalPiece = trackPieceMemory.Peek(); // Get last track piece

            //int rand = Random.Range(0, 2);
            //int rand = 1;
            int rand = test[i];
            var targetTrackDelta = deltaDefinitions[rand];

            // Last piece orientation augmentation
            // Determine the additional rotation to be added to new piece as a result of a turning piece
            // Adjust new piece positions based on last pos and end deltas
            // ADD END delta of final piece type to final piece pos and ADD START delta of new piece type aswell to get the correct origin coords for new piece
            float newXPos = finalPiece.xPos;
            float newYPos = finalPiece.yPos;
            float newZPos = finalPiece.zPos;
            
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

            Instantiate(Resources.Load("Cube"), new Vector3(
                        newXPos,
            newYPos,
            newZPos
                    ), Quaternion.identity);

            Debug.Log(newPieceRotation.eulerAngles.y);
            if (newPieceRotation.eulerAngles.y == 270)
            {
                if (orientationDirection == "left")
                {
                    newXPos += deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd + targetTrackDelta.zDeltaStart;
                    newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                    newZPos += deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd - targetTrackDelta.xDeltaStart;
                } else
                {
                    newXPos += -deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd + targetTrackDelta.zDeltaStart;
                    newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                    newZPos += deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd - targetTrackDelta.xDeltaStart;
                }
                
            } 
            else if(newPieceRotation.eulerAngles.y == 180)
            {
                if (orientationDirection == "left")
                {
                    newXPos += deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd - targetTrackDelta.zDeltaStart;
                    newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                    newZPos += -deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd + targetTrackDelta.xDeltaStart;
                } else
                {
                    Instantiate(Resources.Load("Cube 1"), new Vector3(
                        newXPos - deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd - targetTrackDelta.xDeltaStart,
                        newYPos + deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd,
                        newZPos + deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd - targetTrackDelta.zDeltaStart
                    ), Quaternion.identity);


                    newXPos += -deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd + targetTrackDelta.xDeltaStart;
                    newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                    newZPos += deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd - targetTrackDelta.zDeltaStart;
                    
                }
                
            } 
            else if (newPieceRotation.eulerAngles.y == 90)
            {
                if (orientationDirection == "left")
                {
                    newXPos += -deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd + targetTrackDelta.zDeltaStart;
                    newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                    newZPos += -deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd + targetTrackDelta.xDeltaStart;
                }
                else
                {
                    newXPos += -deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd + targetTrackDelta.zDeltaStart;
                    newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                    newZPos += -deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd + targetTrackDelta.xDeltaStart;
                }
            } 
            else if (newPieceRotation.eulerAngles.y == 0)
            {
                newXPos += deltaDefinitions[finalPiece.deltasIndex].xDeltaEnd - targetTrackDelta.xDeltaStart;
                newYPos += deltaDefinitions[finalPiece.deltasIndex].yDeltaEnd - targetTrackDelta.yDeltaStart;
                newZPos += deltaDefinitions[finalPiece.deltasIndex].zDeltaEnd - targetTrackDelta.zDeltaStart;
            }

            TrackPiece newPiece = new TrackPiece(newXPos, newYPos, newZPos, rand, newPieceRotation);

            // Add new piece to the list
            trackPieceMemory.Push(newPiece);

            // Instantiate the prefab
            Instantiate(targetTrackDelta.prefab, new Vector3(newXPos, newYPos, newZPos), newPieceRotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
