using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDestroyEffectHandler : IEffectHandler
{
    public override void applyEffect(BoardObject thisObject, TileProperties thisTile){
        GameController.MainGame.DestroyObject(thisObject);
        Destroy(thisObject.gameObject);
    }
}
