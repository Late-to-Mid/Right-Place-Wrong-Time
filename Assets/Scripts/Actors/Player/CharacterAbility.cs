using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


[RequireComponent(typeof(Actor))]
public class CharacterAbility : MonoBehaviour
{
    public enum AbilityState
    {
        Cooldown,
        Ready,
        Active
    }

    [Tooltip("Dummy to be placed that enemies will shoot at")]
    public GameObject playerDummyObject;
    [Tooltip("Particle trail effect for when the abiliy is active")]
    public GameObject playerTrailEffect;
    [Tooltip("Time stealth lasts")]
    [Range(0f, 10f)]
    public float timeLength = 5f;
    public float cooldown = 5f;


    float m_TimeActivated;
    float m_TimeEnded;
    public float readyBar { get; private set; }
    AbilityState m_State = AbilityState.Ready;

    [Header("Internal References (DO NOT SET)")]
    public DummyController dummyController;
    Actor m_Actor;
    ActorsManager actorsManager;
    PlayerInputHandler m_PlayerInputHandler;

    public UnityAction onAbilityUsed;
    public UnityAction onAbilityOver;

    // Start is called before the first frame update
    void Start()
    {
        m_Actor = GetComponent<Actor>();
        actorsManager = FindObjectOfType<ActorsManager>();

        m_PlayerInputHandler = GetComponent<PlayerInputHandler>();

        readyBar = 1f;
    }

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

    public void CheckToUseAbility()
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

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Cursor.lockState == CursorLockMode.Locked)
        {
            CheckToUseAbility();
        }
    }
}
