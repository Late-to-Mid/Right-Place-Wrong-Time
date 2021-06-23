using UnityEngine;
using PlayerScripts;

namespace PlayerScripts
{

}
public class CreateDummy : PlayerAbilityBase
{
    [Tooltip("Dummy to be placed that enemies will shoot at")]
    public GameObject playerDummyObject;
    [Tooltip("Particle trail effect for when the abiliy is active")]
    public GameObject playerTrailEffect;
    [Tooltip("Time stealth lasts")]
    [Range(0f, 10f)]
    public float timeLength = 5f;
    public float cooldown = 5f;



    [Header("Internal References (DO NOT SET)")]
    public DummyController dummyController;


    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case AbilityState.Ready:
                break;
            case AbilityState.Cooldown:
                OnCooldown();
                break;
            case AbilityState.Active:
                CheckToEndAbility();
                break;
        }
    }

    public override void CheckToUseAbility()
    {
        if (m_State == AbilityState.Ready)
        {
            UseAbility();
        }        
    }

    void UseAbility()
    {
        if (onAbilityUsed != null)
        {
            onAbilityUsed.Invoke();
        }

        // Set now as the time we used the ability
        m_TimeActivated = Time.time;

        // Spawn a player dummy in our place and rotation
        GameObject dummy = Instantiate(playerDummyObject, transform.position + Vector3.up, transform.rotation);
        dummyController = dummy.GetComponent<DummyController>();
        dummyController.lifeTime = timeLength;

        // Start the trail effect
        playerTrailEffect.SetActive(true);

        // Set the ability state
        m_State = AbilityState.Active;

        // Unregister ourselves as an actor so enemies will stop shooting at us
        actorsManager.UnregisterActor(m_Actor);

        // Set the readybar to zero (so we can't activate the ability again)
        readyBar = 0f;
    }

    void CheckToEndAbility()
    {
        if (Time.time > m_TimeActivated + timeLength || m_PlayerInputHandler.GetFireInputHeld() || dummyController == null)
        {
            actorsManager.RegisterActor(m_Actor);
            m_State = AbilityState.Cooldown;
            if (dummyController != null)
            {
                dummyController.Kill();
                dummyController = null;
            }

            playerTrailEffect.SetActive(false);
            
            m_TimeEnded = Time.time;

            if (onAbilityOver != null)
            {
                onAbilityOver.Invoke();
            }
        }
    }

    void OnCooldown()
    {
        readyBar = (Time.time - m_TimeEnded) / cooldown;
        if (Time.time > m_TimeEnded + cooldown)
        {
            m_State = AbilityState.Ready;
            readyBar = 1f;
        }
    }
}
