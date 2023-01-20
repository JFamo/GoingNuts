using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public Material hoverOutlineMaterial;

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

    private void ClearTileSelection(){
        RemoveSelectedTileOutline();
        selectedTile = null;
        GUIController.MainGUI.ClearTileSelection();
    }   

    private void ClearObjectSelection(){

        // Deselect
        hasSelection = false;
        selectedObjects.Clear();
        
        // Clear GUI
        GUIController.MainGUI.ClearObjectSelection();
    }

    private List<BoardObject> CopyBoardObjectList(List<BoardObject> input){
        List<BoardObject> outputList = new List<BoardObject>();
        foreach(BoardObject thisObject in input){
            outputList.Add(thisObject);
        }
        return outputList;
    }

    private void RemoveSelectedTileOutline(){
        // Remove highlighting from previous tile
        if(selectedTile != null){
            selectedTile.DequeueOutlineMaterial(hoverOutlineMaterial);
        }
    }

    private void OutlineCursorTile(){
        // Calculate where cursor is pointing and validate it
        Vector2Int cursorPosition = CalculateMousePosition();
        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){

            RemoveSelectedTileOutline();
            
            selectedTile = GameController.MainGame.GetPositionTile(cursorPosition);

            // Update GUI with selections
            GUIController.MainGUI.ShowSelectedTile(selectedTile);

            // Apply highlighting
            selectedTile.EnqueueOutlineMaterial(hoverOutlineMaterial);

        }
        else{
            // On failure, clear selection
            ClearTileSelection();
        }
    }

    private void SelectCursorObjects(){

        // Calculate where cursor is pointing and validate it
        Vector2Int cursorPosition = CalculateMousePosition();
        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){
            
            // Select tile from controller
            hasSelection = true;
            // Pass copy of object so we don't lose references on move
            selectedObjects = CopyBoardObjectList(GameController.MainGame.GetPositionObjects(cursorPosition));

            // Update GUI with selections
            GUIController.MainGUI.ShowSelections(selectedObjects);

        }
        else{
            // On failure, clear selection
            ClearObjectSelection();
        }
    }

    private void IssueMoveOrder(){
        Vector2Int cursorPosition = CalculateMousePosition();

        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){
            
            foreach(BoardObject thisObject in selectedObjects){

                if(thisObject.GetProperty("movable") >= 1.0f){
                    thisObject.GetComponent<MovableObject>().MoveToPoint(cursorPosition);
                }
            }

        }
    }

    public void Update(){
        // Mouse Move
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0){
            OutlineCursorTile();
        }
        // Spacebar
        if(Input.GetKeyDown("space")){
            ClearObjectSelection();
        }
        // LMB click
        if(Input.GetMouseButtonDown(0)){
            SelectCursorObjects();
        }
        // RMB click
        if(Input.GetMouseButtonDown(1)){
            IssueMoveOrder();
        }
    }
}
