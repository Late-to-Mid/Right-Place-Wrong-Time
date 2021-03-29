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
    }

    void LookingAtTrigger()
    {

        //Transform objectThatShouldBeOutOfSight;
        //Vector3 directionToObject;

        directionToVault = objectToBeVaulted.position - playerCamera.transform.position;

        if (Vector3.Angle(playerCamera.transform.forward, directionToVault) < playerCamera.fieldOfView + 15)
        {
            Vault();
        }
    }

    void Vault()
    {
        Debug.Log("The player should vault now");
    }

}