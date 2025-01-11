using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Projectile
{
    public GameObject projObject;
    protected GameObject projInstance;

    protected Vector3 forceDirection;

    public Projectile(GameObject _basicObj)
    {
        projObject = _basicObj;
    }

    public virtual void fire(Vector3 _from, Vector3 _to)
    {
        projInstance = MonoBehaviour.Instantiate(projObject, _from, Quaternion.Euler(projObject.transform.forward));
        projInstance.SetActive(true);

        forceDirection = Vector3.Normalize(_to - _from);

        projInstance.GetComponent<Rigidbody>().AddForce(forceDirection * 10.0f, ForceMode.Impulse);
    }




}
