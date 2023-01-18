using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    bool hasSelection;
    private List<BoardObject> selectedObjects;
    private TileProperties selectedTile;

    void Start(){
        hasSelection = false;
        selectedObjects = new List<BoardObject>();
    }

    private Vector2Int CalculateMousePosition(){
        Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit myHit;

        if(Physics.Raycast(myRay, out myHit)){
            Vector2Int hexComputed = HexGridLayout.GetCoordinateFromGamePosition(myHit.point);
            return hexComputed;
        }
        return new Vector2Int(-1, -1); // Trigger validation error on no hit
    }

    private void ClearSelection(){

        // DEBUG
        Debug.Log("Processing clear selections");

        // Deselect
        hasSelection = false;
        selectedObjects.Clear();
        selectedTile = null;

        // Clear GUI
        GUIController.MainGUI.ClearSelections();
    }

    private List<BoardObject> CopyBoardObjectList(List<BoardObject> input){
        List<BoardObject> outputList = new List<BoardObject>();
        foreach(BoardObject thisObject in input){
            outputList.Add(thisObject);
        }
        return outputList;
    }

    private void SelectCursorTile(){

        // DEBUG
        Debug.Log("Processing select tile");

        // Calculate where cursor is pointing and validate it
        Vector2Int cursorPosition = CalculateMousePosition();
        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){
            
            // Select tile from controller
            hasSelection = true;
            selectedTile = GameController.MainGame.GetPositionTile(cursorPosition);
            // Pass copy of object so we don't lose references on move
            selectedObjects = CopyBoardObjectList(GameController.MainGame.GetPositionObjects(cursorPosition));

            // Update GUI with selections
            GUIController.MainGUI.ShowSelections(selectedObjects, selectedTile);

        }
        else{
            // On failure, clear selection
            ClearSelection();
        }
    }

    private void IssueMoveOrder(){
        Vector2Int cursorPosition = CalculateMousePosition();

        // DEBUG
        Debug.Log("Attempting move to " + cursorPosition.x + ", " + cursorPosition.y + " on " + selectedObjects.Count + " objects");

        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){
            
            foreach(BoardObject thisObject in selectedObjects){

                // DEBUG
                Debug.Log("Issuing move to " + cursorPosition.x + ", " + cursorPosition.y + " for " + thisObject.strings["name"]);

                if(thisObject.properties["movable"] >= 1.0f){
                    thisObject.GetComponent<MovableObject>().MoveToPoint(cursorPosition);
                }
            }

            // DEBUG
            Debug.Log("After loop got " + selectedObjects.Count + " objects");

        }
    }

    public void Update(){
        // Spacebar
        if(Input.GetKeyDown("space")){
            ClearSelection();
        }
        // LMB click
        if(Input.GetMouseButtonDown(0)){
            SelectCursorTile();
        }
        // RMB click
        if(Input.GetMouseButtonDown(1)){
            IssueMoveOrder();
        }
    }
}
