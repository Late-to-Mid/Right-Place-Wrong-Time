using UnityEngine;

[RequireComponent(typeof(DamageArea))]
public class Grenade : MonoBehaviour
{
    [Header("Grenade Attributes")]
    [Tooltip("Time until explosion after throwing")]
    public float lifetime = 3f;
    [Tooltip("Radius of explosion")]
    public float explosionRadius = 5f;
    [Tooltip("Force of explosion (for knockback)")]
    public float explosionForce = 100f;
    [Tooltip("Damage value for actors")]
    public float damage = 15f;

    [Header("References")]
    [Tooltip("Layers this projectile can collide with")]
    public LayerMask hittableLayers = -1;
    [Tooltip("Visual effects for explosion")]
    public GameObject explosionEffect;
    [Tooltip("Damage Area Script")]
    public DamageArea areaOfDamage;

    float m_SpawnTime;
    const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnTime = Time.time;
    }

    // Use FixedUpdate for more accurate physics calculations as it is called
    // on a time-basis, not a frame-basis.
    void FixedUpdate()
    {
        if (Time.time > m_SpawnTime + lifetime)
        {
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        areaOfDamage.InflictDamageInArea(damage, transform.position, hittableLayers, k_TriggerInteraction, gameObject);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody RB = nearbyObject.GetComponent<Rigidbody>();
            if (RB != null)
            {
                RB.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

        }

        Destroy(gameObject);
    }
}
