using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class BasicProjectile : ProjectileBase
{
    protected Vector3 forceDirection;

    protected void Start()
    { 
        UnityEngine.Debug.Log("Basic Start()");
        forceDirection = Vector3.Normalize(to - from);

        gameObject.SetActive(true);
        GetComponent<Rigidbody>().AddForce(forceDirection * 10.0f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }


}
