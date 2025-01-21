using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class BasicProjectile : ProjectileBase
{
    protected Vector3 forceDirection;

    protected void Start()
    {
        damage = 5;

        forceDirection = Vector3.Normalize(to - from);

        gameObject.SetActive(true);
        GetComponent<Rigidbody>().AddForce(forceDirection * 15.0f, ForceMode.Impulse);

        GetComponent<AudioSource>().Play();

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidee = collision.gameObject;
        GameObject collider = gameObject;

        if(collidee.tag == "Enemy")
        {
            collider.SetActive(false);
            Destroy(collider);
        }

        if(collidee.tag == "Player" || collidee.layer == LayerMask.NameToLayer("UI"))
        {
            Physics.IgnoreCollision(collider.GetComponent<Collider>(), collidee.GetComponent<Collider>(), true);
        }

        ///// particle system
        //// clear missile particle system
        //gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Clear(false);

        //// stop smoke particle system
        //ParticleSystem smoke = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystem>();
        //smoke.Stop();

        //// play explosion particle system
        //gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        ////explosion.play();
    }


}
