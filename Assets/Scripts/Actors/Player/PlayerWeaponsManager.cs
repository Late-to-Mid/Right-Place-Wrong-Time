using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerCharacterController), typeof(PlayerInputHandler))]
public class PlayerWeaponsManager : MonoBehaviour
{
    [Tooltip("Weapon the player will start with")]
    public WeaponController weaponPrefab;

    [Header("References")]
    [Tooltip("Secondary camera used to avoid seeing weapon go through geometries")]
    public Camera weaponCamera;
    [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
    public Transform weaponParentSocket;
    [Tooltip("Position for weapons when active")]
    public Transform defaultWeaponPosition;
    [Tooltip("Position for weapons when aiming")]
    public Transform aimingWeaponPosition;
    [Tooltip("Position for innactive weapons")]
    public Transform downWeaponPosition;

    [Header("Weapon Bob")]
    [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
    public float bobFrequency = 10f;
    [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
    public float bobSharpness = 10f;
    [Tooltip("Distance the weapon bobs when not aiming")]
    public float defaultBobAmount = 0.05f;
    [Tooltip("Distance the weapon bobs when aiming")]
    public float aimingBobAmount = 0.02f;

    [Header("Misc")]
    [Tooltip("Speed at which the aiming animation is played")]
    public float aimingAnimationSpeed = 10f;
    [Tooltip("Field of view when not aiming")]
    public float defaultFOV = 90f;
    [Tooltip("Portion of the regular FOV to apply to the weapon camera")]
    public float weaponFOVMultiplier = 1f;
    [Tooltip("Layer to set FPS weapon gameObjects to")]
    public LayerMask FPSWeaponLayer;

    public bool isAiming;
    public bool isPointingAtEnemy { get; private set; }

    PlayerInputHandler m_InputHandler;
    PlayerCharacterController m_PlayerCharacterController;
    float m_WeaponBobFactor;
    Vector3 m_WeaponMainLocalPosition;
    Vector3 m_WeaponBobLocalPosition;
    Vector3 m_WeaponRecoilLocalPosition;
    Vector3 m_AccumulatedRecoil;
    WeaponHUDManager weaponHUDManager;
    public WeaponController weapon { get; private set; }

    public UnityAction<WeaponController> onAddedWeapon;

    private void Start()
    {
        m_InputHandler = GetComponent<PlayerInputHandler>();

        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();

        weaponHUDManager = FindObjectOfType<WeaponHUDManager>();

        SetFOV(defaultFOV);

        // Add starting weapons
        AddWeapon(weaponPrefab);
    }

    private void Update()
    {
        // handle aiming down sights
        weapon.SetAnimAimParameter(isAiming, m_PlayerCharacterController.isSprinting, m_PlayerCharacterController.isSliding);

        weapon.HandleShootInputs(
            m_InputHandler.GetFireInputDown(),
            m_InputHandler.GetFireInputHeld(),
            m_InputHandler.GetFireInputReleased());

        // // Pointing at enemy handling
        // isPointingAtEnemy = false;
        // if (activeWeapon)
        // {
        //     if(Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out RaycastHit hit, 1000, -1, QueryTriggerInteraction.Ignore))
        //     {
        //         if(hit.collider.GetComponentInParent<EnemyController>())
        //         {
        //             isPointingAtEnemy = true;
        //         }
        //     }
        // }
    }


    // Update various animated features in LateUpdate because it needs to override the animated arm position
    private void LateUpdate()
    {
        UpdateWeaponAiming();
        UpdateWeaponBob();
        UpdateWeaponRecoil();

        // Set final weapon socket position based on all the combined animation influences
        weaponParentSocket.localPosition = m_WeaponMainLocalPosition + m_WeaponBobLocalPosition + m_WeaponRecoilLocalPosition;
    }

    // Sets the FOV of the main camera and the weapon camera simultaneously
    public void SetFOV(float fov)
    {
        m_PlayerCharacterController.playerCamera.fieldOfView = fov;
        weaponCamera.fieldOfView = fov * weaponFOVMultiplier;
    }

    // Updates weapon position and camera FoV for the aiming transition
    void UpdateWeaponAiming()
    {
        if (isAiming)
        {
            SetFOV(Mathf.Lerp(m_PlayerCharacterController.playerCamera.fieldOfView, weapon.aimZoomRatio * defaultFOV, aimingAnimationSpeed * Time.deltaTime));
        }
        else
        {
            SetFOV(Mathf.Lerp(m_PlayerCharacterController.playerCamera.fieldOfView, defaultFOV, aimingAnimationSpeed * Time.deltaTime));
        }
    }

    // Updates the weapon bob animation based on character speed
    void UpdateWeaponBob()
    {
        if (Time.deltaTime > 0f)
        {
            // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
            float characterMovementFactor = 0f;
            if (m_PlayerCharacterController.isGrounded && !m_PlayerCharacterController.isSliding)
            {
                characterMovementFactor = Mathf.Clamp01(m_PlayerCharacterController.m_CharacterVelocity.magnitude / (m_PlayerCharacterController.walkSpeed * m_PlayerCharacterController.sprintSpeedRatio));
            }
            m_WeaponBobFactor = Mathf.Lerp(m_WeaponBobFactor, characterMovementFactor, bobSharpness * Time.deltaTime);

            // Calculate vertical and horizontal weapon bob values based on a sine function
            float bobAmount = defaultBobAmount;
            float frequency = bobFrequency;
            float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * m_WeaponBobFactor;
            float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * m_WeaponBobFactor;

            // Apply weapon bob
            m_WeaponBobLocalPosition.x = hBobValue;
            m_WeaponBobLocalPosition.y = Mathf.Abs(vBobValue);
        }
    }

    // Updates the weapon recoil animation
    void UpdateWeaponRecoil()
    {
        // // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
        // if (m_WeaponRecoilLocalPosition.z >= m_AccumulatedRecoil.z * 0.99f)
        // {
        //     m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, m_AccumulatedRecoil, recoilSharpness * Time.deltaTime);
        // }
        // // otherwise, move recoil position to make it recover towards its resting pose
        // else
        // {
        //     m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, Vector3.zero, recoilRestitutionSharpness * Time.deltaTime);
        //     m_AccumulatedRecoil = m_WeaponRecoilLocalPosition;
        // }
    }

    // Adds a weapon to our inventory
    void AddWeapon(WeaponController weaponPrefab)
    {
        // spawn the weapon prefab as child of the weapon socket
        weapon = Instantiate(weaponPrefab, weaponParentSocket);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
        weapon.owner = gameObject;
        weapon.sourcePrefab = weaponPrefab.gameObject;
        weapon.ShowWeapon(true);

        // Assign the first person layer to the weapon
        int layerIndex = Mathf.RoundToInt(Mathf.Log(FPSWeaponLayer.value, 2)); // This function converts a layermask to a layer index
        foreach (Transform t in weapon.gameObject.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layerIndex;
        }

        weaponHUDManager.AddWeapon(weapon);
    }

    public void Reload()
    {
        weapon.Reload();
    }
}
