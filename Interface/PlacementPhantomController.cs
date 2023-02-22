using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPhantomController : MonoBehaviour
{
    public GameObject treePhantom;

    private bool visible;
    private Vector2Int lastComputed;
    private int lastCount;
    private List<GameObject> phantoms;
    private List<Vector2Int> currentPlacements;
    private TreePlacementGenerator placementRenderer;

    void Start(){
        visible = false;
        lastComputed = new Vector2Int(0,0);
        lastCount = 0;
        phantoms = new List<GameObject>();
        currentPlacements = new List<Vector2Int>();

        placementRenderer = GetComponent<TreePlacementGenerator>();
    }

    public List<Vector2Int> GetPlacements(){
        return currentPlacements;
    }

    // Check for re-render on mouse movement
    public void UpdateRender(Vector2Int posn){
        if(posn != lastComputed){
            Hide();
            Show(posn, lastCount);
        }
    }

    public void Show(Vector2Int posn, int count){
        // Set visible and load placements
        visible = true;
        List<Vector2Int> placements = placementRenderer.GetPlacementsAroundPosition(posn, count);
        currentPlacements = placements;

        // Create phantom at each place
        foreach(Vector2Int placement in placements){
            GameObject newObj = Instantiate(treePhantom);
            phantoms.Add(newObj);
            Vector3 placementPosn = HexGridLayout.GetPositionForHexFromCoord(placement);
            newObj.transform.position = new Vector3(placementPosn.x, newObj.transform.position.y, placementPosn.z);
        }

        // Save state for re-render
        lastComputed = posn;
        lastCount = count;
    }

    // Hide all current phantoms
    public void Hide(){
        visible = false;
        foreach(GameObject phantom in phantoms){
            Destroy(phantom);
        }
        currentPlacements.Clear();
    }

    // Toggle phantoms
    public void Toggle(Vector2Int posn, int count){
        if(visible){
            Hide();
        }
        else{
            Show(posn, count);
        }
    }
}
