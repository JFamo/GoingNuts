using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    // Singleton
    public static GUIController MainGUI {get; private set;}

    public Transform selectionPanel;
    public GameObject selectedTileGuiObject;
    public GameObject selectedBoardObjectGuiObject;

    private List<GameObject> activeBoardObjectGuiObjects;

    void Awake(){
        if(MainGUI != null && MainGUI != this){
            Destroy(this);
        }
        else{
            MainGUI = this;
        }
    }

    void Start(){
        selectedTileGuiObject.SetActive(false);
        activeBoardObjectGuiObjects = new List<GameObject>();
    }

    private void ShowSelectedTile(TileProperties tile){
        if(tile == null){
            selectedTileGuiObject.SetActive(false);
        }
        else{
            selectedTileGuiObject.SetActive(true);
            Text[] childText = selectedTileGuiObject.GetComponentsInChildren<Text>();
            childText[0].text = "(" + tile.myPosition.x + ", " + tile.myPosition.y + ")";
            childText[1].text = "Movable : " + tile.movable + "\nShade : " + tile.shade;
        }
    }

    private void ShowSelectedObjects(List<BoardObject> objects){
        foreach(BoardObject thisObject in objects){
            // Create copy of GUI tile
            GameObject newObjectGui = Instantiate(selectedBoardObjectGuiObject, selectionPanel);

            // Update text
            Text[] childText = newObjectGui.GetComponentsInChildren<Text>();
            childText[0].text = thisObject.strings["name"];
            childText[1].text = thisObject.strings["tag"];
            // TODO just have this iterate I think
            childText[2].text = "Movable: " + thisObject.properties["movable"] + "\nSolid: " + thisObject.properties["solid"] + "\nSpeed: " + thisObject.properties["speed"];

            activeBoardObjectGuiObjects.Add(newObjectGui);
        }
    }

    private void DeleteOldSelectionGUI(){
        foreach(GameObject thisObj in activeBoardObjectGuiObjects){
            Destroy(thisObj);
        }
        activeBoardObjectGuiObjects.Clear();
    }

    public void ShowSelections(List<BoardObject> objects, TileProperties tile){
        DeleteOldSelectionGUI();
        ShowSelectedObjects(objects);
        ShowSelectedTile(tile);
    }

    public void ClearSelections(){
        DeleteOldSelectionGUI();
        ShowSelectedTile(null);
    }
}
