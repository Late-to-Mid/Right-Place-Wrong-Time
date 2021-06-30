using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderSpin : MonoBehaviour
{
    // Start is called before the first frame update
    public float spinSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, spinSpeed, 0, Space.Self);
    }
}
