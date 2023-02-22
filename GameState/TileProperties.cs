using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileProperties
{
    public Vector2Int tilePosition;
    public bool movable;
    public float shade;
    public float water;

    private List<Material> outlineMaterials;

    public TileProperties(Vector2Int posn){
        this.tilePosition = posn;
        this.movable = true;
        this.shade = 0.0f;
        this.water = 0.0f;

        outlineMaterials = new List<Material>();
    }

    public TileProperties(Vector2Int posn, bool movable, float shade, float water){
        this.tilePosition = posn;
        this.movable = movable;
        this.shade = shade;
        this.water = water;

        outlineMaterials = new List<Material>();
    }

    // Function to trigger tile property updates
    public void UpdateProperties(){
        UpdateMovable();
        UpdateOverlay(); // TODO probably remove this once we do better shading
    }

    // Setter for shade
    public void SetShade(float shade){
        this.shade = shade;
        if(this.shade < 0.0f){
            this.shade = 0.0f;
        }
        if(this.shade > 1.0f){
            this.shade = 1.0f;
        }
        UpdateOverlay();
    }

    // Setter for water
    public void SetWater(float water){
        this.water = water;
        if(this.water < 0.0f){
            this.water = 0.0f;
        }
        if(this.water > 1.0f){
            this.water = 1.0f;
        }
        UpdateOverlay();
    }

    // Adder for shade
    public void AddShade(float diff)
    {
        this.shade = this.shade + diff;
        if (this.shade < 0.0f)
        {
            this.shade = 0.0f;
        }
        if (this.shade > 1.0f)
        {
            this.shade = 1.0f;
        }
        UpdateOverlay();
    }

    // Adder for water
    public void AddWater(float diff)
    {
        this.water = this.water + diff;
        if (this.water < 0.0f)
        {
            this.water = 0.0f;
        }
        if (this.water > 1.0f)
        {
            this.water = 1.0f;
        }
        UpdateOverlay();
    }

    // Function to check whether position is growable
    public bool CanGrow(){
        if(this.movable && this.water >= 0.6f && this.shade <= 0.3f){
            return true;
        }
        return false;
    }

    // Function to update whether this tile is movable
    private void UpdateMovable(){
        bool canMove = true;
        List<BoardObject> occupants = GameController.MainGame.GetPositionObjects(tilePosition);
        foreach(BoardObject obj in occupants){
            //DEBUG
            if(obj.GetProperty("solid") > 0f){
                canMove = false;
                break;
            }
        }
        this.movable = canMove;
    }

    // Function to update shade at this tile
    private void UpdateOverlay(){
        HexOverlay.MainOverlay.UpdateOverlayAtPosition(tilePosition, this);
    }

    // Function to recompute and apply outline material
    private void RecomputeOutline(){
        if(outlineMaterials.Count > 0){
            HexGridLayout.MainGrid.UpdateTileMaterial(tilePosition, outlineMaterials.Last());
        }
        else{
            HexGridLayout.MainGrid.UpdateTileMaterial(tilePosition);
        }
    }

    // Function to enqueue a material
    public void EnqueueOutlineMaterial(Material mat){
        outlineMaterials.Add(mat);
        RecomputeOutline();
    }

    // Function to dequeue a material
    public void DequeueOutlineMaterial(Material mat){
        outlineMaterials.Remove(mat); // TODO could need to change this logic because remove pops first occurence
        RecomputeOutline();
    }

    // Function to get adjacent TileProperties
    public List<TileProperties> GetAdjacentTileProperties()
    {
        List <TileProperties> adjacentProperties = new List<TileProperties>();
        foreach (Vector2Int adjacentTile in HexGridLayout.MainGrid.SuccessorsFromPosition(tilePosition))
        {
            adjacentProperties.Add(GameController.MainGame.GetPositionTile(adjacentTile));
        }
        return adjacentProperties;
    }
}
