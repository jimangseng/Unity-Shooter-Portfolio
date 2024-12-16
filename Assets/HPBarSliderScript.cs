using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarSliderScript : MonoBehaviour
{
    public Slider slider;
    float sliderValue;

    private void Start()
    {
        slider.GetComponent<Slider>().value = 1.0f;
    }

}
