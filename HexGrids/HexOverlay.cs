using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexOverlay : MonoBehaviour
{
    // Singleton instance MainOverlay
    public static HexOverlay MainOverlay {get; private set;}

    // Hex grid definitions (pulled from main game)
    private static float sqThree = Mathf.Sqrt(3);
    private static Vector2Int gridSize;
    private static float outerSize = 1f;
    private static float height = 0f;
    private static bool isFlatTopped;
    private static Dictionary<Vector2Int, GameObject> hexTileMap;

    [Header("Overlay Materials")]
    public Material defaultMaterial;
    public int shadeRowLength;
    public Material[] overlayMaterials;

    void Awake(){
        // Setup singleton
        if(MainOverlay != null && MainOverlay != this){
            Destroy(this);
        }
        else{
            MainOverlay = this;
        }
    }

    public void SetupOverlay(){
        // Load variables from game singleton
        GameController main = GameController.MainGame;
        gridSize = main.gridSize;
        outerSize = main.outerSize;
        height = main.height;
        isFlatTopped = main.isFlatTopped;

        // Initialize hex map
        hexTileMap = new Dictionary<Vector2Int, GameObject>();

        LayoutGrid();
    }

    private void LayoutGrid(){
        for(int y = 0; y < gridSize.y; y++){
            for(int x = 0; x < gridSize.x; x++){
                GameObject tile = new GameObject($"HexOverlay {x},{y}", typeof(HexRender));
                tile.transform.position = HexGridLayout.GetPositionForHexFromCoord(new Vector2Int(x,y));

                HexRender hexRender = tile.GetComponent<HexRender>();
                hexRender.isFlatTopped = isFlatTopped;
                hexRender.outerSize = outerSize;
                hexRender.innerSize = 0.0f;
                hexRender.height = height;
                hexRender.SetMaterial(defaultMaterial);
                hexRender.DrawMesh();

                hexTileMap.Add(new Vector2Int(x,y), tile);

                tile.transform.SetParent(transform, true);
            }
        }
    }

    // Function to calculate material based on shade and water
    private Material CalculateOverlayFromProperties(TileProperties props){
        int shadesCount = shadeRowLength;
        int waterCount = overlayMaterials.Length / shadeRowLength;

        int shadesDimension = (int)Mathf.Floor(props.shade * (shadesCount - 1));
        int waterDimension = (int)Mathf.Floor(props.water * (waterCount - 1));

        return overlayMaterials[shadesDimension + (waterDimension * shadeRowLength)];
    }

    // Update tile overlay based on shade and water
    public void UpdateOverlayAtPosition(Vector2Int pos, TileProperties props){
        GameObject tileObj = hexTileMap[pos];
        if(tileObj != null){
            HexRender hexRender = tileObj.GetComponent<HexRender>();
            hexRender.SetMaterial(CalculateOverlayFromProperties(props));
            hexRender.DrawMesh();
        }
    }

}
