using UnityEngine;
using UnityEngine.UI;
using PlayerScripts;
using System.Collections;


public class PlayerHealthBar : HealthBar
{
    [Tooltip("Image component dispplaying current health")]
    public Image healthBarImage;

    float m_previousHealth;

    Health m_PlayerHealth;

    public Image damageIndicator;

    void Awake()
    {
        PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

        m_PlayerHealth = playerCharacterController.GetComponent<Health>();

        m_PlayerHealth.healthBar = this;

        m_previousHealth = m_PlayerHealth.currentHealth;
    }

    void Start()
    {
        m_PlayerHealth.onDamaged += OnDamaged;
    }

    public override void UpdateHealthBar(float fillAmount)
    {
        healthBarImage.fillAmount = fillAmount;
    }

    void OnDamaged(float amt, GameObject source)
    {
        StartCoroutine(DamageIndicator(amt));
    }

    IEnumerator DamageIndicator(float amt)
    {
        damageIndicator.color = new Color(100f, 0, 0, amt/100);
        yield return new WaitForSeconds(0.25f);
        damageIndicator.color = new Color(100f, 0, 0, 0);
    }
}
