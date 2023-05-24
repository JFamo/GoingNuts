using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionGUIController : MonoBehaviour
{
    // Singleton instance MainActionGUI
    public static ActionGUIController MainActionGUI {get; private set;}

    [Header("Action GUI Settings")]
    public Transform actionGUI;
    public ExecutorAction[] actionNamesList;
    public Transform[] actionIconsList;
    public Transform radialBar;
    public int radius = 500;
    public int offset = 10;
    public float degreeRange = 80.0f;
    public float insensitiveSelectionRange = 10.0f;

    private List<GameObject> activeBars;
    private Dictionary<ExecutorAction, Transform> actionIconsMap;
    private Vector3 initialMousePosition;
    private Dictionary<int, ExecutorAction> activeIconList;
    private Image highlightedActionIcon;

    void Awake(){
        // Setup singleton
        if(MainActionGUI != null && MainActionGUI != this){
            Destroy(this);
        }
        else{
            MainActionGUI = this;
        }
    }

    void Start(){
        actionGUI.gameObject.SetActive(false);

        radialBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.radius);
        radialBar.gameObject.SetActive(false);

        activeBars = new List<GameObject>();
        activeIconList = new Dictionary<int, ExecutorAction>();

        CreateActionIconsMap();
    }

    void Update(){
        float newScale = CalculateScale();
        actionGUI.localScale = new Vector3(newScale,newScale,newScale);
    }

    public void CreateActionUIAtPosition(Vector2Int gameCoordinates, List<ExecutorAction> availableActions, Vector3 initialMousePosition){
        DisableAllIcons();
        this.initialMousePosition = initialMousePosition;
        actionGUI.position = HexGridLayout.GetPositionForHexFromCoord(gameCoordinates);
        actionGUI.gameObject.SetActive(true);
        InitializeActionIcons(availableActions);
    }

    private ExecutorAction GetSelectedAction(Vector3 mousePosition){

        // Handle empty actionlist
        if(activeIconList.Count == 0){
            Debug.LogError("Trying to select default action from empty list");
            return ExecutorAction.NONE;
        }

        // Handle inner circle default
        if(Vector3.Distance(initialMousePosition, mousePosition) <= insensitiveSelectionRange){
            return activeIconList[0];
        }

        // Calculate relative mouse posn
        float xDiff = mousePosition.x - initialMousePosition.x;
        float yDiff = mousePosition.y - initialMousePosition.y;

        // Calculate angle of mouse posn
        float innerAngle = RadiansToDegrees(Mathf.Atan(yDiff / xDiff));

        // Handle Q2 and Q4 cases
        if(innerAngle <= 0.0f){
            if(xDiff >= 0.0f){
                return activeIconList[0];
            }
            else{
                return activeIconList[activeIconList.Count - 1];
            }
        }

        // Calculate value from inside offset
        if(innerAngle <= this.offset){
            return activeIconList[0];
        }

        // Calculate index from inner angle Q1 and Q3
        int selectedIndex = (int)Mathf.Floor((activeIconList.Count / this.degreeRange) * (innerAngle - this.offset));

        return activeIconList[selectedIndex];

    }

    public void UpdateSelectionFromMousePosition(Vector3 newMousePosition){
        // Compute selected action
        ExecutorAction selectionAction = GetSelectedAction(newMousePosition);

        // Undo highlighting on image if present
        if(highlightedActionIcon != null){
            highlightedActionIcon.color = new Color32(255,255,255,255);
        }

        // Apply highlighting to image
        Image thisImage = actionIconsMap[selectionAction].gameObject.GetComponent<Image>();
        thisImage.color = new Color32(100, 255, 255, 255);
        highlightedActionIcon = thisImage;
    }

    public ExecutorAction SubmitAction(Vector3 mousePosition){
        ExecutorAction thisAction = GetSelectedAction(mousePosition);
        DisableAllIcons();
        return thisAction;
    }

    private float CalculateScale(){
        float cameraY = Camera.main.transform.position.y;
        return 0.01f + ((0.04f / (38f - 6f)) * (cameraY - 6.0f));
    }

    private void DisableAllIcons(){
        foreach(KeyValuePair<ExecutorAction, Transform> iconEntry in actionIconsMap){
            iconEntry.Value.gameObject.SetActive(false);
        }
        foreach(GameObject bar in activeBars){
            Destroy(bar);
        }
        activeBars.Clear();
        activeIconList.Clear();
    }

    private float DegreesToRadians(float deg){
        return Mathf.PI * deg / 180.0f;
    }

    private float RadiansToDegrees(float rads){
        return rads * 180.0f / Mathf.PI;
    }

    private int CalculateBarAngle(int index, int count){
        return -90 + (int)(this.offset + (index * this.degreeRange / count));
    }

    private Vector3 CalculateIconPosition(int index, int count){
        int yPosn = (int)(this.radius * Mathf.Sin(DegreesToRadians(this.offset + (index * this.degreeRange / count))));
        int xPosn = (int)(this.radius * Mathf.Cos(DegreesToRadians(this.offset + (index * this.degreeRange / count))));
        return new Vector3(xPosn, yPosn, 0);
    }

    private void InitializeActionIcon(ExecutorAction actionName, int index, int totalCount){
        // Activate action icon
        actionIconsMap[actionName].gameObject.SetActive(true);
        actionIconsMap[actionName].localPosition = CalculateIconPosition(index, totalCount);
        activeIconList.Add(index, actionName);

        // Initialize radial bar
        GameObject thisRadialBar = Instantiate(radialBar.gameObject, actionGUI);
        thisRadialBar.GetComponent<RectTransform>().rotation = Quaternion.Euler(50,0, CalculateBarAngle(index, totalCount));
        thisRadialBar.GetComponent<RectTransform>().SetAsFirstSibling();
        thisRadialBar.SetActive(true);
        activeBars.Add(thisRadialBar);
    }

    private void InitializeActionIcons(List<ExecutorAction> availableActions){
        int i = 0;
        foreach(ExecutorAction actionName in availableActions){
            InitializeActionIcon(actionName, i, availableActions.Count);
            i += 1;
        }
    }

    private void CreateActionIconsMap(){
        if(actionNamesList.Length != actionIconsList.Length){
            Debug.LogError("Action GUI names list size does not match GUI icons list!");
        }
        else{
            actionIconsMap = new Dictionary<ExecutorAction, Transform>();

            for(int i = 0; i < actionNamesList.Length; i ++){
                actionIconsMap.Add(actionNamesList[i], actionIconsList[i]);
            }
        }
    }
}
