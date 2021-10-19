using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [Header("Platform Generator")]
    [Header("Core")]
    [SerializeField] int platformCounter;
    [SerializeField] Vector2 lastPlatformPos;
    [SerializeField] bool canSpawnHere;
    [SerializeField] GameObject platformPrefab;
    [Header("Customization")]
    [SerializeField] int maxPlatform;
    [SerializeField] float spaceBetweenTwoPlatformsX;
    [SerializeField] float spaceBetweenTwoPlatformsY;
    [Header("Generating Platform")]
    [SerializeField] Vector2 generatedPos;
    [SerializeField] Transform borderOneX;
    [SerializeField] Transform borderTwoX;
    [SerializeField] float x;
    [SerializeField] float y;
    public void Update(){
      GeneratePlatformPosition();
    }
    public void GeneratePlatformPosition(){
        GenerateNumber();
        if(generatedPos.x > borderOneX.position.x && generatedPos.x < borderTwoX.position.x && platformCounter < maxPlatform)
         GeneratePlatform();
        else
         GenerateNumber(); 
    }
    public void GenerateNumber(){
        x = Random.Range(lastPlatformPos.x, lastPlatformPos.x + spaceBetweenTwoPlatformsX);
        y = Random.Range(lastPlatformPos.y, lastPlatformPos.y + spaceBetweenTwoPlatformsY);
        generatedPos = new Vector2(x,y);
    }
    public void GeneratePlatform(){
        lastPlatformPos = generatedPos;
        platformCounter += 1;
        Instantiate(platformPrefab, generatedPos, Quaternion.Euler(0f,0f,0f));
    }
}
