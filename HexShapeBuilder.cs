using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexWithFaces
{
    public int[] faces;
    public Vector2Int position;

    public HexWithFaces(int[] faces, Vector2Int position){
        this.faces = faces;
        this.position = position;
    }
}

public class HexShapeBuilder : MonoBehaviour
{

    public static List<HexWithFaces> GetTriangleTopAtPoint(Vector2Int top){
        List<HexWithFaces> shapeList = new List<HexWithFaces>();
        shapeList.Add(new HexWithFaces(new int[]{0,1,2,3}, top));
        shapeList.Add(new HexWithFaces(new int[]{2,3,4,5}, HexGridLayout.GetBottomLeftOfPosition(top)));
        shapeList.Add(new HexWithFaces(new int[]{4,5,0,1}, HexGridLayout.GetBottomRightOfPosition(top)));
        return shapeList;
    }

    public static List<HexWithFaces> GetRadialShapeAroundPoint(Vector2Int center, int radius){
        List<HexWithFaces> shapeList = new List<HexWithFaces>();

        // Handle single point
        if(radius <= 1){
            shapeList.Add(new HexWithFaces(new int[]{0,1,2,3,4,5}, center));
            return shapeList;
        }

        int topRightCol, topRow, topLeftCol, botRightCol, botRow, botLeftCol;

        // Calculate top and bottom row
        botRow = center.y + (radius - 1);
        topRow = center.y - (radius - 1);

        // Determine if this is a left or right row
        if(center.y % 2 == 0){ // Right
            
            botRightCol = center.x + (int)Mathf.Ceil((radius-1)/2.0f);
            botLeftCol = center.x - (int)Mathf.Floor((radius-1)/2.0f);
            topRightCol = center.x + (int)Mathf.Ceil((radius-1)/2.0f);
            topLeftCol = center.x - (int)Mathf.Floor((radius-1)/2.0f);

        }
        else{ // Left

            botRightCol = center.x + (int)Mathf.Floor((radius-1)/2.0f);
            botLeftCol = center.x - (int)Mathf.Ceil((radius-1)/2.0f);
            topRightCol = center.x + (int)Mathf.Floor((radius-1)/2.0f);
            topLeftCol = center.x - (int)Mathf.Ceil((radius-1)/2.0f);

        }

        // Add corner spokes
        shapeList.Add(new HexWithFaces(new int[]{4,5,0}, new Vector2Int(botRightCol, botRow)));
        shapeList.Add(new HexWithFaces(new int[]{2,1,0}, new Vector2Int(topRightCol, topRow)));
        shapeList.Add(new HexWithFaces(new int[]{3,4,5}, new Vector2Int(botLeftCol, botRow)));
        shapeList.Add(new HexWithFaces(new int[]{3,2,1}, new Vector2Int(topLeftCol, topRow)));

        // Add direct left/right spokes
        Vector2Int rightSpokePosition = new Vector2Int(center.x + (radius-1), center.y);
        Vector2Int leftSpokePosition = new Vector2Int(center.x - (radius-1), center.y);
        shapeList.Add(new HexWithFaces(new int[]{1,0,5}, rightSpokePosition));
        shapeList.Add(new HexWithFaces(new int[]{2,3,4}, leftSpokePosition));

        // Add top/bottom edges
        for(int topBotEdgeIndex = 1; topBotEdgeIndex < topRightCol - topLeftCol; topBotEdgeIndex += 1){
            shapeList.Add(new HexWithFaces(new int[]{2,1}, new Vector2Int(topLeftCol + topBotEdgeIndex, topRow)));
            shapeList.Add(new HexWithFaces(new int[]{4,5}, new Vector2Int(botLeftCol + topBotEdgeIndex, botRow))); 
        }

        // Add top right edges
        Vector2Int diagonalEdgePosition = HexGridLayout.GetBottomRightOfPosition(new Vector2Int(topRightCol, topRow));
        while(diagonalEdgePosition != rightSpokePosition){
            shapeList.Add(new HexWithFaces(new int[]{0,1}, diagonalEdgePosition));
            diagonalEdgePosition = HexGridLayout.GetBottomRightOfPosition(diagonalEdgePosition);
        }

        // Add top left edges
        diagonalEdgePosition = HexGridLayout.GetBottomLeftOfPosition(new Vector2Int(topLeftCol, topRow));
        while(diagonalEdgePosition != leftSpokePosition){
            shapeList.Add(new HexWithFaces(new int[]{2,3}, diagonalEdgePosition));
            diagonalEdgePosition = HexGridLayout.GetBottomLeftOfPosition(diagonalEdgePosition);
        }

        // Add bottom right edges
        diagonalEdgePosition = HexGridLayout.GetBottomLeftOfPosition(rightSpokePosition);
        while(diagonalEdgePosition != (new Vector2Int(botRightCol, botRow))){
            shapeList.Add(new HexWithFaces(new int[]{0,5}, diagonalEdgePosition));
            diagonalEdgePosition = HexGridLayout.GetBottomLeftOfPosition(diagonalEdgePosition);
        }

        // Add bottom left edges
        diagonalEdgePosition = HexGridLayout.GetBottomRightOfPosition(leftSpokePosition);
        while(diagonalEdgePosition != (new Vector2Int(botLeftCol, botRow))){
            shapeList.Add(new HexWithFaces(new int[]{3,4}, diagonalEdgePosition));
            diagonalEdgePosition = HexGridLayout.GetBottomRightOfPosition(diagonalEdgePosition);
        }

        return shapeList;
    }
}
