using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTransmuteEffectHandler : IEffectHandler
{
    public GameObject transformationObject;

    public override void applyEffect(BoardObject thisObject, TileProperties thisTile){
        GameController.MainGame.DestroyObject(thisObject);
        Destroy(thisObject.gameObject);

        GameObject newTransformationObject = Instantiate(transformationObject);
        newTransformationObject.GetComponent<BoardObject>().UpdatePosition(thisTile.tilePosition.x, thisTile.tilePosition.y);
    }
}