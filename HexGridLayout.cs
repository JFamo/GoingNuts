using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    // Singleton instance MainGrid
    public static HexGridLayout MainGrid {get; private set;}

    public static float sqThree = Mathf.Sqrt(3);

    private static Vector2Int gridSize;
    private static float outerSize = 1f;
    private static float innerSize = 0f;
    private static float height = 0f;
    private static bool isFlatTopped;
    private static Dictionary<Vector2Int, GameObject> hexTileMap;

    [Header("Tile Settings")]
    public Material material;

    void Awake(){
        if(MainGrid != null && MainGrid != this){
            Destroy(this);
        }
        else{
            MainGrid = this;
        }
    }

    public void SetupGrid(){
        // Load variables from game singleton
        GameController main = GameController.MainGame;
        gridSize = main.gridSize;
        outerSize = main.outerSize;
        innerSize = main.innerSize;
        height = main.height;
        isFlatTopped = main.isFlatTopped;

        // Initialize hex map
        hexTileMap = new Dictionary<Vector2Int, GameObject>();

        LayoutGrid();
    }

    private void LayoutGrid(){
        for(int y = 0; y < gridSize.y; y++){
            for(int x = 0; x < gridSize.x; x++){
                GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexRender));
                tile.transform.position = GetPositionForHexFromCoord(new Vector2Int(x,y));

                HexRender hexRender = tile.GetComponent<HexRender>();
                hexRender.isFlatTopped = isFlatTopped;
                hexRender.outerSize = outerSize;
                hexRender.innerSize = innerSize;
                hexRender.height = height;
                hexRender.SetMaterial(material);
                hexRender.DrawMesh();

                hexTileMap.Add(new Vector2Int(x,y), tile);

                tile.transform.SetParent(transform, true);
            }
        }
    }

    // Reset position to default material
    public void UpdateTileMaterial(Vector2Int pos){
        GameObject tileObj = hexTileMap[pos];
        if(tileObj != null){
            HexRender hexRender = tileObj.GetComponent<HexRender>();
            hexRender.SetMaterial(material);
        }
    }

    // Set position tile to specified material
    public void UpdateTileMaterial(Vector2Int pos, Material mat){
        GameObject tileObj = hexTileMap[pos];
        if(tileObj != null){
            HexRender hexRender = tileObj.GetComponent<HexRender>();
            hexRender.SetMaterial(mat);
        }
    }

    // Function to validate a given point as in the game grid
    public bool ValidatePointInGameGrid(Vector2Int point){
        if(point.x >= 0 && point.y >= 0){
            if(point.x < gridSize.x && point.y < gridSize.y){
                return true;
            }
        }
        return false;
    }

    // Function to generate successors to some position
    public List<Vector2Int> SuccessorsFromPosition(Vector2Int pos){
        if(GameController.MainGame.isFlatTopped){
            // TODO
            Debug.LogError("Attempt to use flat top hex positions");
            return null;
        }
        else{

            // Create list of adjacent positions
            List<Vector2Int> posns = new List<Vector2Int>();

            // Unique diagonals for even rows
            if(pos.y % 2 == 0){
                // Get each possible position
                if(pos.x > 0){
                    posns.Add(new Vector2Int(pos.x - 1, pos.y));
                }
                if(pos.x < gridSize.x - 1){
                    posns.Add(new Vector2Int(pos.x + 1, pos.y)); 
                }
                if(pos.y > 0){
                    posns.Add(new Vector2Int(pos.x, pos.y - 1));
                }
                if(pos.y > 0 && pos.x < gridSize.x - 1){
                    posns.Add(new Vector2Int(pos.x + 1, pos.y - 1));
                }
                if(pos.y < gridSize.y - 1){
                    posns.Add(new Vector2Int(pos.x, pos.y + 1));  
                }
                if(pos.x < gridSize.x - 1 && pos.y < gridSize.y - 1){
                    posns.Add(new Vector2Int(pos.x + 1, pos.y + 1));
                }
            }
            // Unique diagonals for odd rows
            else{
                // Get each possible position
                if(pos.x > 0){
                    posns.Add(new Vector2Int(pos.x - 1, pos.y));
                }
                if(pos.x < gridSize.x - 1){
                    posns.Add(new Vector2Int(pos.x + 1, pos.y)); 
                }
                if(pos.y > 0){
                    posns.Add(new Vector2Int(pos.x, pos.y - 1));
                }
                if(pos.y > 0 && pos.x > 0){
                    posns.Add(new Vector2Int(pos.x - 1, pos.y - 1));
                }
                if(pos.y < gridSize.y - 1){
                    posns.Add(new Vector2Int(pos.x, pos.y + 1));
                }
                if(pos.y < gridSize.y - 1 && pos.x > 0){
                    posns.Add(new Vector2Int(pos.x - 1, pos.y + 1));
                }
            }

            return posns;
        }
    }

    // Function to calculate bottom left successor from some position
    public static Vector2Int GetBottomLeftOfPosition(Vector2Int pos){
        if(GameController.MainGame.isFlatTopped){
            // TODO
            Debug.LogError("Attempt to use flat top hex positions");
            return new Vector2Int(0,0);
        }
        else{
            if(pos.y % 2 == 0){
                return new Vector2Int(pos.x, pos.y + 1); 
            }
            else{
                return new Vector2Int(pos.x - 1, pos.y + 1);
            }
        }
    }

    // Function to calculate bottom right successor from some position
    public static Vector2Int GetBottomRightOfPosition(Vector2Int pos){
        if(GameController.MainGame.isFlatTopped){
            // TODO
            Debug.LogError("Attempt to use flat top hex positions");
            return new Vector2Int(0,0);
        }
        else{
            if(pos.y % 2 == 0){
                return new Vector2Int(pos.x + 1, pos.y + 1); 
            }
            else{
                return new Vector2Int(pos.x, pos.y + 1);
            }
        }
    }

    public static Vector3 GetPositionForHexFromCoord(Vector2Int coord){
        int col = coord.x;
        int row = coord.y;

        float width, height, xPos, yPos, horDist, verDist, offset;
        float size = outerSize;
        bool shouldOffset;

        if(!isFlatTopped){
            shouldOffset = (row % 2) == 0;
            width = sqThree * size;
            height = 2f * size;

            horDist = width;
            verDist = height * (3f / 4f);

            offset = (shouldOffset) ? width/2 : 0;

            xPos = (col * horDist) + offset;
            yPos = (row * verDist);
        }
        else{
            shouldOffset = (col % 2) == 0;
            width = 2f * size;
            height = sqThree * size;

            horDist = width * (3f / 4f);
            verDist = height;

            offset = (shouldOffset) ? height/2 : 0;
            xPos = (col * horDist);
            yPos = (row * verDist) - offset;
        }

        return new Vector3(xPos, 0, -yPos);
    }

    public static Vector2Int GetCoordinateFromGamePosition(Vector3 pos){
        float size = outerSize;
        float col;
        float row;

        if(!isFlatTopped){
            // Handle top (positive) z row
            if(pos.z >= 0){
                col = (int)Mathf.Floor(pos.x / (size*sqThree));
                return new Vector2Int((int)col, 0);
            }

            // Calculate positive half-row
            float halfSize = size/2;
            int halfRow = (int)Mathf.Floor(Mathf.Abs(pos.z) / halfSize);

            // Determine type of offset
            bool isRightRow = (halfRow % 6) == 0 || (halfRow % 6 == 5);
            bool isLeftRow = (halfRow % 6) == 2 || (halfRow % 6 == 3);
            bool isIndeterminate = (halfRow % 6) == 1 || (halfRow % 6 == 4);

            // Handle definitive right row
            if(isRightRow){
                col = (int)Mathf.Floor(pos.x / (size*sqThree));
                if(halfRow % 6 == 0){
                    row = Mathf.Floor(halfRow / 3);
                }
                else{
                    row = Mathf.Floor(halfRow / 3) + 1;
                }
                return new Vector2Int((int)col, (int)row);
            }

            // Handle definitive left row
            if(isLeftRow){
                col = (int)Mathf.Floor((pos.x + (size*sqThree/2)) / (size*sqThree));
                if(halfRow % 6 == 3){
                    row = Mathf.Floor(halfRow / 3);
                }
                else{
                    row = Mathf.Floor(halfRow / 3) + 1;
                }
                return new Vector2Int((int)col, (int)row);
            }

            // Sort out intersecting row coords
            if(isIndeterminate){
                float alpha = sqThree * size / 2;
                float abz = Mathf.Abs(pos.z);
                float abx = Mathf.Abs(pos.x);
                float alphaDiff = Mathf.Floor(abx / alpha);
                float remainderX = abx - (alpha * alphaDiff);
                if(halfRow % 6 == 1){
                    if(alphaDiff % 2 == 0){
                        if(abz > (halfRow * halfSize) + (sqThree * remainderX)){
                            row = Mathf.Floor(halfRow / 3) + 1;
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        else{
                            row = Mathf.Floor(halfRow / 3);
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        return new Vector2Int((int)col, (int)row);
                    }
                    else{
                        if(abz > ((halfRow+1) * halfSize) - (sqThree * remainderX)){
                            row = Mathf.Floor(halfRow / 3) + 1;
                            col = Mathf.Floor(abx / (sqThree * size)) + 1;
                        }
                        else{
                            row = Mathf.Floor(halfRow / 3);
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        return new Vector2Int((int)col, (int)row);
                    }
                }
                else{
                    if(alphaDiff % 2 != 0){
                        if(abz > (halfRow * halfSize) + (sqThree * remainderX)){
                            row = Mathf.Floor(halfRow / 3) + 1;
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        else{
                            row = Mathf.Floor(halfRow / 3) + 1;
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        return new Vector2Int((int)col, (int)row);
                    }
                    else{
                        if(abz > ((halfRow+1) * halfSize) - (sqThree * remainderX)){
                            row = Mathf.Floor(halfRow / 3) + 1;
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        else{
                            row = Mathf.Floor(halfRow / 3);
                            col = Mathf.Floor(abx / (sqThree * size));
                        }
                        return new Vector2Int((int)col, (int)row);
                    }
                }
            }
            return new Vector2Int(0, 0);
        }
        else{
            Debug.LogError("Trying to calculate unsupported flat-topped coordinates!");
            return new Vector2Int(0,0);
        }
    }
}
