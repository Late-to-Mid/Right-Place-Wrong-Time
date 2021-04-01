using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVault : MonoBehaviour
{

    public Camera playerCamera;

    //Boolean to be changed to see if player is in a trigger or not
    bool inCollider;
    bool isVaulting;

    //These are to be changed to the co-ordinates of the object the player enters the trigger for
    Transform objectToBeVaulted;
    Vector3 directionToVault;

    //These are the objects for getting the size of the collider
    Collider m_Collider;



    PlayerCharacterController m_Controller;
    public float vaultForce = 0; 

    // Start is called before the first frame update
    void Start()
    {
        // Set the camera to the main camera in the scene
        playerCamera = Camera.main;
        // Set check if inside trigger to false
        inCollider = false;
        m_Controller = GetComponent<PlayerCharacterController>();

        isVaulting = false;
    }

    // Update is called once per frame
    void Update()
    {
        //inCollider is already true or false
        if (Input.GetKeyDown(KeyCode.E) && inCollider)
        {
            LookingAtTrigger();
        }
        if (isVaulting)
        {
            Vault();
        }
    }

// ========================= TRIGGER ==============================
    private void OnTriggerEnter(Collider collider)
    {
        //This class is to check if player is colliding with vaultable walls

        //Getting sizes of the vault wall the player collides with
        m_Collider = collider;

        //Getting position of vaultable object
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
        isVaulting = false;
    }

//==================== END TRIGGER ===================
    void LookingAtTrigger()
    {

        directionToVault = objectToBeVaulted.position - playerCamera.transform.position;

        if (Vector3.Angle(playerCamera.transform.forward, directionToVault) < playerCamera.fieldOfView + 15)
        {
            isVaulting = true;
            Vault();
        }
    }

    void Vault()
    {
        //Get vector between the two objects

        m_Controller.m_CharacterVelocity.y = 0;
        m_Controller.isGrounded = false;
        m_Controller.m_CharacterVelocity += Vector3.up * vaultForce;



    }
}