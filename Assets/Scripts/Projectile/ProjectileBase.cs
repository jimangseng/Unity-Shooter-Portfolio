using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileBase: MonoBehaviour
{
    protected Vector3 from = Vector3.zero;
    protected Vector3 to = Vector3.zero;

    public int Damange { get; set; } = 0;

    // Start is called before the first frame update
    protected void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFromAndTo(Vector3 _from, Vector3 _to)
    {
        from = _from;
        to = _to;
    }


}
