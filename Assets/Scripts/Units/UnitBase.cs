using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    protected int TotalHP { get; set; }
    protected int CurrentHP { get; set; }

    private void Start()
    {
        TotalHP = 32;
        CurrentHP = 32;
    }

    protected virtual void Damage(int _damage)
    {
        CurrentHP -= _damage;
    }
}