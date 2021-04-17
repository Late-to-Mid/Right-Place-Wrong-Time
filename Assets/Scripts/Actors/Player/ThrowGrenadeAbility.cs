using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeAbility : MonoBehaviour
{
    public float throwForce = 40f;

    public GameObject grenade;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        GameObject thrown_grenade = Instantiate(grenade, transform.position + Vector3.up * 1.4f, transform.rotation);
        Rigidbody rb = thrown_grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce);
    }
}
