using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Movement Options")]
    [Tooltip("Make crouching a toggle (default true)")]
    public bool crouchIsToggle = true;
    [Tooltip("Make sprinting a toggle (default false)")]
    public bool sprintIsToggle = false;
    [Tooltip("Make aiming a toggle (default true)")]
    public bool aimIsToggle = true;

    // GameFlowManager m_GameFlowManager;
    bool m_FireInputWasHeld;

    public PlayerCharacterController m_PlayerCharacterController
    {
        get => default;
        set
        {
            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // m_GameFlowManager = FindObjectOfType<GameFlowManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        m_FireInputWasHeld = GetFireInputHeld();
    }
    
    public bool GetCrouchingState(bool isCrouching)
    {
        if (crouchIsToggle)
        {
            if (GetCrouchInputDown()) { return !isCrouching; }
            else { return isCrouching; }
        }
        else
        {
            return GetCrouchInputHeld();
        }
    }

    public bool GetSprintingState(bool isSprinting)
    {
        if (sprintIsToggle)
        {
            if (GetSprintInputDown()) { return !isSprinting; }
            else { return isSprinting; }
        }
        else
        {
            return GetSprintInputHeld();
        }
    }

    public bool GetAimingState(bool isAiming)
    {
        if (aimIsToggle)
        {
            if (GetAimInputDown()) { return !isAiming; }
            else { return isAiming; }
        }
        else
        {
            return GetAimInputHeld();
        }
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked; // && !m_GameFlowManager.gameIsEnding;
    }

    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f, Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    public float GetLookInputsHorizontal()
    {
        if (invertXAxis)
        {
            return -GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal, GameConstants.k_AxisNameJoystickLookHorizontal);
        } else
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal, GameConstants.k_AxisNameJoystickLookHorizontal);
        }
    }

    public float GetLookInputsVertical()
    {
        if (invertYAxis)
        {
            return -GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical, GameConstants.k_AxisNameJoystickLookVertical);
        } else
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical, GameConstants.k_AxisNameJoystickLookVertical);
        }
    }

    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
        }

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.k_ButtonNameJump);
        }

        return false;
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && m_FireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        if (CanProcessInput())
        {
            //bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadFire1) != 0f;
            //if (isGamepad)
            //{
            //    return Input.GetAxis(GameConstants.k_ButtonNameGamepadFire1) >= triggerAxisThreshold;
            //}
            //else
            {
                return Input.GetButton(GameConstants.k_ButtonNameFire);
            }
        }

        return false;
    }

    public bool GetAimInputHeld()
    {
        if (CanProcessInput())
        {
            bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) != 0f;
            bool i = isGamepad ? (Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) > 0f) : Input.GetButton(GameConstants.k_ButtonNameAim);
            return i;
        }
    
        return false;
    }

    public bool GetAimInputDown()
    {
        if (CanProcessInput())
        {
            bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) != 0f;
            bool i = isGamepad ? (Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) > 0f) : Input.GetButtonDown(GameConstants.k_ButtonNameAim);
            return i;
        }

        return false;
    }

    public bool GetSprintInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.k_ButtonNameSprint);
        }    
        return false;
    }

    public bool GetSprintInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.k_ButtonNameSprint);
        }
        return false;
    }

    public bool GetCrouchInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.k_ButtonNameCrouch);
        }

        return false;
    }

    public bool GetCrouchInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
        }
        return false;
    }

    public bool GetVaultInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.k_ButtonNameVault);
        }
        return false;
    }

    public bool GetReloadInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.k_ButtonNameReload);
        }
        return false;
    }

    public int GetSwitchWeaponInput()
    {
        if (CanProcessInput())
        {

            bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadSwitchWeapon) != 0f;
            string axisName = isGamepad ? GameConstants.k_ButtonNameGamepadSwitchWeapon : GameConstants.k_ButtonNameSwitchWeapon;

            if (Input.GetAxis(axisName) > 0f)
                return -1;
            else if (Input.GetAxis(axisName) < 0f)
                return 1;
            else if (Input.GetAxis(GameConstants.k_ButtonNameNextWeapon) > 0f)
                return -1;
            else if (Input.GetAxis(GameConstants.k_ButtonNameNextWeapon) < 0f)
                return 1;
        }

        return 0;
    }

    public int GetSelectWeaponInput()
    {
        if (CanProcessInput())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                return 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                return 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                return 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                return 4;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                return 5;
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                return 6;
            else
                return 0;
        }

        return 0;
    }

    float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
    {
        if (CanProcessInput())
        {
            // Check if this look input is coming from the mouse
            // bool isGamepad = Input.GetAxis(stickInputName) != 0f;
            // float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);
            float i = Input.GetAxisRaw(mouseInputName);

            // apply sensitivity multiplier
            i *= lookSensitivity;

            // if (isGamepad)
            // {
            //     // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
            //     i *= Time.deltaTime;
            // }
            // else
            {
                // reduce mouse input amount to be equivalent to stick movement
                i *= 0.01f;
#if UNITY_WEBGL
                // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                i *= webglLookSensitivityMultiplier;
#endif
            }

            return i;
        }

        return 0f;
    }
}
