using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Enums;

public class ProjectilesManager : MonoBehaviour
{
    // 
    public GameObject[] projectiles;
    protected GameObject projInstance;

    // 대포 궤적
    public static Trace trace;

    // Cannon 궤적을 그리기 위한 LineRenderer 관련
    public LineRenderer LineRenderer { get; set; }
    readonly int lineSegments = 20;

    private void Start()
    {
        trace = new Trace();

        LineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
    }

    private void Update()
    {
        
    }

    public void Instantiate(Vector3 _from, Vector3 _to, AttackMode _mode)
    {
        GameObject projectile = null;

        switch(_mode)
        {
            case AttackMode.Basic:
                projectile = projectiles[0];
                break;

            case AttackMode.Cannon:
                projectile = projectiles[1];
                break;
        }

        projInstance = Instantiate(projectile, _from, Quaternion.LookRotation(Vector3.forward), transform);
        projInstance.GetComponent<ProjectileBase>().SetFromAndTo(_from, _to);
    }

    // 궤적 미리보기
    public void previewTrace()
    {
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
