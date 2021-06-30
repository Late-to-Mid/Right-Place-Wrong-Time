using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderSpin : MonoBehaviour
{
    // Start is called before the first frame update
    public float spinSpeed = 10f;

    [Tooltip("Cylinder Object to be spun")]
    public GameObject Cylinder;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CylinderSpinEvent()
    {
        Debug.Log("hope");
        StartCoroutine("SpinCylinder");
    }

    public IEnumerator SpinCylinder()
    {
        for (int i = 0; i <= 60; i++)
        {
            Cylinder.transform.Rotate(0, spinSpeed, 0, Space.Self);
            yield return null;
        }
    }
}
