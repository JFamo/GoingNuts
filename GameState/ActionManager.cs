using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static List<string> GetPossibleActions(BoardObject subject, List<BoardObject> targetObjects, TileProperties targetTile){

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

        List<string> possibleActions = new List<string>();

        if(canMove){
            possibleActions.Add("move");
        }
        if(targetTile.CanGrow()){
            possibleActions.Add("grow");
        }
        if(canClear){
            possibleActions.Add("clear");
        }
        if(canGather){
            possibleActions.Add("gather");
        }
        if(canFight){
            possibleActions.Add("fight");
        }

        return possibleActions;

    }

    public static bool IsActionInPlace(string action)
    {
        if(action == "grow" || action == "move" || action == "clear")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // DEFUNCT - Now using executor for handling actions
    public static void ProcessAction(string action, List<BoardObject> selectedObjects, List<BoardObject> targetObjects, TileProperties targetTile){
        // Handle move
        foreach(BoardObject selectedObject in selectedObjects){
            if(action == "move" && selectedObject.GetProperty("movable") >= 1.0f){
                selectedObject.GetComponent<MovableObject>().MoveToPoint(targetTile.tilePosition);
            }
            if(action == "grow"){
                IEffectHandler thisHandler = selectedObject.GetEffectHandler(action);
                if(thisHandler != null){
                    thisHandler.applyEffect(selectedObject, targetTile);
                }
            }
        }
        // Apply effects
        foreach(BoardObject targetObject in targetObjects){
            IEffectHandler thisHandler = targetObject.GetEffectHandler(action);
            if(thisHandler != null){
                thisHandler.applyEffect(targetObject, targetTile);
            }
        }
    }
}
