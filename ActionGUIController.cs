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
    public string[] actionNamesList;
    public Transform[] actionIconsList;
    public Transform radialBar;
    public int radius = 500;
    public int offset = 10;

    private List<GameObject> activeBars;
    private Dictionary<string, Transform> actionIconsMap;

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

        CreateActionIconsMap();
    }

    void Update(){
        float newScale = CalculateScale();
        actionGUI.localScale = new Vector3(newScale,newScale,newScale);
    }

    public void CreateActionUIAtPosition(Vector2Int gameCoordinates, List<string> availableActions){
        DisableAllIcons();
        actionGUI.position = HexGridLayout.GetPositionForHexFromCoord(gameCoordinates);
        actionGUI.gameObject.SetActive(true);
        InitializeActionIcons(availableActions);
    }

    private float CalculateScale(){
        float cameraY = Camera.main.transform.position.y;
        return 0.01f + ((0.04f / (38f - 6f)) * (cameraY - 6.0f));
    }

    private void DisableAllIcons(){
        foreach(KeyValuePair<string, Transform> iconEntry in actionIconsMap){
            iconEntry.Value.gameObject.SetActive(false);
        }
        foreach(GameObject bar in activeBars){
            Destroy(bar);
        }
        activeBars.Clear();
    }

    private float DegreesToRadians(float deg){
        return Mathf.PI * deg / 180.0f;
    }

    private int CalculateBarAngle(int index, int count){
        return -90 + (int)(this.offset + (index * 80 / count));
    }

    private Vector3 CalculateIconPosition(int index, int count){
        int yPosn = (int)(this.radius * Mathf.Sin(DegreesToRadians(this.offset + (index * 80 / count))));
        int xPosn = (int)(this.radius * Mathf.Cos(DegreesToRadians(this.offset + (index * 80 / count))));
        return new Vector3(xPosn, yPosn, 0);
    }

    private void InitializeActionIcon(string actionName, int index, int totalCount){
        actionIconsMap[actionName].gameObject.SetActive(true);
        actionIconsMap[actionName].localPosition = CalculateIconPosition(index, totalCount);
        Debug.Log("Using calculated position " + CalculateIconPosition(index, totalCount).x + ", " + CalculateIconPosition(index, totalCount).y);
        GameObject thisRadialBar = Instantiate(radialBar.gameObject, actionGUI);
        thisRadialBar.GetComponent<RectTransform>().rotation = Quaternion.Euler(50,0, CalculateBarAngle(index, totalCount));
        thisRadialBar.GetComponent<RectTransform>().SetAsFirstSibling();
        thisRadialBar.SetActive(true);
        activeBars.Add(thisRadialBar);
    }

    private void InitializeActionIcons(List<string> availableActions){
        int i = 0;
        foreach(string actionName in availableActions){
            InitializeActionIcon(actionName, i, availableActions.Count);
            i += 1;
        }
    }

    private void CreateActionIconsMap(){
        if(actionNamesList.Length != actionIconsList.Length){
            Debug.LogError("Action GUI names list size does not match GUI icons list!");
        }
        else{
            actionIconsMap = new Dictionary<string, Transform>();

            for(int i = 0; i < actionNamesList.Length; i ++){
                actionIconsMap.Add(actionNamesList[i], actionIconsList[i]);
            }
        }
    }
}
