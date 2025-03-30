using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Scorer 
{
    public Dictionary<FishType, IAmMagnetic> BiggestFishDictionary;
    public static Action<float> OnScored; 
    public static Action<FishType, float> OnFishAddedToCollection;
    public Scorer()
    {
        BiggestFishDictionary = new Dictionary<FishType, IAmMagnetic>();
    }
    public void AddFishToCollectionIfBigger(IAmMagnetic fish, float score)
    {
       BiggestFishDictionary.TryGetValue(fish.GetFishType(), out IAmMagnetic currentFish ); 

        //f the fish type isnt in the dictionary, add this one
        if(currentFish == null)
        {
            BiggestFishDictionary[fish.GetFishType()] = fish; 
            OnFishAddedToCollection?.Invoke(fish.GetFishType(), score);

        }
        //only add the fish if its bigger than the current fish in the dictionary
        else if(fish.GetMass() > currentFish.GetMass())
        {
            BiggestFishDictionary[fish.GetFishType()] = fish; 
            OnFishAddedToCollection?.Invoke(fish.GetFishType(), score);
        }

        //Push something to UI to update the collection of fish
        Debug.Log("Added fish to collection: " + fish.GetFishType() + " " + fish.GetMass());
    }
    public float ScoreAllFish(List<IAmMagnetic> allFish)
    {
        float totalScore = 0f;
        foreach(var fish in allFish)
        {
            float score = fish.GetMass();//GetFishScore(fish.GetMass(), fish.GetLocalScaleMagnitude());
            AddFishToCollectionIfBigger(fish, score);
            totalScore += score;
        }
        Debug.Log("Total score: " + totalScore);
        return totalScore;
    }

}


