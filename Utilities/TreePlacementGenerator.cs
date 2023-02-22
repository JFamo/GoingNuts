using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreePlacementGenerator : MonoBehaviour
{
    public List<Vector2Int> GetPlacementsAroundPosition(Vector2Int posn, int count){
        return GetPlacementsAroundPositionWithThreshold(posn, count, 0.8f);
    }

    public List<Vector2Int> GetPlacementsAroundPositionWithThreshold(Vector2Int posn, int count, float threshold){
        // Setup lists for exploring
        List<Vector2Int> explored = new List<Vector2Int>();
        List<Vector2Int> expanded = new List<Vector2Int>();
        List<Vector2Int> assigned = new List<Vector2Int>();

        // Init
        TileProperties thisTileProps = GameController.MainGame.GetPositionTile(posn);
        if(thisTileProps.CanGrow()){
            assigned.Add(posn);
        }
        explored.Add(posn);

        // Iterate assignments
        while(assigned.Count < count){

            // Handle nothing else to explore
            if(explored.Count <= 0){
                return new List<Vector2Int>();
            }

            // Choose next
            Vector2Int thisPosn = explored.First();
            explored.RemoveAt(0);
            expanded.Add(thisPosn);

            // Get TileProps
            thisTileProps = GameController.MainGame.GetPositionTile(thisPosn);

            // Iterate each successor
            foreach(TileProperties successorTileProps in thisTileProps.GetAdjacentTileProperties()){

                // Only consider if unexplored/expanded
                if(explored.IndexOf(successorTileProps.tilePosition) < 0 && expanded.IndexOf(successorTileProps.tilePosition) < 0){

                    // Check if valid
                    if(successorTileProps.CanGrow()){

                        // Occasionally skip valid assignment for randomness
                        if(Random.Range(0.0f, 1.0f) < threshold){

                            assigned.Add(successorTileProps.tilePosition);

                            // Check if sufficient and exit
                            if(assigned.Count >= count){
                                return assigned;
                            }
                        
                        }

                    }

                    // Add as explored
                    explored.Add(successorTileProps.tilePosition);

                }

            }

        }

        // Return assignments or empty list
        return assigned;
    }
}
