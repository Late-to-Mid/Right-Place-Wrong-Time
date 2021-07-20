using UnityEngine;
using UnityEngine.Events;

namespace PlayerScripts
{
    [RequireComponent(typeof(PlayerCharacterController), typeof(PlayerInputHandler))]
    public class PlayerWeaponsManager : MonoBehaviour
    {
        [Tooltip("Weapon the player will start with")]
        public WeaponController weapon;

        [Header("References")]
        [Tooltip("Secondary camera used to avoid seeing weapon go through geometries")]
        public Camera weaponCamera;
        [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        public Transform weaponParentSocket;

        [Header("Weapon Bob")]
        [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
        public float bobFrequency = 10f;
        [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
        public float bobSharpness = 10f;
        [Tooltip("Distance the weapon bobs when not aiming")]
        public float defaultBobAmount = 0.02f;
        [Tooltip("Distance the weapon bobs when aiming")]
        public float aimingBobAmount = 0.002f;

        [Header("Misc")]
        [Tooltip("Speed at which the aiming animation is played")]
        public float aimingAnimationSpeed = 10f;
        [Tooltip("Field of view when not aiming")]
        public float defaultFOV = 90f;
        [Tooltip("Portion of the regular FOV to apply to the weapon camera")]
        public float weaponFOVMultiplier = 0.66f;
        [Tooltip("Layer to set FPS weapon gameObjects to")]
        public LayerMask FPSWeaponLayer;
        [Tooltip("Angle amount for recoil to move")]
        public float recoilAngle = 5f;

        public GameObject crosshair;
        CanvasGroup crosshairCanvasGroup;

        public bool isAiming;

        PlayerInputHandler m_InputHandler;
        PlayerCharacterController m_PlayerCharacterController;
        float m_WeaponBobFactor;
        Vector3 m_WeaponMainLocalPosition;
        Vector3 m_WeaponBobLocalPosition;
        Vector3 m_WeaponRecoilLocalPosition;

        public UnityAction<WeaponController> onAddedWeapon;

        private void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();

            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();

            SetFOV(defaultFOV);

            int layerIndex = Mathf.RoundToInt(Mathf.Log(FPSWeaponLayer.value, 2));
            weapon.gameObject.layer = layerIndex;

            WeaponHUDManager weaponHUDManager = FindObjectOfType<WeaponHUDManager>();
            weaponHUDManager.AddWeapon(weapon);

            // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
            weapon.owner = gameObject;

            crosshairCanvasGroup = crosshair.GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            // handle aiming down sights as well as other animator parameters
            weapon.SetAnimAimParameter(isAiming, 
                m_PlayerCharacterController.isSprinting, 
                m_PlayerCharacterController.isSliding, 
                !m_PlayerCharacterController.isGrounded, 
                m_PlayerCharacterController.isCrouching,
                m_PlayerCharacterController.isMoving
                );

            bool didShoot = weapon.HandleShootInputs(
                m_InputHandler.GetFireInputDown(),
                m_InputHandler.GetFireInputHeld(),
                m_InputHandler.GetFireInputReleased());
        }


        // Update various animated features in LateUpdate because it needs to override the animated arm position
        private void LateUpdate()
        {
            UpdateWeaponAiming();
            UpdateWeaponBob();

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
                crosshairCanvasGroup.alpha = 0f;
            }
            else
            {
                SetFOV(Mathf.Lerp(m_PlayerCharacterController.playerCamera.fieldOfView, defaultFOV, aimingAnimationSpeed * Time.deltaTime));
                crosshairCanvasGroup.alpha = 1f;
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
                float bobAmount = isAiming? aimingBobAmount : defaultBobAmount;
                float frequency = bobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * m_WeaponBobFactor;
                float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * m_WeaponBobFactor;

                // Apply weapon bob
                m_WeaponBobLocalPosition.x = hBobValue;
                m_WeaponBobLocalPosition.y = Mathf.Abs(vBobValue);
            }
        }

        // Adds a weapon to our inventory
        // void AddWeapon(WeaponController weaponPrefab)
        // {
        //     // spawn the weapon prefab as child of the weapon socket
        //     weapon = Instantiate(weaponPrefab, weaponParentSocket);
        //     weapon.transform.localPosition = Vector3.zero;
        //     weapon.transform.localRotation = Quaternion.identity;

        //     // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
        //     weapon.owner = gameObject;
        //     weapon.sourcePrefab = weaponPrefab.gameObject;
        //     weapon.ShowWeapon(true);

        //     // Assign the first person layer to the weapon
        //     int layerIndex = Mathf.RoundToInt(Mathf.Log(FPSWeaponLayer.value, 2)); // This function converts a layermask to a layer index
        //     foreach (Transform t in weapon.gameObject.GetComponentsInChildren<Transform>(true))
        //     {
        //         t.gameObject.layer = layerIndex;
        //     }

        //     WeaponHUDManager weaponHUDManager = FindObjectOfType<WeaponHUDManager>();
        //     weaponHUDManager.AddWeapon(weapon);
        // }

        public void Reload()
        {
            weapon.Reload();
        }

        public void AttachSight()
        {
            Debug.Log("Attaching sight!");
            float sightCameraOffset = weapon.AttachSight();
            weaponCamera.transform.Translate(new Vector3(0f, sightCameraOffset, 0f), Space.World);
        }
    }
}