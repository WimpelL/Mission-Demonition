using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOpt : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int spedDestroy = 22;
    void OnCollisionEnter(Collision coll) 
    {
        if(coll.gameObject.tag == "Projectile" )
        {
            Vector3 impactVelocity = coll.relativeVelocity;
            Debug.Log($"Impact velocity: {impactVelocity.magnitude} m/s");
            if(impactVelocity.magnitude > spedDestroy)
            {
                Destroy(gameObject);
            }

        }
    }
}
