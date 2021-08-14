/*
 * Created 14-08-2021
 * Arian Jahiri 13348469
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSpawnerController : MonoBehaviour
{
    // Track Piece Prefabs, linked from Unity
    public Transform straightTrackLongPrefab;

    /* 
     * Prefab Deltas and Track Piece Properties:
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

    // Origin game object where spawning starts
    private Transform originObject;

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

        public TrackPieceDeltas(float xDeltaStart, float yDeltaStart, float zDeltaStart, float xDeltaEnd, float yDeltaEnd, float zDeltaEnd, int bankFactor, string bankDirection, int elevationFactor, string elevationDirection)
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
        }
    }

    /*
     * Note in unity, the x coords map to width, y to height and z to depth
     * Compared to Blender where z is height and y is depth
     */

    // Track Delta definitions, xyz deltas need to be measured for each piece
    private TrackPieceDeltas straightFlatLongDeltas = new TrackPieceDeltas(-6.4f, -0.2f, -4f, -6.4f, -0.2f, 4f, 0, "none", 0, "none");

    // TrackPiece struct for use in list to track piece positions
    private struct TrackPiece
    {
        // Positions of track piece transform (relative to unity universe)
        public float xPos;
        public float yPos;
        public float zPos;

        public TrackPiece(float xPos, float yPos, float zPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.zPos = zPos;
        }
    }

    /* 
     * List for track pieces, might be better to have some fixed size data type and 
     * only remember track pieces that are currently rendered in the view. As pieces
     * appear to the player (slide up from ground feature) we can push to a fixed stack.
     */
    private Stack<TrackPiece> trackPieceMemory = new Stack<TrackPiece>();

    void Start()
    {
        originObject = GetComponent<Transform>();

        // Instantiate the initial starting track piece where others will connect to
        Instantiate(straightTrackLongPrefab, new Vector3(originObject.position.x, originObject.position.y, originObject.position.z), Quaternion.identity);
        var firstPiece = new TrackPiece(originObject.position.x, originObject.position.y, originObject.position.z);
        trackPieceMemory.Push(firstPiece);

        for (int i = 0; i < 2; i++)
        {
            var finalPiece = trackPieceMemory.Peek(); // Get last track piece

            // Adjust new piece positions based on last pos and end deltas
            // ADD END delta of final piece type to final piece pos and ADD START delta of new piece type aswell to get the correct origin coords for new piece
            float newXPos = finalPiece.xPos + straightFlatLongDeltas.xDeltaEnd - straightFlatLongDeltas.xDeltaStart;
            float newYPos = finalPiece.yPos + straightFlatLongDeltas.yDeltaEnd - straightFlatLongDeltas.yDeltaStart;
            float newZPos = finalPiece.zPos + straightFlatLongDeltas.zDeltaEnd - straightFlatLongDeltas.zDeltaStart;

            TrackPiece newPiece = new TrackPiece(newXPos, newYPos, newZPos);

            // Add new piece to the list
            trackPieceMemory.Push(newPiece);

            // Instantiate the prefab
            Instantiate(straightTrackLongPrefab, new Vector3(newXPos, newYPos, newZPos), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
