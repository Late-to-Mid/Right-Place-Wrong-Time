using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    Camera playerCamera;
    public float playerSpeed = 10.0f;

    // Declare variables for motion vector
    Vector3 motion;

    // Declare variables for input
    float horizontalInput;
    float verticalInput;

    // Declare controller
    public CharacterController controller;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = Camera.main;
    }

    void Update()
    {
        // Call movement method
        Movement();
    }

    // Get movement input from player
    void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        motion = new Vector3(horizontalInput, 0, verticalInput).normalized;
        if (motion.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(motion.x, motion.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * playerSpeed * Time.deltaTime);
        }

    }
}