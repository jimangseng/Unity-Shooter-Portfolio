using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    protected int totalHP;
    protected int currentHP;

    private void Start()
    {
        totalHP = 32;
        currentHP = 32;
    }

    protected virtual void Damage(int _damage)
    {
        currentHP -= _damage;
    }
}