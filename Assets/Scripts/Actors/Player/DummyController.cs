using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Actor))]
public class DummyController : MonoBehaviour
{
    public float lifeTime;

    float m_SpawnTime;

    Health m_Health;
    ActorsManager m_ActorsManager;

    // Start is called before the first frame update
    void Start()
    {
        m_ActorsManager = FindObjectOfType<ActorsManager>();

        m_Health = GetComponent<Health>();

        m_Health.onDie += OnDie;

        m_SpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_SpawnTime + lifeTime)
        {
            m_Health.Kill();
        }
    }

    public void Kill()
    {
        m_Health.Kill();
    }

    void OnDie()
    {
        // this will call the OnDestroy function
        Destroy(gameObject);
    }
}
