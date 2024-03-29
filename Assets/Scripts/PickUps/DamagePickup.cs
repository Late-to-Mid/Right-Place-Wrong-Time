﻿using UnityEngine;
using PlayerScripts;

public class DamagePickup : MonoBehaviour
{
    Pickup m_Pickup;

    // Start is called before the first frame update
    void Start()
    {
        m_Pickup = GetComponent<Pickup>();

        // Subscribe to pickup action
        m_Pickup.onPick += OnPicked;
    }

    // Update is called once per frame
    void OnPicked(PlayerCharacterController player)
    {
        PlayerWeaponsManager weaponManager = player.GetComponent<PlayerWeaponsManager>();
        WeaponController currentWeapon = weaponManager.weapon;
        if (currentWeapon)
        {
            currentWeapon.IncreaseDamage(10);

            m_Pickup.PlayPickupFeedback();

            Destroy(gameObject);
        }
    }
}
