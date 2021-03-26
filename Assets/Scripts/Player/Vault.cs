using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vault : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        


    }


    private void OnTriggerEnter(Collider collider)
    {
        //This class is to check if player is colliding with vaultable walls

        if (collider.gameObject.layer == LayerMask.NameToLayer("Mount"))
        {
            Debug.Log("Collided with vault wall");
        }


    }
}
