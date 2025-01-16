using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UnitWithSlider : UnitBase
{
    public Slider slider;

    void Start()
    {
        slider.GetComponent<Slider>().value = 1.0f;
    }

    protected override void Damage(int _damage)
    {
        base.Damage(_damage);

        slider.value = (float)currentHP / (float)totalHP;
    }
}
