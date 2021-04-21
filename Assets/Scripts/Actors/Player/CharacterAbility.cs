using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(PlayerInputHandler))]
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


    float m_TimeActivated;
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
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case AbilityState.Ready:
                CheckForAbilityUse();
                break;
            case AbilityState.Cooldown:
                OnCooldown();
                break;
            case AbilityState.Active:
                CheckToEndAbility();
                break;
        }
    }

    void CheckForAbilityUse()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            m_TimeActivated = Time.time;
            GameObject dummy = Instantiate(playerDummyObject, transform.position + Vector3.up, transform.rotation);
            dummyController = dummy.GetComponent<DummyController>();
            dummyController.lifeTime = timeLength;
            m_State = AbilityState.Active;
            actorsManager.UnregisterActor(m_Actor);
        }
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
        }
    }

    void OnCooldown()
    {
        if (Time.time > m_TimeActivated + timeLength)
        {
            m_State = AbilityState.Ready;
        }
    }
}
