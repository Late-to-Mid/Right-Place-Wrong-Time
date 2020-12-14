using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMove : MonoBehaviour
{
    // Set speed multiplier
    public float playerSpeed = 20.0f;

    // Declare variables for motion vector
    private Rigidbody m_Rigidbody;
    private Vector3 motion;

    // Declare variables for input
    private float horizontalInput;
    private float verticalInput;

    private void Start()
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
    private void Movement()
    {
        motion = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(motion * playerSpeed * Time.deltaTime);
    }
}