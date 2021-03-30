using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;
    [Tooltip("Max movement speed when crouching")]
    [Range(0, 1)]
    public float maxSpeedCrouchedRatio = 0.5f;
    [Tooltip("Max movement speed when not grounded")]
    public float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;
    [Range(0.1f, 1f)]

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float cameraHeightRatio = 0.9f;
    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;
    [Tooltip("Height of character when crouching")]
    public float capsuleHeightCrouching = 0.9f;
    [Tooltip("Speed of crouching transitions")]
    public float crouchingSharpness = 10f;

    public Vector3 characterVelocity; // { get; set; }
    public bool hasJumpedThisFrame { get; private set; }
    public float m_LastTimeJumped = 0f;
    public float RotationMultiplier
    {
        get
        {
            // if (m_WeaponsManager.isAiming)
            // {
            //     return aimingRotationMultiplier;
            // }

            return 1f;
        }
    }

    float m_CameraVerticalAngle = 0f;
    float m_TargetCharacterHeight;

    Camera playerCamera;
    PlayerCharacterController m_CharacterController;
    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    Actor m_Actor;

    // Start is called before the first frame update
    void Start()
    {
        // Set the camera to the main camera in the scene
        playerCamera = Camera.main;

        // Get the player character controller
        m_CharacterController = GetComponent<PlayerCharacterController>();

        // Get the character controller
        m_Controller = GetComponent<CharacterController>();
        m_Controller.enableOverlapRecovery = true;

        // Get the input handler of the character, used to handle player input
        m_InputHandler = GetComponent<PlayerInputHandler>();

        // force the crouch state to false when starting
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);

        // Get the actor, used to keep track of allies and enemies
        m_Actor = GetComponent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset variables
        hasJumpedThisFrame = false;
    }

    void RotateCharacter()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * rotationSpeed * RotationMultiplier), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * rotationSpeed * RotationMultiplier;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
        }
    }

    void HandleGroundedMovement(Vector3 worldspaceMoveInput, float speedModifier)
    {
        // calculate the desired velocity from inputs, max speed, and current slope
        Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;
        // reduce speed if crouching by crouch speed ratio
        if (m_InputHandler.isCrouching) { targetVelocity *= maxSpeedCrouchedRatio; }

        targetVelocity = m_CharacterController.GetDirectionReorientedOnSlope(targetVelocity.normalized, m_CharacterController.m_GroundNormal) * targetVelocity.magnitude;

        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
        characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

        // jumping
        if (m_CharacterController.isGrounded && m_InputHandler.GetJumpInputDown())
        {
            // force the crouch state to false
            if (SetCrouchingState(false, false))
            {
                // start by canceling out the vertical component of our velocity
                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                // then, add the jumpSpeed value upwards
                characterVelocity += Vector3.up * jumpForce;

                // play sound
                // audioSource.PlayOneShot(jumpSFX);

                // remember last time we jumped because we need to prevent snapping to ground for a short time
                m_LastTimeJumped = Time.time;
                hasJumpedThisFrame = true;

                // Force grounding to false
                m_CharacterController.isGrounded = false;
                m_CharacterController.m_GroundNormal = Vector3.up;
            }
        }

        // footsteps sound
        // float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
        // if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
        // {
        //     m_footstepDistanceCounter = 0f;
        //     audioSource.PlayOneShot(footstepSFX);
        // }

        // keep track of distance traveled for footsteps sound
        m_CharacterController.m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
    }

    void HandleAirMovement(Vector3 worldspaceMoveInput, float speedModifier)
    {
        // add air acceleration
        characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

        // limit air speed to a maximum, but only horizontally
        float verticalVelocity = characterVelocity.y;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
        characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

        // apply the gravity to the velocity
        characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
    }

    // returns false if there was an obstruction
    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        // set appropriate heights
        if (crouched)
        {
            m_TargetCharacterHeight = capsuleHeightCrouching;
        }
        else
        {
            // Detect obstructions if ignore obstructions is set to false
            if (!ignoreObstructions)
            {
                // Get all the overlapped colliders
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    m_CharacterController.GetCapsuleBottomHemisphere(),
                    m_CharacterController.GetCapsuleTopHemisphere(capsuleHeightStanding),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);

                // For each collider we're currently colliding with...
                foreach (Collider c in standingOverlaps)
                {
                    // If the collider isn't ourself
                    if (c != m_Controller)
                    {
                        // We can't stand up
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = capsuleHeightStanding;
        }
        m_InputHandler.isCrouching = crouched;
        return true;
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * cameraHeightRatio;
            m_Actor.aimPoint.transform.localPosition = m_Controller.center;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight, crouchingSharpness * Time.deltaTime);
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * m_TargetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
            m_Actor.aimPoint.transform.localPosition = m_Controller.center;
        }
    }
}
