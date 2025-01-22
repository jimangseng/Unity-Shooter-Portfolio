using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 물리량의 묶음
public struct Force
{
    public float Acceleration { get; set; }
    public Vector3 InitialVelocity { get; set; }
    public Vector3 InitialPosition { get; set; }
    public Vector3 InstantVelocity { get; set; }
    public Vector3 CurrentPosition { get; set; }

    public Force(Force _prev)
    {
        Acceleration = _prev.Acceleration;
        InitialVelocity = _prev.InitialVelocity;
        InitialPosition = _prev.InitialPosition;
        InstantVelocity = _prev.InstantVelocity;
        CurrentPosition = _prev.CurrentPosition;
    }
}
