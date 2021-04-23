using UnityEngine;
using UnityEngine.InputSystem;

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

    bool m_FireInputWasHeld;
    public Vector3 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }

    // GameFlowManager m_GameFlowManager;
    PlayerCharacterController m_PlayerCharacterController;
    PlayerWeaponsManager m_PlayerWeaponsManager;

    // Start is called before the first frame update
    void Start()
    {
        // m_GameFlowManager = FindObjectOfType<GameFlowManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
        m_PlayerWeaponsManager = GetComponent<PlayerWeaponsManager>();
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();;
    }

    public void OnMove(InputAction.CallbackContext context) 
    {
        Vector2 moveInput2d = context.ReadValue<Vector2>();
        moveInput = new Vector3(moveInput2d.x, 0, moveInput2d.y);
    }

    public void OnCouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            m_PlayerCharacterController.isCrouching = !m_PlayerCharacterController.isCrouching;
        }
    }

    public void OnSprint(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Performed)
        {
            m_PlayerCharacterController.isSprinting = !m_PlayerCharacterController.isSprinting;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            m_PlayerCharacterController.Jump();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            m_PlayerWeaponsManager.isAiming = !m_PlayerWeaponsManager.isAiming;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
            break;

            case InputActionPhase.Performed:
            m_PlayerWeaponsManager.Fire(true, true, false);
            break;

            case InputActionPhase.Canceled:
            m_PlayerWeaponsManager.Fire(false, false, true);
            break;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        m_PlayerWeaponsManager.Reload();
    }
}
