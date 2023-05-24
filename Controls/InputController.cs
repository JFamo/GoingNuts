using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public Material hoverOutlineMaterial;
    public float openActionThreshold = 1.5f;

    private List<BoardObject> selectedObjects;
    private TileProperties selectedTile;
    private List<BoardObject> targetObjects;
    private TileProperties targetTile;
    private int selectionRadius;
    private float rmbTimer;
    private bool actionDialogOpen;
    private bool growToggle;

    private PlacementPhantomController phantomController;

    void Start(){
        selectionRadius = 1;
        actionDialogOpen = false;
        selectedObjects = new List<BoardObject>();
        targetObjects = new List<BoardObject>();
        growToggle = false;

        phantomController = GetComponent<PlacementPhantomController>();
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
        selectedObjects.Clear();
        
        // Clear GUI
        GUIController.MainGUI.ClearObjectSelection();
    }

    private List<BoardObject> CopyBoardObjectList(List<BoardObject> input){
        return new List<BoardObject>(input);
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

    private void OpenActionDialog(){
        Vector2Int cursorPosition = CalculateMousePosition();

        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){

            List<ExecutorAction> possibleActions = new List<ExecutorAction>();

            foreach(BoardObject thisObject in selectedObjects){

                if(thisObject.GetProperty("movable") >= 1.0f){
                    targetObjects = CopyBoardObjectList(GameController.MainGame.GetPositionObjects(cursorPosition));
                    targetTile = GameController.MainGame.GetPositionTile(cursorPosition);
                    foreach(ExecutorAction action in ActionManager.GetPossibleActions(thisObject, targetObjects, targetTile))
                    {
                        possibleActions.Add(action);
                    }
                }

            }

            // If possible actions, create GUI
            if(possibleActions.Count > 0) {
                actionDialogOpen = true;
                ActionGUIController.MainActionGUI.CreateActionUIAtPosition(cursorPosition, possibleActions, Input.mousePosition);
            }

        }
    }

    // Function to submit a default action, when we select and release RMB on any square to an executor
    private void SubmitDefaultAction(){
        ExecutorAction action;
        actionDialogOpen = false;

        // Get targeted tile and its objects
        Vector2Int cursorPosition = CalculateMousePosition();
        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){
            targetObjects = CopyBoardObjectList(GameController.MainGame.GetPositionObjects(cursorPosition));
            targetTile = GameController.MainGame.GetPositionTile(cursorPosition);

            // Look for executors of action among selected
            foreach(BoardObject boardObject in selectedObjects.ToList())
            {
                if(boardObject.GetProperty("executor") > 0f)
                {
                    action = ActionManager.GetDefaultAction(boardObject, targetObjects, targetTile);
                    
                    if(action != ExecutorAction.NONE){
                        boardObject.GetComponent<ExecutorObject>().handleAction(action, targetTile.tilePosition);
                    }
                }
            }
        }
    }

    // Function to submit the selected action via mouseover from an open action dialog to an executor
    private void SubmitActionDialog(){
        ExecutorAction action = ActionGUIController.MainActionGUI.SubmitAction(Input.mousePosition);
        actionDialogOpen = false;

        // Look for executors of action among selected
        foreach(BoardObject boardObject in selectedObjects.ToList())
        {
            if(boardObject.GetProperty("executor") > 0f)
            {
                boardObject.GetComponent<ExecutorObject>().handleAction(action, targetTile.tilePosition);
            }
        }
    }

    private void SubmitGrowAction(){
        List<Vector2Int> posns = phantomController.GetPlacements();
        int posnIterator = 0;
        foreach(BoardObject boardObject in selectedObjects)
        {
            if(boardObject.GetProperty("executor") > 0f && boardObject.GetProperty("canGrow") > 0f)
            {
                boardObject.GetComponent<ExecutorObject>().handleAction(ExecutorAction.GROW, posns[posnIterator]);
                posnIterator += 1;
            }
        }
    }

    // Function to cancel showing grow placement phantom overlay
    private void UpdateGrowPhantoms(string action){
        // Get mouse position
        Vector2Int cursorPosition = CalculateMousePosition();
        if(HexGridLayout.MainGrid.ValidatePointInGameGrid(cursorPosition)){
            // Count growable objects selected
            int growCount = 0;
            if(action != "update"){
                foreach(BoardObject boardObject in selectedObjects)
                {
                    if(boardObject.GetProperty("executor") > 0f && boardObject.GetProperty("canGrow") > 0f)
                    {
                        growCount += 1;
                    }
                }
            }

            // Toggle or show new render
            if(action == "toggle"){
                phantomController.Toggle(cursorPosition, growCount);
                growToggle = !growToggle;
            }
            if(action == "update"){
                phantomController.UpdateRender(cursorPosition);
                growToggle = true;
            }
            else{
                phantomController.Show(cursorPosition, growCount);
                growToggle = true;
            }
        }
    }

    // Function to cancel showing grow placement phantom overlay
    private void CancelGrowPhantoms(){
        growToggle = false;
        phantomController.Hide();
    }

    public void Update(){
        // Mouse Move
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0){
            if (!actionDialogOpen)
            {
                OutlineCursorTile();
            }
            if(growToggle){
                UpdateGrowPhantoms("update");
            }
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
            if (!actionDialogOpen)
            {
                OutlineCursorTile();
            }
        }
        // G Key
        if(Input.GetKeyDown(KeyCode.G)){
            UpdateGrowPhantoms("toggle");
        }
        // F Key - DEBUG select all movers
        if(Input.GetKeyDown(KeyCode.F)){
            // Select ALL objects
            BoardObject[] allBoardObjects = GameObject.FindObjectsOfType<BoardObject>();
            selectedObjects = allBoardObjects.ToList();

            // Update GUI with selections
            GUIController.MainGUI.ShowSelections(selectedObjects);
        }
        // LMB click
        if(Input.GetMouseButtonDown(0)){
            CancelGrowPhantoms();
            SelectCursorObjects();
        }
        // RMB click
        if(Input.GetMouseButtonDown(1)){
            rmbTimer = Time.time;
        }
        // RMB held
        if(Input.GetMouseButton(1)){
            if(!growToggle){
                if(!actionDialogOpen){
                    if(Time.time - rmbTimer >= openActionThreshold){
                        OpenActionDialog();
                    }
                }
                else{
                    ActionGUIController.MainActionGUI.UpdateSelectionFromMousePosition(Input.mousePosition);
                }
            }
        }
        // RMB release
        if(Input.GetMouseButtonUp(1)){
            if(growToggle){
                SubmitGrowAction();
                CancelGrowPhantoms();
            }
            else{
                if(!actionDialogOpen){
                    SubmitDefaultAction();
                }
                else{
                    SubmitActionDialog();
                }
            }
        }
    }
}
