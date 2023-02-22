using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionOverlay : MonoBehaviour
{
    // Singleton instance MainSelection
    public static SelectionOverlay MainSelection {get; private set;}

    // Hex grid definitions (pulled from main game)
    private static float sqThree = Mathf.Sqrt(3);
    private static Vector2Int gridSize;
    private static float outerSize = 1f;
    private static float innerSize = 0f;
    private static float height = 0f;
    private static bool isFlatTopped;
    private int radius;
    private List<GameObject> hexTiles;

    [Header("Selection Materials")]
    public Material hoverMaterial;

    void Awake(){
        // Setup singleton
        if(MainSelection != null && MainSelection != this){
            Destroy(this);
        }
        else{
            MainSelection = this;
        }
    }

    public void SetupSelectionOverlay(){
        // Load variables from game singleton
        GameController main = GameController.MainGame;
        gridSize = main.gridSize;
        outerSize = main.outerSize;
        innerSize = main.innerSize;
        height = main.height;
        isFlatTopped = main.isFlatTopped;

        // Initialize hex list
        hexTiles = new List<GameObject>();
    }

    public void SelectPointWithRadius(Vector2Int posn, int radius){
        this.radius = radius;
        MoveSelectionToPoint(posn);
    }

    public void ClearSelection(){
        foreach(GameObject obj in hexTiles){
            Destroy(obj);
        }
        hexTiles.Clear();
    }

    private void CreateHexAtPosition(Vector2Int posn, int[] faces){
        GameObject tile = new GameObject($"SelectionOverlayHex {posn.x},{posn.y}", typeof(HexRender));
        tile.transform.position = HexGridLayout.GetPositionForHexFromCoord(posn);

        HexRender hexRender = tile.GetComponent<HexRender>();
        hexRender.isFlatTopped = isFlatTopped;
        hexRender.outerSize = outerSize;
        hexRender.innerSize = innerSize;
        hexRender.height = height;
        hexRender.SetMaterial(hoverMaterial);
        hexRender.DrawMesh(faces);

        hexTiles.Add(tile);

        tile.transform.SetParent(transform, true);
    }

    private void DrawSelectionWithRadiusAtPoint(Vector2Int center){
        foreach(HexWithFaces hexFaces in HexShapeBuilder.GetRadialShapeAroundPoint(center, this.radius)){
            CreateHexAtPosition(hexFaces.position, hexFaces.faces);
        }
    }

    private void MoveSelectionToPoint(Vector2Int posn){
        ClearSelection();
        DrawSelectionWithRadiusAtPoint(posn);
    }

}
