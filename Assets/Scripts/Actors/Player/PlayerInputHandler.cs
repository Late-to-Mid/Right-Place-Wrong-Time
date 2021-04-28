﻿using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;
    [Tooltip("Additional sensitivity multiplier for WebGL")]
    public float webglLookSensitivityMultiplier = 0.25f;
    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;
    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = false;
    [Tooltip("Used to flip the horizontal input axis")]
    public bool invertXAxis = false;

    // [Header("Movement Options")]
    // [Tooltip("Make crouching a toggle (default false)")]
    // public bool crouchIsToggle = false;
    // [Tooltip("Make sprinting a toggle (default false)")]
    // public bool sprintIsToggle = false;
    // [Tooltip("Make aiming a toggle (default true)")]
    // public bool aimIsToggle = true;

    bool Firing;
    bool m_FireInputWasHeld;

    // GameFlowManager m_GameFlowManager;
    PlayerWeaponsManager m_PlayerWeaponsManager;

    public UnityAction<InputAction.CallbackContext> onMenu;

    // Start is called before the first frame update
    void Start()
    {
        // m_GameFlowManager = FindObjectOfType<GameFlowManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_PlayerWeaponsManager = GetComponent<PlayerWeaponsManager>();
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        m_FireInputWasHeld = Firing;
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && CanProcessInput())
        {
            m_PlayerWeaponsManager.isAiming = !m_PlayerWeaponsManager.isAiming;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (CanProcessInput())
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Firing = true;
                break;

                case InputActionPhase.Performed:
                Firing = true;
                break;

                case InputActionPhase.Canceled:
                Firing = false;
                break;
            }
        }
    }

    public bool GetFireInputHeld()
    {
        return Firing;
    }

    public bool GetFireInputDown()
    {
        return Firing && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !Firing && m_FireInputWasHeld;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && CanProcessInput())
        {
            m_PlayerWeaponsManager.Reload();
        }
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (onMenu != null)
        {
            onMenu.Invoke(context);
        }
    }
}
