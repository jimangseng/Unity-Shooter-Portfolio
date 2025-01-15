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
        UnityEngine.Debug.Log("Basic Start()");
        forceDirection = Vector3.Normalize(to - from);

        gameObject.SetActive(true);
        GetComponent<Rigidbody>().AddForce(forceDirection * 10.0f, ForceMode.Impulse);
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
            
            collider.SetActive(false);
            Destroy(collider);
        }

        if(collidee.tag == "Player")
        {
            collider.SetActive(false);
            Destroy(collider);
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
