using UnityEngine;
using UnityEngine.UI;
using PlayerScripts;

public class PlayerHealthBar : HealthBar
{
    [Tooltip("Image component dispplaying current health")]
    public Image healthBarImage;

    float m_previousHealth;

    Health m_PlayerHealth;

    public Image DamageIndicator;

    private void Start()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        m_PlayerHealth = playerCharacterController.GetComponent<Health>();

        m_PlayerHealth.healthBar = this;

        m_previousHealth = m_PlayerHealth.currentHealth;
    }

    void Update()
    {
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

    public override void UpdateHealthBar(float fillAmount)
    {
        healthBarImage.fillAmount = fillAmount;
    }
}
