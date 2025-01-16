using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Cannon : ProjectileBase
{

    Trace trace;

    public Vector3 position = Vector3.zero;
    Vector3 playerPosition = Vector3.zero;


    protected void Start()
    {
        this.trace = new Trace(CharacterController.trace);

        trace.Reset();

        playerPosition = from;

        gameObject.SetActive(true);

    }

    public void Update()
    {
        trace.update();

        if (gameObject != null)
        {
            position = trace.Position + playerPosition;
            gameObject.transform.position = position;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidee = collision.gameObject;
        GameObject collider = gameObject;

        if(collidee.tag == "Enemy")
        {
            collidee.SetActive(false);
            GameManager.Instance.enemies.Remove(collidee);
            Destroy(collidee);
        }

    }

    private void OnDestroy()
    {

    }




}
