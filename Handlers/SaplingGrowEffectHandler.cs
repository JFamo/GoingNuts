using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaplingGrowEffectHandler : IEffectHandler
{
    public GameObject transformationObject;

    public override void applyEffect(BoardObject thisObject, TileProperties thisTile)
    {
        // Destroy Sapling
        GameController.MainGame.DestroyObject(thisObject);
        Destroy(thisObject.gameObject);

        // Create Tree
        GameObject newTransformationObject = Instantiate(transformationObject);
        newTransformationObject.GetComponent<BoardObject>().UpdatePosition(thisTile.tilePosition.x, thisTile.tilePosition.y);

        // Update tile
        thisTile.AddWater(0.2f);
        thisTile.AddShade(0.8f);

        // Update adjacent tiles
        foreach (TileProperties adjacentTileProperties in thisTile.GetAdjacentTileProperties())
        {
            adjacentTileProperties.AddShade(0.3f);
            adjacentTileProperties.AddWater(-0.1f);
        }
    }
}