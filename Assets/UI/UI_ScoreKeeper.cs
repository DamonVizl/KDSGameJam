using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using System;
using System.Linq;
public class UI_ScoreKeeper : MonoBehaviour
{
    [SerializeField] GameObject _UIPrefab;
    Dictionary<FishType, UI_LabelController> _biggestFishDictionary = new Dictionary<FishType, UI_LabelController>();
// Dictionary<FishType, UI_LabelController> _UIElements = new Dictionary<FishType, UI_LabelController>();
    bool _isSetup = false;

    void Start()
    {
        Setup();
        Scorer.OnFishAddedToCollection += UpdateUI;
    }

    private void Setup()
    {
        Debug.Log("Setuping UI_ScoreKeeper");
        var BiggestFishDictionary = new Dictionary<FishType, UI_LabelController>();
        foreach (FishType fishType in Enum.GetValues(typeof(FishType)))
        {
            BiggestFishDictionary[fishType] = null; // Initialize with null
        }
        SetupUI(BiggestFishDictionary);
        Debug.Log(BiggestFishDictionary.Count);

    }
    // void Update()
    // {
    //     if(!_isSetup)
    //     {
    //         Scorer.OnFishDictionarySetup += SetupUI;
    //         _isSetup = true;
    //     }
    // }
    void UpdateUI(FishType fishType, float score)
    {
        _biggestFishDictionary[fishType].SetNewData(fishType, score);
    }

    void SetupUI(Dictionary<FishType, UI_LabelController> biggestFishDictionary)
    {
        Debug.Log("Setting up UI with biggest fish dictionary: " + biggestFishDictionary.Count);
        _biggestFishDictionary = biggestFishDictionary;
        for(int i= 0; i<biggestFishDictionary.Count; i++)
        {
            GameObject ui = Instantiate(_UIPrefab.gameObject, transform);
            var UIElement = ui.GetComponent<UI_LabelController>();
            _biggestFishDictionary[biggestFishDictionary.ElementAt(i).Key] = UIElement;

            UIElement.SetNewData(biggestFishDictionary.ElementAt(i).Key, 0);
            //ui.GetComponent<UI_Score>().Setup(fish.Key, fish.Value.GetMass(), fish.Value.GetPosition());
        }
    }
}
