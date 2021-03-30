using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelperMethods : MonoBehaviour
{
    // Gets the center point of the bottom hemisphere of the character controller capsule    
    public Vector3 GetCapsuleBottomHemisphere(float controller_radius)
    {
        return transform.position + (transform.up * controller_radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    public Vector3 GetCapsuleTopHemisphere(float atHeight, float controller_radius)
    {
        return transform.position + (transform.up * (atHeight - controller_radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    public bool IsNormalUnderSlopeLimit(Vector3 normal, float slopeLimit)
    {
        return Vector3.Angle(transform.up, normal) <= slopeLimit;
    }
}
