using UnityEngine;

[CreateAssetMenu(fileName = "FishSpawnData", menuName = "Fish Spawn Data", order = 51)]
public class FishSpawnData : ScriptableObject
{
    [Header("Prefab Settings")]
    public GameObject fishPrefab;

    [Header("Fish Type")]
    public FishType fishType;

    [Header("Spawn Height Range")]
    // Allowed spawn heights (for example, lower: -100, upper: -3)
    public int upperSpawnHeight = -3;
    public int lowerSpawnHeight = -100;

    [Header("Fish Scale")]
    public float minFishScale = 1f;
    public float maxFishScale = 1f;
    
    [Header("Fish Spawn Rate")]
    public int fishSpawnAttempts = 100;
    
    [Header("Fish Schooling")]
    public bool schoolOfFish = false;

    [Header("Fish Mass")]
    public float mass = 1.0f;


  

}
public enum  FishType
{
    Grouper,
    Puffer,
    Swordfish,
    TropicalGreen,
    TropicalOrange,
    Clown,
    Shark, 
    Goldfish,
    Salmon,
    Hammerhead,
}