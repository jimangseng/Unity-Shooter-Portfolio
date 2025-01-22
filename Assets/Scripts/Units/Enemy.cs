using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : UnitWithSlider
{
    public int AP { get; set; } = 10;

    private void Start()
    {
        TotalHP = 15;
        CurrentHP = 15;
    }

    private void Update()
    {
        if(CurrentHP < 0 )
        {
            gameObject.SetActive(false);
            GameManager.Instance.Kills++;
            GameManager.Instance.Enemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Damage(collision.gameObject.GetComponent<ProjectileBase>().Damange);
        }
    }
}
