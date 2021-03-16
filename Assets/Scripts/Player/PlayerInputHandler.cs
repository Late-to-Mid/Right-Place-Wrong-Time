using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;

    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = false;
    [Tooltip("Used to flip the horizontal input axis")]
    public bool invertXAxis = false;

    [Header("Input Variables")]
    [Tooltip("Variables to track when movement characters are being pressed")]
    public float horizontalInput;
    public float verticalInput;

    PlayerCharacterController m_PlayerCharacterController;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            horizontalInput = Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal);
            verticalInput = Input.GetAxisRaw(GameConstants.k_AxisNameVertical);

            Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
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

    public bool GetCrouchInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
        }

        return false;
    }

    public bool GetCrouchInputReleased()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
        }

        return false;
    }

    public float GetLookInputsHorizontal()
    {
        if (invertXAxis)
        {
            return -GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        }
        else
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        }
    }

    public float GetLookInputsVertical()
    {
        if (invertYAxis)
        {
            return -GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical);
        }
        else
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical);
        }
    }

    float GetMouseLookAxis(string mouseInputName)
    {
        // Check if this look input is coming from the mouse
        float i = Input.GetAxisRaw(mouseInputName);

        // apply sensitivity multiplier
        i *= lookSensitivity;

        // reduce mouse input amount to be equivalent to stick movement
        i *= 0.01f;

        return i;
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked && true;
    }
}
