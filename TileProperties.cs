using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties
{
    public Vector2Int myPosition;
    public bool movable;
    public float shade;

    public TileProperties(Vector2Int posn){
        this.myPosition = posn;
        this.movable = true;
        this.shade = 0.0f;
    }

    public TileProperties(Vector2Int posn, bool movable, float shade){
        this.myPosition = posn;
        this.movable = movable;
        this.shade = shade;
    }

    // Function to trigger tile property updates
    public void UpdateProperties(){
        UpdateMovable();
        UpdateShade();
    }

    // Function to update whether this tile is movable
    private void UpdateMovable(){
        bool canMove = true;
        List<BoardObject> occupants = GameController.MainGame.GetPositionObjects(myPosition);
        foreach(BoardObject obj in occupants){
            if(obj.properties["solid"] > 0f){
                canMove = false;
                break;
            }
        }
        this.movable = canMove;
    }

    // Function to update shade at this tile
    private void UpdateShade(){
        if(this.shade > 0.5f){
            HexGridLayout.MainGrid.UpdateTileShading(myPosition, true);
        }
        else{
            HexGridLayout.MainGrid.UpdateTileShading(myPosition, false);
        }
    }
}
