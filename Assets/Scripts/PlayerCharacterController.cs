using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    Camera playerCamera;
    public float playerSpeed = 10.0f;

    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;

    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = false;
    [Tooltip("Used to flip the horizontal input axis")]
    public bool invertXAxis = false;

    float m_CameraVerticalAngle = 0f;

    // Declare variables for motion vector
    Vector3 motion;

    // Declare variables for input
    float horizontalInput;
    float verticalInput;

    void Start()
    {
        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Call movement method
        Movement();

        HandleCameraControl();
    }

    // Get movement input from player
    void Movement()
    {
        motion = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(motion * playerSpeed * Time.deltaTime);
    }

    public float GetLookInputsHorizontal()
    {
        if (invertXAxis)
        {
            return -GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        } else
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal);
        }
    }

    public float GetLookInputsVertical()
    {
        if (invertYAxis)
        {
            return -GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical);
        } else
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

    void HandleCameraControl()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (GetLookInputsHorizontal() * rotationSpeed), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += GetLookInputsVertical() * rotationSpeed;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            playerCamera.transform.localEulerAngles = new Vector3(-m_CameraVerticalAngle, 0, 0);
        }
    }
}