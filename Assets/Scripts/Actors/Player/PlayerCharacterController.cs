using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Audio source for footsteps, jump, etc...")]
    public AudioSource audioSource;

    [Header("General")]
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.05f;

    [Tooltip("Height at which the player dies instantly when falling off the map")]
    public float killHeight = -50f;

    public UnityAction<bool> onStanceChanged;

    [Header("Current Variables (DO NOT CHANGE, MONITOR ONLY)")]
    public bool isGrounded; // { get; private set; }
    public bool isDead; // { get; private set; }

    Health m_Health;
    CharacterController m_Controller;
    PlayerMovementHandler m_MovementHandler;
    PlayerWeaponsManager m_WeaponsManager;    
    public Vector3 m_GroundNormal;
    Vector3 m_CharacterVelocity;
    public float m_footstepDistanceCounter;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    void Start()
    {
        // Get the movement handler
        m_MovementHandler = GetComponent<PlayerMovementHandler>();

        // Get the character controller
        m_Controller = GetComponent<CharacterController>();
        m_Controller.enableOverlapRecovery = true;

        // Get the weapons manager, used for managing weapons
        m_WeaponsManager = GetComponent<PlayerWeaponsManager>();

        // Get the health, used for tracking health
        m_Health = GetComponent<Health>();
    }

    void Update()
    {
        // check for Y kill
        if (!isDead && transform.position.y < killHeight)
        {
            m_Health.Kill();
        }

        // set wasGrounded to the previous isGrounded
        bool wasGrounded = isGrounded;
        // set new isGrounded
        GroundCheck();

        // landing SFX
        // if (isGrounded && !wasGrounded) { audioSource.PlayOneShot(landSFX) }

        // Call movement method
        HandleCharacterMovement();
    }

    public void HandleCharacterMovement()
    {
        m_MovementHandler.RotateCharacter();

        // character movement handling
        // Crouching
        m_MovementHandler.SetCrouchingState(isCrouching, false);

        // Update the character height (but do not force it)
        UpdateCharacterHeight(false);

        if (m_InputHandler.isSprinting)
        {
            m_InputHandler.isSprinting = SetCrouchingState(false, false);
        }

        float speedModifier = m_InputHandler.isSprinting ? sprintSpeedModifier : 1f;

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

        // handle grounded movement
        if (m_CharacterController.isGrounded)
        {
            HandleGroundedMovement(worldspaceMoveInput, speedModifier);
        }
        else
        {
            HandleAirMovement(worldspaceMoveInput, speedModifier);
        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = m_CharacterController.GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = m_CharacterController.GetCapsuleTopHemisphere(m_Controller.height);

        // detect obstructions to adjust velocity accordingly
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
        }

        m_Controller.Move(characterVelocity * Time.deltaTime);
    }

    void GroundCheck()
    {
        // This function sets the isGrounded variable to true or false depending on whether the character is touching the ground.


        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_MovementHandler.m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            // if we're grounded, collect info about the ground normal vector with a downward capsule cast representing our character capsule.
            // This info is stored as a RaycastHit called hit.
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height), m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    public Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    public Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }
}