using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Enums;

public class Projectiles : MonoBehaviour
{
    public LineRenderer LineRenderer { get; set; }
    readonly int lineSegments = 20;

    public GameObject[] projectiles;
    protected GameObject projInstance;

    public static Trace trace;

    private void Start()
    {
        LineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();

        trace = new Trace();
    }

    private void Update()
    {
        
    }

    public void Instantiate(Vector3 _from, Vector3 _to, AttackMode _mode)
    {
        switch(_mode)
        {
            case AttackMode.Basic:
                projInstance = Instantiate(projectiles[0], _from, Quaternion.LookRotation(Vector3.forward));
                break;

            case AttackMode.Cannon:
                projInstance = Instantiate(projectiles[1], _from, Quaternion.LookRotation(Vector3.forward));
                break;
        }

        projInstance.GetComponent<ProjectileBase>().SetFromAndTo(_from, _to);
        projInstance.transform.SetParent(transform);
    }

    // 궤적 미리보기
    public void previewTrace()
    {
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

        LineRenderer.positionCount = lineSegments;

        Vector3[] tPositions = new Vector3[lineSegments];

        for (int i = 0; i < lineSegments; ++i)
        {
            tPositions[i] = trace.From + trace.GetPositionByTime(i * 0.05f);
        }

        LineRenderer.SetPositions(tPositions);
        LineRenderer.enabled = true;
    }
}
