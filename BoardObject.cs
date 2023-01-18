using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    [System.Serializable]
    public class BoardObjectPropertiesPair {
        public string propName;
        public float propValue;
    }

    [System.Serializable]
    public class BoardObjectStringsPair {
        public string stringName;
        public string stringValue;
    }

    [Header("Position")]
    public float xCoord = 0f;
    public float zCoord = 0f;

    [Header("Properties")]
    public List<BoardObjectPropertiesPair> PropList = new List<BoardObjectPropertiesPair>();
    public Dictionary<string, float> properties = new Dictionary<string, float>();
    public List<BoardObjectStringsPair> StringList = new List<BoardObjectStringsPair>();
    public Dictionary<string, string> strings = new Dictionary<string, string>();

    [Header("Materials")]
    public Material hexTileMaterial;
    public Material targetHexTileMaterial;

    public void UpdatePosition(float newx, float newz){
        // Build posn
        Vector2Int gridIntPosn = new Vector2Int((int)newx, (int)newz);

        // Convert to hex coords
        Vector3 newGamePosition = HexGridLayout.GetPositionForHexFromCoord(gridIntPosn);

        // Update game
        GameController.MainGame.MoveObject(this, GetPosition(), gridIntPosn);

        // Update locally
        xCoord = newx;
        zCoord = newz;

        // Move object
        transform.position = new Vector3(newGamePosition.x, transform.position.y + newGamePosition.y, newGamePosition.z);
    }

    public Vector2Int GetPosition(){
        return new Vector2Int((int)xCoord, (int)zCoord);
    }

    private void InitializeComponents(){
        if(properties["movable"] > 0f){
            gameObject.AddComponent(typeof(BoardPathing));
            gameObject.AddComponent(typeof(MovableObject));
        }
    }

    public void Start(){
        UpdatePosition(xCoord, zCoord);
    }

    public void Awake(){
        // Initialize Properties
        foreach(var propPair in PropList){
            properties[propPair.propName] = propPair.propValue;
        }

        // Initialize Strings
        foreach(var stringPair in StringList){
            strings[stringPair.stringName] = stringPair.stringValue;
        }

        InitializeComponents();
    }
}
