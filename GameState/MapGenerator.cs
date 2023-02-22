using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Game Object Prefabs")]
    public GameObject playerMover;
    public GameObject river;
    public GameObject bridge;
    public GameObject[] foilages;
    public GameObject[] solids;

    private int GetAdjacentValueCount(int [,] grid, int x, int y, int value){
        int count = 0;

        List<Vector2Int> successors = HexGridLayout.MainGrid.SuccessorsFromPosition(new Vector2Int(x,y));
        foreach(Vector2Int successorPosn in successors){
            if(grid[successorPosn.x, successorPosn.y] == value){
                count += 1;
            }
        }

        return count;
    }

    private int GetAdjacentRiverCount(int [,] grid, int x, int y){
        return GetAdjacentValueCount(grid, x, y, 2) + GetAdjacentValueCount(grid, x, y, 3);
    }

    private void UpdateTilePropertiesAtPosition(int x, int y, float shade, float water){
        TileProperties thisTileProperties = GameController.MainGame.GetPositionTile(new Vector2Int(x,y));
        thisTileProperties.SetShade(shade);
        thisTileProperties.SetWater(water);
    }

    private int ComputeGridAtPosition(int [,] grid, int x, int y){
        float shade = 0.0f; 
        float water = 0.0f;
        int rivers = GetAdjacentRiverCount(grid, x, y);
        float chance = Random.Range(0.0f, 0.9999f);
        int output = 0;
    
        if(rivers == 1){
            if(chance < 0.7f){
                output = 2; // River
                shade = 0.0f;
                water = 1.0f;
            }
            else if(chance < 0.8f){
                output = 3; // Bridge
                shade = 1.0f;
                water = 1.0f;
            }
            else{
                output = 0; // Nothing
                shade = 0.0f;
                water = 0.8f;
            }
        }
        else{
            if(chance < 0.005f){
                output = 2; // River
                shade = 0.0f;
                water = 1.0f;
            }
            else if(chance < 0.3f){
                output = 4; // Foilage
                shade = 0.2f;
                water = 0.2f;
            }
            else if(chance < 0.4f){
                output = 5; // Solid
                shade = 0.5f;
                water = 0.2f;
            }
            else{
                output = 0; // Nothing
                shade = 0.0f;
                if(chance < 0.7f){
                    water = 0.4f;
                }
                else if(chance < 0.8f){
                    water = 0.5f;
                }
                else if(chance < 0.9f){
                    water = 0.6f;
                }
                else{
                    water = 0.3f;
                }
            }
        }

        if(rivers > 0 && water < 0.8f){
            water = 0.8f;
        }

        UpdateTilePropertiesAtPosition(x,y,shade,water);

        return output;
    }

    private int[,] PopulateGenerativeGrid(Vector2Int gridSize){
        // Setup board generation 2d array
        int[,] boardSetup = new int[gridSize.x, gridSize.y];

        boardSetup[gridSize.x / 2, gridSize.y / 2] = 1; // Put player at middle
        boardSetup[gridSize.x / 2 + 1, gridSize.y / 2] = 1; // Put player at middle
        boardSetup[gridSize.x / 2 + 3, gridSize.y / 2] = 1; // Put player at middle

        for (int x = 0; x < gridSize.x; x ++){
            for(int y = 0; y < gridSize.y; y ++){
                if(boardSetup[x,y] != 1){
                    boardSetup[x,y] = ComputeGridAtPosition(boardSetup, x, y);
                }
            }
        }

        return boardSetup;
    }

    private GameObject RandomObjectFromArray(GameObject[] arr){
        int index = (int)Mathf.Floor(Random.Range(0.0f, 0.999f) * arr.Length);
        return arr[index];
    }

    private GameObject GetGameObjectFromGridCode(int code){
        if(code == 1){
            return playerMover;
        }
        if(code == 2){
            return river;
        }
        if(code == 3){
            return bridge;
        }
        if(code == 4){
            return RandomObjectFromArray(foilages);
        }
        if(code == 5){
            return RandomObjectFromArray(solids);
        }
        return null;
    }

    private void InstantiateGameFromGenerativeGrid(int[,] grid){
        for(int x = 0; x < grid.GetLength(0); x ++){
            for(int y = 0; y < grid.GetLength(1); y ++){

                if(grid[x,y] > 0){
                    GameObject thisObject = GetGameObjectFromGridCode(grid[x,y]);
                    GameObject newObject = Instantiate(thisObject);
                    newObject.GetComponent<BoardObject>().UpdatePosition(x,y);
                }
            }
        }
    }

    public void InstantiateMap()
    {
        int[,] generativeGrid = PopulateGenerativeGrid(GameController.MainGame.gridSize);
        InstantiateGameFromGenerativeGrid(generativeGrid);
    }
}
