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
    public Dictionary<string, float> properties;
    public List<BoardObjectStringsPair> StringList = new List<BoardObjectStringsPair>();
    public Dictionary<string, string> strings;

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
        if(GetProperty("movable") > 0f){
            gameObject.AddComponent(typeof(BoardPathing));
            gameObject.AddComponent(typeof(MovableObject));
        }
    }

    public float GetProperty(string key){
        if(properties != null){
            if(properties.ContainsKey(key)){
                return properties[key];
            }
        }
        else{
            foreach(BoardObjectPropertiesPair propPair in PropList){
                if(propPair.propName == key){
                    return propPair.propValue;
                }
            }
        }
        Debug.LogError("Failed to get object property " + key);
        return 0.0f;
    }

    public string GetString(string key){
        if(strings != null){
            if(strings.ContainsKey(key)){
                return strings[key];
            }
        }
        else{
            foreach(BoardObjectStringsPair stringPair in StringList){
                if(stringPair.stringName == key){
                    return stringPair.stringValue;
                }
            }
        }
        Debug.LogError("Failed to get object string " + key);
        return "UNDEFINED";
    }

    public void Start(){
        if(properties.ContainsKey("editorspawn")){
            UpdatePosition(xCoord, zCoord);
        }
    }

    public void Awake(){

        properties = new Dictionary<string, float>();
        strings = new Dictionary<string, string>();

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
