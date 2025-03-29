using UnityEngine;

[CreateAssetMenu(fileName = "FishingData", menuName = "ScriptableObjects/Fishing/FishingData", order = 1)]
public class FishingData : ScriptableObject
{

    //cast distance multiplier
    public float CastMultiplier = 10f;
    //minimum cast distance of the rod
    public float MinCastDistance = 10f;
    //maximum cast distance of the rod
    public float MaxCastDistance = 100f;

    //reel strength multiplier
    public float ReelStrengthMultiplier = 5f;
}
