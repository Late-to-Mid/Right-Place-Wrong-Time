using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public Camera playerCamera;
    public float playerSpeed = 20.0f;

    // Declare variables for motion vector
    Rigidbody m_Rigidbody;
    Vector3 motion;

    // Declare variables for input
    float horizontalInput;
    float verticalInput;

    void Start()
    {
        // Get rigidbody component of the object this script is attached to
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Call movement method
        Movement();
    }

    // Get movement input from player
    void Movement()
    {
        motion = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(motion * playerSpeed * Time.deltaTime);
    }
}