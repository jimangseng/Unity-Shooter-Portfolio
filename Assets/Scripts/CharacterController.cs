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
    [SerializeField] GameObject player;
    [SerializeField] GameObject moveCursor;
    [SerializeField] GameObject targetCursor;
    [SerializeField] GameObject projectileManager;

    // Components
    Animator anim;
    ProjectilesManager projectiles;

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
        projectiles = projectileManager.GetComponent<ProjectilesManager>();
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
                if (Vector3.Distance(player.transform.position, playerDestination) < 0.1f)
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

        float tx = playerDestination.x - player.transform.position.x;
        float tZ = playerDestination.z - player.transform.position.z;

        targetDirection = new Vector3(tx, 0.0f, tZ);
            
        moveCursor.transform.position = playerDestination;
        moveCursor.SetActive(true);

        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * 8.0f);
        player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(playerDestination.x, 0.5f, playerDestination.z), Time.deltaTime * 5.0f);

        SwitchMode(Status.Moving);
    }

    void QuitAim()
    {
        targetCursor.SetActive(false);
        projectiles.LineRenderer.enabled = false;

        SwitchMode(Status.Stopped);
    }

    void Aim()
    {
        Vector3 targetPosition = GetRaycastHitpoint();
        targetCursor.transform.position = targetPosition;
        targetCursor.SetActive(true);

        targetPosition.y = 0.55f;
        player.transform.LookAt(targetPosition);

        // position to fire the projectile
        Vector3 firePosition = Vector3.MoveTowards(player.transform.position, targetCursor.transform.position, 0.5f);
        //firePosition.y += 1.5f;

        if(attackMode == AttackMode.Cannon)
        {
            ProjectilesManager.trace.setTrace(firePosition, targetPosition);
            ProjectilesManager.trace.calculate();
            projectiles.previewTrace();
        }


        SwitchMode(Status.Aiming);

    }

    void Fire(AttackMode _mode)
    {
        Vector3 tFrom = player.transform.position;
        tFrom.y += 1.0f;

        Vector3 tTo = targetCursor.transform.position;

        projectiles.Instantiate(tFrom, tTo, _mode);
    }

    Vector3 GetRaycastHitpoint()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;
        Physics.Raycast(r, out h);
        //Debug.Log(h.point);
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

}
