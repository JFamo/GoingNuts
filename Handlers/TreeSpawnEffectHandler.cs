using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawnEffectHandler : IEffectHandler
{
    public GameObject creationObject;

    public override void applyEffect(BoardObject thisObject, TileProperties thisTile)
    {
        // Create nut at adjacent
        foreach (TileProperties adjacentTileProperties in thisTile.GetAdjacentTileProperties())
        {
            if (adjacentTileProperties.movable)
            {
                // Spawn object
                GameObject newObject = Instantiate(creationObject);
                newObject.GetComponent<BoardObject>().UpdatePosition(adjacentTileProperties.tilePosition.x, adjacentTileProperties.tilePosition.y);

                // Return to only spawn one
                return;

            }
        }

    }
}