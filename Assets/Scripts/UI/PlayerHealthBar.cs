using UnityEngine;
using UnityEngine.UI;
using PlayerScripts;

public class PlayerHealthBar : MonoBehaviour
{
    [Tooltip("Image component dispplaying current health")]
    public Image healthFillImage;

    Health m_PlayerHealth;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        m_PlayerHealth = playerCharacterController.GetComponent<Health>();
    }

    void Update()
    {
        // update health bar value
        healthFillImage.fillAmount = m_PlayerHealth.currentHealth / m_PlayerHealth.maxHealth;
    }
}
