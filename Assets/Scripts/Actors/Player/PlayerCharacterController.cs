using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Height at which the player dies instantly when falling off the map")]
    public float killHeight = -50f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.06f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float walkSpeed = 8f;
    [Tooltip("Max movement speed when crouching")]
    [Range(0, 1)]
    public float crouchedSpeedRatio = 0.6f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    [Range(1, 2)]
    public float sprintSpeedRatio = 1.5f;
    [Tooltip("Max movement speed when not grounded")]
    public float airSpeed = 8f;
    [Tooltip("Minimum speed player must be going in order to slide")]
    public float requiredSpeedForSliding = 10f;
    [Tooltip("Speed the player stops sliding at (and begins to crouch-walk at)")]
    public float slideSpeedMinimum = 4f;

    [Header("Acceleration")]
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float accelerationSpeedOnGround = 20f;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 15f;
    [Tooltip("Sliding deceleration value. Lower value means slower deleceration")]
    public float slidingDeceleration = 1.25f;

    [Header("Force")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;
    [Tooltip("Force applied downward when vaulting")]
    public float vaultForce = 7.5f;
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 25f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 1f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float aimingRotationMultiplier = 0.4f;

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float cameraHeightRatio = 0.9f;
    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;
    [Tooltip("Height of character when crouching")]
    public float capsuleHeightCrouching = 0.9f;
    [Tooltip("Speed of crouching transitions")]
    public float crouchingSharpness = 10f;

    [Header("Current Variables (DO NOT CHANGE, MONITOR ONLY)")]
    public Vector3 m_CharacterVelocity;
    public float horizontalCharacterVelocity;
    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;
    public bool isSliding;
    public bool isDead;
    public bool inCollider;
    public bool isVaulting;
    float m_LastTimeJumped = 0f;
    public UnityAction<bool, bool> onStanceChanged;
    float RotationMultiplier
    {
        get
        {
            if (m_PlayerWeaponsManager.isAiming)
            {
                return aimingRotationMultiplier;
            }

            return 1f;
        }
    }
    float m_CameraVerticalAngle = 0f;
    float m_TargetCharacterHeight;
    Vector3 m_GroundNormal;
    float m_footstepDistanceCounter;
    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;
    Collider colliderToVault;


    [Header("References")]
    [Tooltip("Sound played when jumping")]
    public AudioClip jumpSFX;

    public Camera playerCamera;
    AudioSource audioSource;
    CharacterController m_Controller;
    PlayerWeaponsManager m_PlayerWeaponsManager;
    PlayerInputHandler m_PlayerInputHandler;
    Actor m_Actor;
    Health m_Health;
    CharacterAbility m_CharacterAbility;
    ThrowGrenadeAbility m_ThrowGrenade;

    void Start()
    {
        // Set the character controller
        m_Controller = GetComponent<CharacterController>();
        m_Controller.enableOverlapRecovery = true;

        // Get the input handler
        m_PlayerInputHandler = GetComponent<PlayerInputHandler>();

        // Set the weapons manager, used for managing weapons
        m_PlayerWeaponsManager = GetComponent<PlayerWeaponsManager>();

        // Set actor
        m_Actor = GetComponent<Actor>();

        // Set the health, used for tracking health
        m_Health = GetComponent<Health>();

        // Subscribe to OnDie
        m_Health.onDie += OnDie;

        m_CharacterAbility = GetComponent<CharacterAbility>();
        m_ThrowGrenade = GetComponent<ThrowGrenadeAbility>();

        audioSource = GetComponent<AudioSource>();

        // force the crouch state to false when starting
        // If this is commented out, the sliding bug occurs.
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);
    }

    void Update()
    {
        // check for Y kill
        if (!isDead && transform.position.y < killHeight)
        {
            m_Health.Kill();
        }

        Move();
        Look();
    }

    void Move()
    {
        // Ground the player
        HandleGrounding();

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 moveInput = m_PlayerInputHandler.moveInput;

        if (moveInput.z <= 0f) { isSprinting = false; }

        Vector3 worldspaceMoveInput = transform.TransformVector(moveInput);

        horizontalCharacterVelocity = Vector3.ProjectOnPlane(m_CharacterVelocity, Vector3.up).magnitude;

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);

        // Adjust speed modifier depending on whether or not the player is sprinting.
        float speedModifier = (isSprinting && !isCrouching) ? sprintSpeedRatio : 1f;

        // handle grounded movement
        if (isGrounded)
        {
            HandleGroundedMovement(worldspaceMoveInput, speedModifier);
        }
        else
        // handle air movement
        {
            // Stop sliding (in case we were)
            isSliding = false;
            HandleAirMovement(worldspaceMoveInput, speedModifier);
        }

        // detect obstructions to adjust velocity accordingly
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, m_CharacterVelocity.normalized, out RaycastHit hit, m_CharacterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            m_CharacterVelocity = Vector3.ProjectOnPlane(m_CharacterVelocity, hit.normal);
        }

        m_Controller.Move(m_CharacterVelocity * Time.deltaTime);
    }

    void Look()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (m_PlayerInputHandler.lookInput.x * rotationSpeed * RotationMultiplier), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += -m_PlayerInputHandler.lookInput.y * rotationSpeed * RotationMultiplier;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
        }
    }

    void HandleGrounding()
    {
        // set wasGrounded to the previous isGrounded
        bool wasGrounded = isGrounded;

        // reset values before the ground check
        isGrounded = false;
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            // set new isGrounded
            SnapToGround(wasGrounded);
        }

        // landing SFX
        // if (isGrounded && !wasGrounded) { audioSource.PlayOneShot(landSFX) }
    }

    void SnapToGround(bool wasGrounded)
    {
        // This function sets the isGrounded variable to true or false depending on whether the character is touching the ground.
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = wasGrounded ? groundCheckDistance : k_GroundCheckDistanceInAir;

        if (Physics.CapsuleCast(
            GetCapsuleBottomHemisphere(),
            GetCapsuleTopHemisphere(m_Controller.height),
            m_Controller.radius,
            Vector3.down,
            out RaycastHit hit,
            chosenGroundCheckDistance,
            groundCheckLayers,
            QueryTriggerInteraction.Ignore))
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

    void HandleGroundedMovement(Vector3 worldspaceMoveInput, float speedModifier)
    {
        // Set sliding
        if (m_CharacterVelocity.magnitude > requiredSpeedForSliding && isCrouching)
        {
            isSliding = true;
        }
        else if (isSliding && 
                (m_CharacterVelocity.magnitude < slideSpeedMinimum || !isCrouching))
        {
            isSliding = false;
        }

        Vector3 targetVelocity = new Vector3(0, 0, 0);
        float accelerationRate;
        if (isSliding)
        {
            accelerationRate = slidingDeceleration;
        }
        else
        {
            targetVelocity = worldspaceMoveInput * walkSpeed * speedModifier;
            accelerationRate = accelerationSpeedOnGround;
        }

        // calculate the desired velocity from inputs, max speed, and current slope
        // reduce speed if crouching by crouch speed ratio
        if (isCrouching) { targetVelocity *= crouchedSpeedRatio; }

        targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) * targetVelocity.magnitude;

        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
        m_CharacterVelocity = Vector3.Lerp(m_CharacterVelocity, targetVelocity, accelerationRate * Time.deltaTime);

        // Update the character height (but do not force it)
        SetCrouchingState(isCrouching, false);
        UpdateCharacterHeight(false);

        // Set UnityEvent for stance change (for hud)
        if (onStanceChanged != null)
        {
            onStanceChanged.Invoke(isCrouching, isSprinting);
        }


        // footsteps sound
        // float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
        // if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
        // {
        //     m_footstepDistanceCounter = 0f;
        //     audioSource.PlayOneShot(footstepSFX);
        // }

        // keep track of distance traveled for footsteps sound
        // m_footstepDistanceCounter += m_CharacterVelocity.magnitude * Time.deltaTime;
    }

    void HandleAirMovement(Vector3 worldspaceMoveInput, float speedModifier)
    {
        isVaulting = CheckForVaulting(isVaulting, worldspaceMoveInput);
        if (isVaulting)
        {
            m_CharacterVelocity.y = vaultForce;
        }

        // add air acceleration
        m_CharacterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

        // limit air speed to a maximum, but only horizontally
        float verticalVelocity = m_CharacterVelocity.y;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(m_CharacterVelocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, airSpeed * speedModifier);
        m_CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

        // apply the gravity to the velocity
        m_CharacterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
    }

    bool CheckForVaulting(bool isVaulting, Vector3 worldspaceMoveInput)
    {
        if (isVaulting) { return true; }
        
        if (inCollider)
        {
            Vector3 directionToVault = colliderToVault.transform.position - transform.position;
            // Check that we're looking at and moving toward the wall
            if (Vector3.Angle(playerCamera.transform.forward, directionToVault) < playerCamera.fieldOfView + 15 &&
                Vector3.Dot(directionToVault, worldspaceMoveInput) > 0f)
            {
                return true;
            }
        }
        return false;
    }

    public void Jump()
    {
        // jumping
        if (isGrounded)
        {
            isCrouching = false;

            // start by canceling out the vertical component of our velocity
            m_CharacterVelocity = new Vector3(m_CharacterVelocity.x, 0f, m_CharacterVelocity.z);

            // then, add the jumpSpeed value upwards
            m_CharacterVelocity += Vector3.up * jumpForce;

            // play sound
            audioSource.PlayOneShot(jumpSFX);

            // remember last time we jumped because we need to prevent snapping to ground for a short time
            m_LastTimeJumped = Time.time;

            // Force grounding to false
            isGrounded = false;
            m_GroundNormal = Vector3.up;
        }
    }

    void OnDie()
    {
        isDead = true;
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
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(capsuleHeightStanding),
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

    public void UseAbility()
    {
        m_CharacterAbility.CheckToUseAbility();
    }

    public void UseGadget()
    {
        m_ThrowGrenade.ThrowGrenade();
    }

    void OnTriggerEnter(Collider collider)
    {
        //This method is to check if player is colliding with vaultable walls

        //Getting sizes of the vault wall the player collides with
        colliderToVault = collider;

        if (collider.gameObject.layer == LayerMask.NameToLayer("Mount"))
        {
            //Sets boolean to true confirming we are in a trigger
            inCollider = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isVaulting)
        {
            m_CharacterVelocity.y = 2f;
            m_CharacterVelocity += transform.forward * 3f;
        }
        inCollider = false;
        isVaulting = false;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
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
