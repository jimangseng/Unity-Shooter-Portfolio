using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Projectile
{
    public GameObject projObject;
    protected GameObject projInstance;

    protected Vector3 forceDirection;

    public Projectile(GameObject _basicObj)
    {
        projObject = _basicObj;
    }

    public virtual void fire(Vector3 _from, Vector3 _to)
    {
        projInstance = MonoBehaviour.Instantiate(projObject, _from, Quaternion.Euler(projObject.transform.forward));
        projInstance.SetActive(true);

        forceDirection = Vector3.Normalize(_to - _from);

        projInstance.GetComponent<Rigidbody>().AddForce(forceDirection * 10.0f, ForceMode.Impulse);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    //if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Obstacle"))
    //    //{
    //        // 충돌한 물체 deactivate
    //        collision.collider.gameObject.SetActive(false);
    //    //}
    //    // prevent object from colliding(?)
    //    projInstance.GetComponent<BoxCollider>().enabled = false;

    //    // clear missile particle system
    //    projInstance.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Clear(false);

    //    // stop smoke particle system
    //    ParticleSystem smoke = projInstance.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystem>();
    //    smoke.Stop();

    //    // play explosion particle system
    //    projInstance.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
    //    //explosion.Play();
    //}


}
