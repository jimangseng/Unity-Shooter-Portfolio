using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : UnitWithSlider
{

    private void Start()
    {
        TotalHP = 64;
        CurrentHP = 64;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Damage(collision.collider.GetComponent<Enemy>().AP);
        }
    }
}
