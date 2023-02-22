using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class TreeLifecycle : MonoBehaviour
{
    public float maxGrowthRate = 0.1f;

    private TreeSpawnEffectHandler spawnEffectHandler;
    private float counter = 0.0f;
    private BoardObject myBoardObject;
    private TileProperties myTileProperties;

    private void Start()
    {
        spawnEffectHandler = this.GetComponent<TreeSpawnEffectHandler>();
        myBoardObject = gameObject.GetComponent<BoardObject>();
        myTileProperties = GameController.MainGame.GetPositionTile(myBoardObject.GetPosition());
    }

    private void PerformSpawn()
    {
        spawnEffectHandler.applyEffect(myBoardObject, myTileProperties);
    }

    private int GetOpenAdjacentTiles()
    {
        int count = 0;
        foreach(TileProperties adjTileProps in myTileProperties.GetAdjacentTileProperties())
        {
            if (adjTileProps.movable)
            {
                count += 1;
            }
        }
        return count;
    }

    private float ComputeSpawnChance()
    {
        float growthConditions = myTileProperties.shade + (1.0f - myTileProperties.water);
        float openAdjacentFraction = GetOpenAdjacentTiles() / 6.0f;
        return openAdjacentFraction * 0.1f / (1.0f + Mathf.Exp(5.0f * (growthConditions - 1.0f)));
    }

    private void AssessSpawn()
    {
        float thisRandom = Random.Range(0.0f, 1.0f);
        float currentGrowthChance = ComputeSpawnChance();
        if (thisRandom < currentGrowthChance)
        {
            PerformSpawn();
        }
    }

    void Update()
    {
        counter += Time.deltaTime;
        if (counter > 1.0f)
        {
            counter = 0.0f;
            AssessSpawn();
        }
    }
}
