using UnityEngine;
using UnityEngine.UI;
using PlayerScripts;

public class PlayerHealthBar : MonoBehaviour
{
    [Tooltip("Image component dispplaying current health")]
    public Image healthFillImage;

    float m_previousHealth;

    Health m_PlayerHealth;

    public Image DamageIndicator;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        m_PlayerHealth = playerCharacterController.GetComponent<Health>();

        m_previousHealth = m_PlayerHealth.currentHealth;
    }

    void Update()
    {
        // update health bar value
        healthFillImage.fillAmount = m_PlayerHealth.currentHealth / m_PlayerHealth.maxHealth;

        if (m_PlayerHealth.currentHealth != m_previousHealth)
        {
            // Notify
            DamageIndicator.gameObject.SetActive(true);

            m_previousHealth = m_PlayerHealth.currentHealth;
        }
        else
        {
            DamageIndicator.gameObject.SetActive(false);
        }
    }
}
