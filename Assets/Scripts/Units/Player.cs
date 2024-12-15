using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : UnitBase
{
    public Slider slider;

    void Damage(int _damage)
    {
        currentHP -= _damage;

        slider.value = (float)currentHP / (float)totalHP;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Enemy")
        {
            Damage(5);
        }
    }
}
