using UnityEngine;

public class FullAmmoPickup : MonoBehaviour
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
        if (currentWeapon && !currentWeapon.FullAmmo())
        {
            currentWeapon.m_CurrentAmmo = currentWeapon.maxAmmo;

            m_Pickup.PlayPickupFeedback();

            Destroy(gameObject);
        }
    }
}
