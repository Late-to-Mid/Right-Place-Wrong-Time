using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace PlayerScripts
{
    [RequireComponent(typeof(PlayerCharacterController), typeof(PlayerWeaponsManager))]
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

        public InputActionReference[] playActions;

        // [Header("Movement Options")]
        // [Tooltip("Make crouching a toggle (default false)")]
        // public bool crouchIsToggle = false;
        // [Tooltip("Make sprinting a toggle (default false)")]
        // public bool sprintIsToggle = false;
        // [Tooltip("Make aiming a toggle (default true)")]
        // public bool aimIsToggle = true;

        bool Firing;
        bool m_FireInputWasHeld;
        bool inMenu = false;


        // GameFlowManager m_GameFlowManager;
        PlayerCharacterController m_PlayerCharacterController;
        PlayerWeaponsManager m_PlayerWeaponsManager;
        PlayerAbilityBase m_PlayerAbility;

        public UnityAction<InputAction.CallbackContext> onMenu;

        // Start is called before the first frame update
        void Start()
        {
            // m_GameFlowManager = FindObjectOfType<GameFlowManager>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
            m_PlayerWeaponsManager = GetComponent<PlayerWeaponsManager>();
            m_PlayerAbility = GetComponent<PlayerAbilityBase>();
        }

        private void LateUpdate()
        {
            m_FireInputWasHeld = Firing;
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (CanProcessInput())
            {
                m_PlayerCharacterController.lookInput = context.ReadValue<Vector2>();
            }
            else
            {
                m_PlayerCharacterController.lookInput = new Vector2(0, 0);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (CanProcessInput())
            {
                Vector2 moveInput2d = context.ReadValue<Vector2>();
                m_PlayerCharacterController.moveInput = new Vector3(moveInput2d.x, 0, moveInput2d.y);
            }
            else
            {
                m_PlayerCharacterController.moveInput = new Vector2(0, 0);
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed && CanProcessInput())
            {
                m_PlayerCharacterController.isSprinting = !m_PlayerCharacterController.isSprinting;
            }
        }

        // public void OnCouchHold(InputAction.CallbackContext context)
        // {
        //     if (context.performed && CanProcessInput())
        //     {
        //         m_PlayerCharacterController.isCrouching = context.ReadValueAsButton();
        //     }
        // }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed && CanProcessInput())
            {
                m_PlayerCharacterController.isCrouching = !m_PlayerCharacterController.isCrouching;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && CanProcessInput())
            {
                m_PlayerCharacterController.Jump();
            }
        }
        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed && CanProcessInput())
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

        public void OnAbility(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed && Cursor.lockState == CursorLockMode.Locked)
            {
                m_PlayerAbility.CheckToUseAbility();
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
            if (context.performed && CanProcessInput())
            {
                m_PlayerWeaponsManager.Reload();
            }
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            inMenu = !inMenu;

            if (onMenu != null)
            {
                if (inMenu)
                {
                    foreach (InputActionReference ele in playActions)
                    {
                        ele.action.Disable();
                    }
                }
                else
                {
                    foreach (InputActionReference ele in playActions)
                    {
                        ele.action.Enable();
                    }
                }

                onMenu.Invoke(context);
            }
        }
    }
}