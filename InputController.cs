using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public Material hoverOutlineMaterial;
    public float openActionThreshold = 1.5f;

    bool hasSelection;
    private List<BoardObject> selectedObjects;
    private TileProperties selectedTile;
    private int selectionRadius;
    private float rmbTimer;

    void Start(){
        hasSelection = false;
        selectionRadius = 1;
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
        selectedTile = null;
        GUIController.MainGUI.ClearTileSelection();
        SelectionOverlay.MainSelection.ClearSelection();
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

    private void OutlineCursorTile(){
        // Calculate where cursor is pointing and validate it
        Vector2Int cursorPosition = CalculateMousePosition();
        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){

            // Compute selected tile
            selectedTile = GameController.MainGame.GetPositionTile(cursorPosition);

            // Update GUI with selections
            GUIController.MainGUI.ShowSelectedTile(selectedTile);

            // Trigger selection overlay
            SelectionOverlay.MainSelection.SelectPointWithRadius(cursorPosition,selectionRadius);

        }
        else{
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

    private void OpenActionDialog(){
        Vector2Int cursorPosition = CalculateMousePosition();

        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){

            foreach(BoardObject thisObject in selectedObjects){

                if(thisObject.GetProperty("movable") >= 1.0f){
                    List<BoardObject> targetObjects = CopyBoardObjectList(GameController.MainGame.GetPositionObjects(cursorPosition));
                    TileProperties targetTile = GameController.MainGame.GetPositionTile(cursorPosition);
                    List<string> possibleActions = ActionManager.GetPossibleActions(thisObject, targetObjects, targetTile);
                    ActionGUIController.MainActionGUI.CreateActionUIAtPosition(cursorPosition, possibleActions);
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
            if(selectionRadius < 6){
                selectionRadius++;
            }
            else{
                selectionRadius = 1;
            }
            ClearObjectSelection();
            OutlineCursorTile();
        }
        // LMB click
        if(Input.GetMouseButtonDown(0)){
            SelectCursorObjects();
        }
        // RMB click
        if(Input.GetMouseButtonDown(1)){
            rmbTimer = Time.time;
        }
        // RMB held
        if(Input.GetMouseButton(1)){
            if(Time.time - rmbTimer >= openActionThreshold){
                OpenActionDialog();
            }
        }
        // RMB release
        if(Input.GetMouseButtonUp(1)){
            if(Time.time - rmbTimer < openActionThreshold){
                IssueMoveOrder();
            }
            else{
                // TODO do action
            }
        }
    }
}
