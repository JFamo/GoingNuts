using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class SaplingLifecycle : MonoBehaviour
{
    public float maxGrowthRate = 0.1f;

    private SaplingGrowEffectHandler matureEffectHandler;
    private float counter = 0.0f;
    private BoardObject myBoardObject;
    private TileProperties myTileProperties;

    private void Start()
    {
        matureEffectHandler = this.GetComponent<SaplingGrowEffectHandler>();
        myBoardObject = gameObject.GetComponent<BoardObject>();
        myTileProperties = GameController.MainGame.GetPositionTile(myBoardObject.GetPosition());
    }

    private void PerformGrowth()
    {
        matureEffectHandler.applyEffect(myBoardObject, myTileProperties);
    }

    private float ComputeGrowthChance()
    {
        float growthConditions = myTileProperties.shade + (1.0f - myTileProperties.water);
        return 0.1f / (1.0f + Mathf.Exp(5.0f * (growthConditions - 1.0f)));
    }

    private void AssessGrowth()
    {
        float thisRandom = Random.Range(0.0f, 1.0f);
        float currentGrowthChance = ComputeGrowthChance();
        if(thisRandom < currentGrowthChance)
        {
            PerformGrowth();
        }
    }

    void Update()
    {
        counter += Time.deltaTime;
        if (counter > 1.0f)
        {
            counter = 0.0f;
            AssessGrowth();
        }
    }
}
