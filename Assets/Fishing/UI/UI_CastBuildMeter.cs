using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_CastBuildMeter : MonoBehaviour
{
    Image _meterImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Fishing.OnCastDistanceChanged += UpdateCastMeter;
        _meterImage = GetComponent<Image>();
        _meterImage.fillAmount = 0f; // Set the initial fill amount to 0
    }
    void UpdateCastMeter(float castDistance, float minCastDistance, float maxCastDistance)
    {
        _meterImage.fillAmount = Mathf.InverseLerp(minCastDistance, maxCastDistance, castDistance);
    }
}
