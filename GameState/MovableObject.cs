using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardPathing))]
[RequireComponent(typeof(BoardObject))]
public class MovableObject : MonoBehaviour
{
    private BoardPathing myPathing;
    private BoardObject myObject;
    private ExecutorObject subscribedExecutor;

    private List<Vector2Int> queue;
    private Vector2Int lastTarget;
    private float counter = 0.0f;
    private float speed;

    private void PathToPoint(Vector2Int point){
        // For now, force the mover to stop and start a new path
        queue.Clear();
        if (subscribedExecutor != null)
        {
            subscribedExecutor.NotifyNewPath();
        }

        // Check if solid object, abort
        // if (!GameController.MainGame.GetPositionTile(point).movable){
        //     return;
        // }

        // Execute AStar pathing
        Vector2Int[] path = myPathing.AStarPath(myObject.GetPosition(), point);
        foreach(Vector2Int step in path){
            // Ignore the first step. It will always be our start and occupied by a solid
            if(step != myObject.GetPosition()){
                queue.Add(step);
            }
        }
    }

    // Function to highlight the selected hex to move to
    private void ShowTargetPosition(Vector2Int posn){
        // Reset last target posn
        HexGridLayout.MainGrid.UpdateTileMaterial(lastTarget);
        // Show new target posn
        if(myObject.targetHexTileMaterial != null){
            HexGridLayout.MainGrid.UpdateTileMaterial(posn, myObject.targetHexTileMaterial);
        }
    }

    // Public interface to issue a move command to some point
    public void MoveToPoint(Vector2Int point){
        ShowTargetPosition(point);
        PathToPoint(point);
        lastTarget = point;
    }

    // Function to handle a pathing failure
    private void HandlePathingFailure()
    {
        if(subscribedExecutor != null)
        {
            subscribedExecutor.NotifyPathingFailure();
        }
        queue.Clear();
    }
    
    private void MoveToNext(){
        if(queue.Count > 0){
            Vector2Int nextPosn = queue[0];
            if(GameController.MainGame.GetPositionTile(nextPosn).movable){
                myObject.UpdatePosition(nextPosn.x, nextPosn.y);
                queue.RemoveAt(0);
                if(subscribedExecutor != null)
                {
                    subscribedExecutor.NotifyPathingUpdate(nextPosn);
                }
            }
            else{
                HandlePathingFailure();
            }
        }
    }

    // Function to subscribe an executor to receive broadcasts from this mover
    public void SubscribeExecutor(ExecutorObject thisExecutor)
    {
        this.subscribedExecutor = thisExecutor;
    }

    public void Update(){
        // Time increments and events
        counter += Time.deltaTime * speed;
        if(counter > 1.0f){
            counter = 0.0f;
            MoveToNext();
        }
    }

    public void Start(){
        // Load props
        speed = myObject.properties["speed"];
        queue = new List<Vector2Int>();
    }

    public void Awake(){
        // Find comps
        myPathing = GetComponent<BoardPathing>();
        myObject = GetComponent<BoardObject>();
    }
}
