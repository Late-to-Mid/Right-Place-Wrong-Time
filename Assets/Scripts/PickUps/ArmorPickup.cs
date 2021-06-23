using UnityEngine;
using PlayerScripts;
public class ArmorPickup : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Amount of health to heal on pickup")]
    public int armorAmount;
    
    Pickup m_Pickup;

    void Start()
    {
        m_Pickup = GetComponent<Pickup>();

        // Subscribe to pickup action
        m_Pickup.onPick += OnPicked;
    }

    void OnPicked(PlayerCharacterController player)
    {
        Damageable playerDamagable = player.GetComponent<Damageable>();
        if (playerDamagable)
        {
            playerDamagable.increaseArmor(armorAmount);

            m_Pickup.PlayPickupFeedback();

            Destroy(gameObject);
        }
    }
}
