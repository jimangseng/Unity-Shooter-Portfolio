using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Enums;

public class Projectiles : MonoBehaviour
{
    public GameObject[] projectiles;
    protected GameObject projInstance;

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
}
