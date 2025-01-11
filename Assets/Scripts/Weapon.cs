using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Weapon: MonoBehaviour
{
    public enum AttackMode
    {
        Basic,
        Cannon
    }

    // 투사체 관련
    public GameObject basicObj;
    public GameObject cannonObj;

    Projectile basic;
    Cannon cannon;

    // 궤적 관련
    const int lineSegments = 20;
    public LineRenderer lineRenderer;

    Trace trace = new Trace();


    private void Start()
    {
        basic = new Projectile(basicObj);
        cannon = new Cannon(cannonObj, trace);
    }

    private void Update()
    {
        trace.update();
        cannon.update();
    }

    // 궤적 미리보기
    public void previewTrace(Vector3 _from, Vector3 _to)
    {
        trace.setTrace(_from, _to);
        trace.calculate();

        lineRenderer.positionCount = lineSegments;

        Vector3[] tPositions = new Vector3[lineSegments];

        for (int i = 0; i < lineSegments; ++i)
        {
            tPositions[i] = _from + trace.GetPositionByTime(i * 0.05f);
        }

        lineRenderer.SetPositions(tPositions);
        lineRenderer.enabled = true;
    }


    public void fire(AttackMode _attackMode, Vector3 _from, Vector3 _to)
    {
        if (_attackMode == AttackMode.Basic)
        {
            basic.fire(_from, _to);
        }

        else if (_attackMode == AttackMode.Cannon)
        {
            trace.time = 0.0f;
            trace.setTrace(_from, _to);
            cannon.fire(trace, _from);
        }
    }

}