using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fake interface to support serialization
public class IEffectHandler : MonoBehaviour{

    public string[] actionNames;

    public virtual void applyEffect(BoardObject thisObject, TileProperties thisTile){
        Debug.LogError("Failed to find child class effect for " + thisObject.GetString("name"));
    }

}