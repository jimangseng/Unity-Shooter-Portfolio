using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        //{
        //    gameObject.SetActive(false);
        //}


        //// prevent object from colliding(?)
        //collision.gameObject.GetComponent<BoxCollider>().enabled = false;

        ////// clear missile particle system
        //collision.gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Clear(false);

        ////// stop smoke particle system
        ////particlesystem smoke = projinstance.transform.getchild(0).getchild(0).gameobject.getcomponent<particlesystem>();
        ////smoke.stop();

        ////// play explosion particle system
        //collision.gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        ////explosion.play();
    }
}
