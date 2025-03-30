using UnityEngine;
using TMPro;

public class UI_LabelController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FishTypeText;
    [SerializeField] TextMeshProUGUI FishScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is createdpo
    public void SetNewData(FishType type, float score)
    {
        FishTypeText.text = type.ToString();
        FishScore.text = score.ToString("F2") + " kg";
    }
}
