using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Debug = UnityEngine.Debug;
using Enums;

public class CharacterController : MonoBehaviour
{
    // GameObjects
    public GameObject player;
    public GameObject moveCursor;
    public GameObject targetCursor;
    public GameObject projectileManager;

    public LineRenderer lineRenderer;
    const int lineSegments = 20;

    public static Trace trace = new Trace();

    // Components
    Animator anim;
    Projectiles projectiles;

    // Move 관련
    Vector3 playerDestination = Vector3.zero;
    Vector3 targetDirection = Vector3.zero;

    // Status 관련
    Status mode = Status.Stopped;
    Status prevMode = Status.Stopped;

    AttackMode attackMode = AttackMode.Basic;

    void Start()
    {
        anim = player.GetComponent<Animator>();
        projectiles = projectileManager.GetComponent<Projectiles>();

        lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        if (Input.GetMouseButtonDown(1))    // RMB
        {
            playerDestination = GetRaycastHitpoint();
            playerDestination.y = 0.0f;

            if (mode == Status.Aiming)
            {
                QuitAim();
            }

            Move();
        }

        if (Input.GetMouseButtonDown(0))    // LMB
        {
            if (mode == Status.Aiming)
            {
                QuitAim();
                Fire(attackMode);
            }
        }

        if (Input.GetKeyDown("1"))
        {
            SwitchMode(Status.Aiming);
            SwitchAttackMode(AttackMode.Cannon);
        }
        
        if (Input.GetKeyDown("a"))
        {
            SwitchMode(Status.Aiming);
            SwitchAttackMode(AttackMode.Basic);
        }

        switch (mode)
        {
            case Status.Moving:

                player.GetComponent<AudioSource>().mute = false;

                // When player moved onto the point
                if (Vector3.Distance(player.transform.position, playerDestination) < 0.05f)
                {
                    Stop();
                }
                else
                {
                    Move();
                }

                break;

            case Status.Stopped:

                player.GetComponent<AudioSource>().mute = true;
                break;

            case Status.Aiming:
                Stop();
                Aim();

                break;
        }

    }

    void Stop()
    {
        anim.SetBool("isRunning", false);
        moveCursor.SetActive(false);

        SwitchMode(Status.Stopped);
    }

    void Move()
    {
        anim.SetBool("isRunning", true);

        playerDestination.y = 0.55f;
        targetDirection = playerDestination - player.transform.position;
        
        moveCursor.transform.position = playerDestination;
        moveCursor.SetActive(true);

        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * 8.0f);
        player.transform.position = Vector3.MoveTowards(player.transform.position, playerDestination, Time.deltaTime * 5.0f);

        SwitchMode(Status.Moving);
    }

    void QuitAim()
    {
        // trace.Reset(); // 왜?

        targetCursor.SetActive(false);
        lineRenderer.enabled = false;

        SwitchMode(Status.Stopped);
    }

    void Aim()
    {
        Vector3 targetPosition = GetRaycastHitpoint();
        targetPosition.y = 0.55f;

        targetCursor.transform.position = targetPosition;
        targetCursor.SetActive(true);

        player.transform.LookAt(targetPosition);


        // position to fire the projectile
        Vector3 firePosition = Vector3.MoveTowards(player.transform.position, targetCursor.transform.position, 0.5f);
        firePosition.y += 1.5f;

        if(attackMode == AttackMode.Cannon)
        {
            // 미리보기
            trace.setTrace(firePosition, targetPosition);
            trace.calculate();
            previewTrace(trace);
        }


        SwitchMode(Status.Aiming);

    }

    void Fire(AttackMode _mode)
    {
        Vector3 tFrom = player.transform.position;
        tFrom.y += 1.5f;

        Vector3 tTo = targetCursor.transform.position;

        projectiles.Instantiate(tFrom, tTo, _mode);
    }



    ///
    ///

    Vector3 GetRaycastHitpoint()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;
        Physics.Raycast(r, out h);

        return h.point;
    }

    void SwitchMode(Status modeChangeTo)
    {
        prevMode = mode;
        mode = modeChangeTo;
    }

    void SwitchAttackMode(AttackMode modeChangeTo)
    {
        attackMode = modeChangeTo;
    }


    // 궤적 미리보기
    public void previewTrace(Trace _trace)
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

        lineRenderer.positionCount = lineSegments;

        Vector3[] tPositions = new Vector3[lineSegments];

        for (int i = 0; i < lineSegments; ++i)
        {
            tPositions[i] = _trace.from + trace.GetPositionByTime(i * 0.05f);
        }

        lineRenderer.SetPositions(tPositions);
        lineRenderer.enabled = true;
    }
}
