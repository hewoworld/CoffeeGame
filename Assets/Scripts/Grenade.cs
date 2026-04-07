using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public float blastRadius = 5f;
    public float force = 700f;

    float countdown;

    bool hasExploded = false;

    public GameObject explosionEffect;
    public float damage = 5;

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if(countdown <= 0 )
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, blastRadius);
            }

            PlayerHealth health = nearbyObject.GetComponent<PlayerHealth>();
            if(health != null)
            {
                health.DealDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
