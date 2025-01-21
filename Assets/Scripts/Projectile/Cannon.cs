using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Cannon : ProjectileBase
{
    Trace trace;

    public Vector3 position = Vector3.zero;
    Vector3 playerPosition = Vector3.zero;

    protected void Start()
    {
        Damange = 15;

        trace = new Trace(ProjectilesManager.trace);
        trace.Reset();

        playerPosition = from;

        gameObject.SetActive(true);

    }

    public void Update()
    {
        trace.update();

        if (gameObject != null)
        {
            position = trace.Position + playerPosition;
            gameObject.transform.position = position;
        }

    }
}
