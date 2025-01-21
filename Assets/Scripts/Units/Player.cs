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

    private void Update()
    {
        if (CurrentHP < 0)
        {
            Debug.Log("You Died");
            CurrentHP = TotalHP;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Damage(collision.collider.GetComponent<Enemy>().AP);
        }
    }
}
