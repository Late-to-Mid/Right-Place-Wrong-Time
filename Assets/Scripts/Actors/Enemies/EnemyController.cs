using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Actor))]
public class EnemyController : MonoBehaviour
{
    ActorsManager m_ActorsManager;
    Health m_Health;
    Actor m_Actor;

    // Start is called before the first frame update
    void Start()
    {
        m_ActorsManager = FindObjectOfType<ActorsManager>();

        m_Health = GetComponent<Health>();

        m_Actor = GetComponent<Actor>();

        m_Health.onDie += OnDie;
    }

    void OnDie()
    {
        Destroy(gameObject);
    }
}
