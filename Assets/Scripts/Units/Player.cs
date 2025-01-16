using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : UnitWithSlider
{
    private void Start()
    {
        totalHP = 64;
        currentHP = 64;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Damage(collision.collider.GetComponent<Enemy>().ap);
        }
    }
}
