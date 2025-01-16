using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 물리량의 묶음
public struct Force
{
    public float acceleration;
    public Vector3 initialVelocity;
    public Vector3 initialPosition;
    public Vector3 instantVelocity;
    public Vector3 currentPosition;

    public Force(Force _prev)
    {
        acceleration = _prev.acceleration;
        initialVelocity = _prev.initialVelocity;
        initialPosition = _prev.initialPosition;
        instantVelocity = _prev.instantVelocity;
        currentPosition = _prev.currentPosition;
    }

}
