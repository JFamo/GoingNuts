using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static List<string> GetPossibleActions(BoardObject subject, List<BoardObject> targetObjects, TileProperties targetTile){

        bool canMove = true;
        bool canGrow = true;
        bool canClear = false;
        bool canFight = false;
        bool canGather = false;

        foreach(BoardObject thisObject in targetObjects){

            if(thisObject.GetProperty("solid") >= 1.0f){
                canMove = false;
                canGrow = false;
            }
            if(thisObject.GetProperty("collectable") >= 1.0f){
                canGather = true;
            }
            if(thisObject.GetProperty("clearable") >= 1.0f){
                canClear = true;
            }

        }

        if(targetTile.shade >= 0.8f || targetTile.water <= 0.2f){
            canGrow = false;
        }

        List<string> possibleActions = new List<string>();

        if(canMove){
            possibleActions.Add("move");
        }
        if(canGrow){
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
}
