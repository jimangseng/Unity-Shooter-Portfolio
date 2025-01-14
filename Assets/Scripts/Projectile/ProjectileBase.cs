using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileBase: MonoBehaviour
{
    protected Vector3 from = Vector3.zero;
    protected Vector3 to = Vector3.zero;

    // Start is called before the first frame update
    protected void Start()
    {
        Debug.Log("Base Start()");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnCollisionEnter(Collision collision)
    {
       
    }

    public void SetFromAndTo(Vector3 _from, Vector3 _to)
    {
        Debug.Log("SetFromAndTo");

        from = _from;
        to = _to;
    }


}
