using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class ExecutorObject : MonoBehaviour
{
    private ExecutorAction queuedTrigger;
    private bool hasTrigger;
    private Vector2Int triggerTarget;
    private Vector2Int targetPosition;

    private void Start()
    {
        hasTrigger = false;
    }

    private void SetTrigger(ExecutorAction trigger)
    {
        this.queuedTrigger = trigger;
        this.hasTrigger = true;
    }

    private void StopTrigger()
    {
        this.queuedTrigger = ExecutorAction.NONE;
        this.hasTrigger = false;
    }

    private void NotifyUserOfActionFailure()
    {
        Debug.Log("Executor failed to perform action " + this.queuedTrigger);
    }

    // Function to check whether there is an available successor to provided position and save it to triggerTarget if there is
    private bool LoadSuccessorTargetPosition(Vector2Int targetPosition)
    {
        // Choose open successor position if any exist
        foreach (Vector2Int successor in HexGridLayout.MainGrid.SuccessorsFromPosition(targetPosition))
        {
            // Check for movable successor position
            if (GameController.MainGame.GetPositionTile(successor).movable)
            {
                this.triggerTarget = successor; 
                return true;
                
            }
        }

        return false;
    }

    // Function to handle a given action targeting a given point
    public void handleAction(ExecutorAction action, Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;

        // Handle an action that we perform on the target tile
        if (ActionManager.IsActionInPlace(action))
        {
            this.triggerTarget = targetPosition;
        }
        else
        {
            // Load some successor position or fail if unavailable
            if (!LoadSuccessorTargetPosition(targetPosition))
            {
                NotifyUserOfActionFailure();
                StopTrigger();
                return;
            }
        }

        // Check if we have already reached our target
        if(gameObject.GetComponent<BoardObject>().GetPosition() == this.targetPosition)
        {
            // First, set the trigger, then execute it
            SetTrigger(action);
            this.executeTrigger();
        }
        else { 
            // Path to our chosen trigger target
            gameObject.GetComponent<MovableObject>().MoveToPoint(this.triggerTarget);

            // Finally, set trigger - we do this after MoveToPoint because it will broadcast and attempt to reset it
            SetTrigger(action);
        }

    }

    // Actually execute the trigger by broadcasting it to handlers at each BoardObject on the target tile
    private void executeTrigger()
    {
        // Make copy to avoid modification errors
        List<BoardObject> targetPosnObjects = new List<BoardObject>(GameController.MainGame.GetPositionObjects(this.targetPosition));
        
        // Send action to each BoardObject at position
        foreach (BoardObject targetObject in targetPosnObjects)
        {
            // Get this object's handler for our action
            IEffectHandler thisHandler = targetObject.GetEffectHandler(this.queuedTrigger);
            if (thisHandler != null)
            {
                thisHandler.applyEffect(targetObject, GameController.MainGame.GetPositionTile(this.targetPosition));
            }
        }
    }

    // Function to a Mover to notify of a pathing failure, such as a blocker
    public void NotifyPathingFailure()
    {
        NotifyUserOfActionFailure();
        StopTrigger();
    }

    // Function for a Mover to notify of a new pathing update, captuing just RMB change in order
    public void NotifyNewPath()
    {
        StopTrigger();
    }

    // Function for a Mover to notify of a new position
    public void NotifyPathingUpdate(Vector2Int newPosition)
    {
        if (this.hasTrigger)
        {
            if(newPosition == this.triggerTarget)
            {
                this.executeTrigger();
            }
        }
    }
  
}
