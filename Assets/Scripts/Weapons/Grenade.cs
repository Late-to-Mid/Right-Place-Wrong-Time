using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float lifetime = 3f;

    public float explosionRadius = 5f;

    public float explosionForce = 100f;

    public GameObject explosionEffect;

    float m_SpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_SpawnTime + lifetime)
        {
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody RB = nearbyObject.GetComponent<Rigidbody>();
            if (RB != null)
            {
                RB.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            if (nearbyObject.tag == "Enemy")
            {
                Health enemyHealth = nearbyObject.GetComponent<Health>();
                enemyHealth
            }
        }

        Destroy(gameObject);
    }
}
