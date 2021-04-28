using UnityEngine;
using UnityEngine.InputSystem;

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
        m_TimeActivated = Time.time;
        GameObject dummy = Instantiate(playerDummyObject, transform.position + Vector3.up, transform.rotation);
        dummyController = dummy.GetComponent<DummyController>();
        dummyController.lifeTime = timeLength;
        m_State = AbilityState.Active;
        actorsManager.UnregisterActor(m_Actor);
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
            m_TimeEnded = Time.time;
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
