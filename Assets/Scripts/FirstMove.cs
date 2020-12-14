using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMove : MonoBehaviour
{
    //Feel free to delete all this code its garbage anyway
    Vector3 upVector;
    Vector3 leftVector;
    Vector3 rightVector;
    Vector3 forVector;
    Vector3 backVector;
    Rigidbody m_Rigidbody;

    public float playerSpeed = 2.0f;

    private void Start()
    {
        upVector = new Vector3(0.0f, 1.0f, 0.0f);
        forVector = new Vector3(0.0f, 0.0f, 1.0f);
        backVector = new Vector3(0.0f, 0.0f, -1.0f);
        leftVector = new Vector3(-1.0f, 0.0f, 0.0f);
        rightVector = new Vector3(1.0f, 0.0f, 0.0f);
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            m_Rigidbody.velocity = upVector * playerSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            m_Rigidbody.velocity = leftVector * playerSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_Rigidbody.velocity = rightVector * playerSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            m_Rigidbody.velocity = forVector * playerSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_Rigidbody.velocity = backVector * playerSpeed;
        }

    }
}