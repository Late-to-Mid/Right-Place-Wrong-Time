using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{
    public GameObject weapon;
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
        if (context.phase == InputActionPhase.Performed && Cursor.lockState == CursorLockMode.Locked)
        {
            weapon.transform.Translate(0f, 0f, 0.5f, weapon.transform);
            Collider[] colliders = Physics.OverlapBox(transform.position + transform.forward * 0.5f, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, hittableLayers, QueryTriggerInteraction.Ignore);

        }
    }
}
