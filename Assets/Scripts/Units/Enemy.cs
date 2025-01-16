using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : UnitWithSlider
{
    public int ap = 10;

    private void Start()
    {
        totalHP = 15;
        currentHP = 15;
    }

    private void Update()
    {
        if(currentHP < 0 )
        {
            gameObject.SetActive(false);
            GameManager.Instance.kills++;
            GameManager.Instance.enemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Damage(collision.gameObject.GetComponent<ProjectileBase>().damage);
        }
    }
}
