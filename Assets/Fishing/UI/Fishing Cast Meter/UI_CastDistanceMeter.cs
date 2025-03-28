using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_CastDistanceMeter : MonoBehaviour
{
    private Image _meterImage;
    void OnEnable()
    {
        Debug.Log("init on image");
        _meterImage = GetComponent<Image>();

        Fishing.OnCastDistanceChanged += UpdateMeter;
    }
    void OnDisable()
    {
        Fishing.OnCastDistanceChanged -= UpdateMeter;
    }

    void UpdateMeter(float fillAmount, float minCast, float maxCast){
        //calculate the fill amount based on the min and max cast distance
        //float fill = Mathf.InverseLerp(minCast, maxCast, fillAmount);
        Debug.Log("Fill Amount: " + fillAmount );
        float fill = fillAmount/maxCast;
        _meterImage.fillAmount = fill;
    }
}
