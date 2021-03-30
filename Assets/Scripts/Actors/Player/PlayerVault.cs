using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVault : MonoBehaviour
{

    public Camera playerCamera;

    //Boolean to be changed to see if player is in a trigger or not
    public bool inCollider;

    //These are to be changed to the co-ordinates of the object the player enters the trigger for
    Transform objectToBeVaulted;
    Vector3 directionToVault;

    //These are the objects for getting the size of the collider
    Collider m_Collider;
    Vector3 m_Center;
    Vector3 m_Size, m_Min, m_Max;
    CharacterController m_Controller;

    float count;


    BoxCollider m_box_Collider;


    /*
    A Vector 3 is a set of 3 numbers. X,Y,Z Example: (1, 3, 5) A Transform is a game 
    objects position, rotation, and scale in the scene, and can be seen in the inspector when you have the object selected. 
    */

    // Start is called before the first frame update
    void Start()
    {
        // Set the camera to the main camera in the scene
        playerCamera = Camera.main;
        // Set check if inside trigger to false
        inCollider = false;
        m_Controller = GetComponent<CharacterController>();        



    }

    // Update is called once per frame
    void Update()
    {
        //inCollider is already true or false
        if (Input.GetKeyDown(KeyCode.E) && inCollider)
        {
            LookingAtTrigger();
            Debug.Log("Vault key pressed");
        }


    }


    private void OnTriggerEnter(Collider collider)
    {
        //This class is to check if player is colliding with vaultable walls

        //Getting sizes of the vault wall the player collides with
        m_Collider = collider;
        m_Size = m_Collider.bounds.size;
        m_Min = m_Collider.bounds.min;
        m_Max = m_Collider.bounds.max;        
        //Fetch the center of the Collider volume
        //m_Center = m_Collider.bounds.center;

        //Getting 
        directionToVault = collider.transform.position;
        objectToBeVaulted = collider.transform;

        if (collider.gameObject.layer == LayerMask.NameToLayer("Mount"))
        {
            //Sets boolean to true confirming we are in a trigger
            inCollider = true;

            


        }
    }

    void OnTriggerExit(Collider other)
    {
        inCollider = false;
        count = 0;
    }

    void LookingAtTrigger()
    {

        directionToVault = objectToBeVaulted.position - playerCamera.transform.position;

        if (Vector3.Angle(playerCamera.transform.forward, directionToVault) < playerCamera.fieldOfView + 15)
        {
            Vault();
        }
    }

    void Vault()
    {
        Debug.Log("The player should vault now");

        //Get vector between the two objects
        while (count < m_Size.y)
        {
            m_Controller.Move(Vector3.up);
            count++;
        }
            



    }

}