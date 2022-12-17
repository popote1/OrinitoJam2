using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider sliderGaz;
    public TMP_Text TxtGazText;

    public void SetGaz(float actualGaz, float maxGaz) {
        sliderGaz.value = actualGaz / maxGaz;
        TxtGazText.text = Mathf.RoundToInt(actualGaz) + "/" + Mathf.RoundToInt(maxGaz);
    }
}
