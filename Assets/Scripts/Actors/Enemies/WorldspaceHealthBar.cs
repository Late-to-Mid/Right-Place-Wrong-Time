using UnityEngine;
using UnityEngine.UI;

public class WorldspaceHealthBar : HealthBar
{
    // [Tooltip("Health component to track")]
    public Health health { get; set; }
    [Tooltip("Image component displaying health left")]
    public Image healthBarImage;
    [Tooltip("The floating healthbar pivot transform")]
    public Transform healthBarPivot;
    [Tooltip("Whether the health bar is visible when at full health or not")]
    public bool hideFullHealthBar = true;

    void Awake()
    {
        health = GetComponent<Health>();
        health.healthBar = this;
    }

    void Update()
    {
        // rotate health bar to face the camera/player
        healthBarPivot.LookAt(Camera.main.transform.position);

        // hide health bar if needed
        if (hideFullHealthBar)
            healthBarPivot.gameObject.SetActive(healthBarImage.fillAmount != 1);
    }

    public override void UpdateHealthBar(float fillAmount)
    {
        healthBarImage.fillAmount = fillAmount;
    }
}
