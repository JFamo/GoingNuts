using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExecutorAction {
    NONE,
    MOVE,
    GROW,
    CLEAR,
    GATHER,
    FIGHT
}

public class ActionManager : MonoBehaviour
{
    public static List<ExecutorAction> GetPossibleActions(BoardObject subject, List<BoardObject> targetObjects, TileProperties targetTile){

        bool canMove = true;
        bool canClear = false;
        bool canFight = false;
        bool canGather = false;

        foreach(BoardObject thisObject in targetObjects){

            if(thisObject.GetProperty("solid") >= 1.0f){
                canMove = false;
            }
            if(thisObject.GetProperty("collectable") >= 1.0f){
                canGather = true;
            }
            if(thisObject.GetProperty("clearable") >= 1.0f){
                canClear = true;
            }

        }

        List<ExecutorAction> possibleActions = new List<ExecutorAction>();

        if(canMove){
            possibleActions.Add(ExecutorAction.MOVE);
        }
        if(targetTile.CanGrow()){
            possibleActions.Add(ExecutorAction.GROW);
        }
        if(canClear){
            possibleActions.Add(ExecutorAction.CLEAR);
        }
        if(canGather){
            possibleActions.Add(ExecutorAction.GATHER);
        }
        if(canFight){
            possibleActions.Add(ExecutorAction.FIGHT);
        }

        return possibleActions;

    }

    public static bool IsActionInPlace(ExecutorAction action)
    {
        if(action == ExecutorAction.GROW || action == ExecutorAction.MOVE || action == ExecutorAction.CLEAR)
        {
            return true;
        }
        return false;
    }

    // Function to get the default action given a selected object and target position
    public static ExecutorAction GetDefaultAction(BoardObject subject, List<BoardObject> targetObjects, TileProperties targetTile){

        bool canMove = true;

        // First, check for collectable
        foreach(BoardObject thisObject in targetObjects){

            if(thisObject.GetProperty("collectable") >= 1.0f){
                return ExecutorAction.GATHER;
            }
            else if(thisObject.GetProperty("solid") >= 1.0f){
                canMove = false;
            }

        }

        // Next, try to move
        if(canMove){
            return ExecutorAction.MOVE;
        }

        // Otherwise, do nothing
        return ExecutorAction.NONE;

    }

}
