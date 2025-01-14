using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Cannon : ProjectileBase
{
    Trace trace = null;

    public Vector3 position = Vector3.zero;
    Vector3 playerPosition = Vector3.zero;

    // 궤적 관련
    const int lineSegments = 20;
    public LineRenderer lineRenderer;


    protected void Start()
    {
        UnityEngine.Debug.Log("Cannon Start()");

        trace = new Trace();

        trace.time = 0.0f;


        trace.setTrace(from, to);
        trace.calculate();

        playerPosition = from;

        gameObject.SetActive(true);

    }

    public void Update()
    {
        position = trace.Position + playerPosition;

        if (gameObject != null)
        {

            gameObject.transform.position = position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    // 궤적 미리보기
    public void previewTrace(Vector3 _from, Vector3 _to)
    {
        //미리보기

        //if (Input.GetKey("q"))
        //{
        //    // 발사각 상승
        //    Debug.Log("발사각 상승");
        //}
        //else if (Input.GetKey("e"))
        //{
        //    // 발사각 하강
        //    Debug.Log("발사각 하강");
        //}


        lineRenderer.positionCount = lineSegments;

        Vector3[] tPositions = new Vector3[lineSegments];

        for (int i = 0; i < lineSegments; ++i)
        {
            tPositions[i] = _from + trace.GetPositionByTime(i * 0.05f);
        }

        lineRenderer.SetPositions(tPositions);
        lineRenderer.enabled = true;
    }


}
