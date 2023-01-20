using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // Singleton instance MainGame
    public static GameController MainGame {get; private set;}

    [Header("Gameboard Properties")]
    public Vector2Int gridSize;
    public float outerSize = 1f;
    public float innerSize = 0f;
    public float height = 0f;
    public bool isFlatTopped;

    [Header("Game State Objects")]
    public Dictionary<Vector2Int, List<BoardObject>> positionObjectsMap;
    public Dictionary<Vector2Int, TileProperties> positionTilesMap;

    // Activate singleton
    private void Awake(){
        if(MainGame != null && MainGame != this){
            Destroy(this);
        }
        else{
            MainGame = this;
            InitializePositionObjectsMap();
            InitializePositionTilesMap();
        }
    }

    private void Start(){
        // Initialize hex grid and overlay
        HexGridLayout.MainGrid.SetupGrid();
        HexOverlay.MainOverlay.SetupOverlay();

        // Initialize tile properties and map
        UpdateAllTileProperties();
        gameObject.GetComponent<MapGenerator>().InstantiateMap();
    }

    // Function to return each of the objects in a state
    public List<BoardObject> GetPositionObjects(Vector2Int posn){
        return positionObjectsMap[posn];
    }

    // Getter for tile properties at point
    public TileProperties GetPositionTile(Vector2Int posn){
        return positionTilesMap[posn];
    }

    // Function to move object
    public void MoveObject(BoardObject thisObject, Vector2Int oldPos, Vector2Int newPos){
        // Remove from old position
        positionObjectsMap[oldPos].Remove(thisObject);
        // Add to new position
        positionObjectsMap[newPos].Add(thisObject);
        // DEBUG
        Debug.Log("Moved " + thisObject.GetString("name") + ". Now " + positionObjectsMap[oldPos].Count + " objects at " + oldPos.x + ", " + oldPos.y + " and " + positionObjectsMap[newPos].Count + " objects at " + newPos.x + ", " + newPos.y);
        // Update tile material queues
        if(!(thisObject.hexTileMaterial is null)){
            positionTilesMap[oldPos].DequeueOutlineMaterial(thisObject.hexTileMaterial);
            positionTilesMap[newPos].EnqueueOutlineMaterial(thisObject.hexTileMaterial);
        }
        // Update tile properties from move
        UpdatePositionTileProperties(oldPos);
        UpdatePositionTileProperties(newPos);
    }

    // Function to update tile properties at some position (so that we only recompute on move)
    private void UpdatePositionTileProperties(Vector2Int posn){
        positionTilesMap[posn].UpdateProperties();
    }

    // Update each tile property
    private void UpdateAllTileProperties(){
        for(int x = 0; x < gridSize.x; x ++){
            for(int y = 0; y < gridSize.y; y ++){
                positionTilesMap[new Vector2Int(x,y)].UpdateProperties();
            }
        }
    }

    // Store list of objects at each game board position
    private void InitializePositionObjectsMap(){
        positionObjectsMap = new Dictionary<Vector2Int, List<BoardObject>>();

        for(int x = 0; x < gridSize.x; x ++){
            for(int y = 0; y < gridSize.y; y ++){
                Vector2Int thisPosn = new Vector2Int(x,y);
                positionObjectsMap[thisPosn] = new List<BoardObject>();
            }
        }
    }

    // Store list of tile properties for each game position
    private void InitializePositionTilesMap(){
        positionTilesMap = new Dictionary<Vector2Int, TileProperties>();

        for(int x = 0; x < gridSize.x; x ++){
            for(int y = 0; y < gridSize.y; y ++){
                Vector2Int thisPosn = new Vector2Int(x,y);
                positionTilesMap[thisPosn] = new TileProperties(thisPosn);
            }
        }
    }

}
