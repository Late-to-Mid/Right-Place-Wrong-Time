using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowGrenadeAbility : MonoBehaviour
{
    public float throwForce = 40f;

    float cooldown = 2.5f;
    float m_TimeLastUsed;
    public float readyBar { get; private set; }

    public GameObject grenade;

    void Start()
    {
        m_TimeLastUsed = -cooldown;
    }

    void Update()
    {
        readyBar = (Time.time - m_TimeLastUsed) / cooldown;
        readyBar = Mathf.Clamp(readyBar, 0, 1);
    }

    public void ThrowGrenade()
    {
        if (Time.time > m_TimeLastUsed + cooldown)
        {
            GameObject thrown_grenade = Instantiate(grenade, transform.position + Vector3.up * 1.4f, transform.rotation);
            Rigidbody rb = thrown_grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce);
            m_TimeLastUsed = Time.time;
        }
    }

    public void OnGadget(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ThrowGrenade();
        }
    }
}
