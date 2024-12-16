using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Cannon
{
    public GameObject projObject;
    public GameObject projInstance;

    Trace trace;

    public Vector3 position = Vector3.zero;

    Vector3 playerPosition = Vector3.zero;

    public Cannon(GameObject _projObject, Trace _trace)
    {
        projObject = _projObject;
        trace = _trace;
    }

    public void update()
    {
        position = trace.Position + playerPosition;

        if (projInstance != null)
        {

            projInstance.transform.position = position;
        }
    }

    public void fire(Trace _trace, Vector3 _from)
    {
        trace = _trace;

        playerPosition = _from;

        projInstance = MonoBehaviour.Instantiate(projObject, _from, Quaternion.Euler(projObject.transform.forward));
        projInstance.SetActive(true);

    }



           
}
