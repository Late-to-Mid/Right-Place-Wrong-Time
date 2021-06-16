using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{
    public float damage = 50f;
    public float healAmount = 50f;
    public LayerMask hittableLayers = -1;

    Health m_Health;


    // Start is called before the first frame update
    void Start()
    {
        m_Health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Melee(InputAction.CallbackContext context)
    {
        Debug.Log("melee");
        if (context.phase == InputActionPhase.Performed)
        {
            Collider[] colliders = Physics.OverlapBox(transform.position + transform.forward * 0.5f, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, hittableLayers, QueryTriggerInteraction.Ignore);
            foreach (Collider collider in colliders) 
            {
                if (collider.GetComponent<Damageable>() != null)
                {
                    Damageable damageable = collider.GetComponent<Damageable>();
                    damageable.InflictDamage(damage, false, gameObject);
                    m_Health.Heal(healAmount);
                }
            }
        }
    }
}
