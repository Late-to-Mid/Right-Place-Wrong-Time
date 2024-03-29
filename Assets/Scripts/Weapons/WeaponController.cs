﻿using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using PlayerScripts;

public enum WeaponShootType
{
    Manual,
    Automatic,
    Charge,
}

public enum WeaponReloadType
{
    Bullet,
    Clip,
    Charge,
}

[System.Serializable]
public struct CrosshairData
{
    [Tooltip("The image that will be used for this weapon's crosshair")]
    public Sprite crosshairSprite;
    [Tooltip("The size of the crosshair image")]
    public int crosshairSize;
    [Tooltip("The color of the crosshair image")]
    public Color crosshairColor;
}

[RequireComponent(typeof(AudioSource))]
public class WeaponController : MonoBehaviour
{
    [Header("Information")]
    [Tooltip("The name that will be displayed in the UI for this weapon")]
    public string weaponName;
    [Tooltip("The image that will be displayed in the UI for this weapon")]
    public Sprite weaponIcon;

    [Tooltip("Default data for the crosshair")]
    public CrosshairData crosshairDataDefault;
    [Tooltip("Data for the crosshair when targeting an enemy")]
    public CrosshairData crosshairDataTargetInSight;

    [Header("Internal References")]
    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject weaponRoot;
    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform weaponMuzzle;

    [Header("Shoot Parameters")]
    [Tooltip("The type of weapon wil affect how it shoots")]
    public WeaponShootType shootType;
    [Tooltip("How the weapon will reload")]
    public WeaponReloadType reloadType;
    [Tooltip("The projectile prefab")]
    public ProjectileBase projectilePrefab;
    [Tooltip("Minimum duration between two shots")]
    public float delayBetweenShots = 0.5f;
    [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
    public float bulletSpreadAngle = 0f;
    [Tooltip("Amount of bullets per shot")]
    public int bulletsPerShot = 1;
    [Tooltip("Force that will push back the weapon after each shot")]
    [Range(0f, 2f)]
    public float recoilForce = 1;
    [Tooltip("Angle for camera to move after shot")]
    public float recoilAngle = 3f;
    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
    [Range(0f, 1f)]
    public float aimZoomRatio = 1f;
    [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
    public Vector3 aimOffset;

    [Header("Ammo Parameters")]
    [Tooltip("Amount of ammo reloaded per second")]
    public float ammoReloadRate = 1f;
    [Tooltip("Delay after the last shot before starting to reload")]
    public float ammoReloadDelay = 0f;
    [Tooltip("Maximum amount of ammo in the gun")]
    public float maxAmmo = 8;

    [Header("Charging parameters (charging weapons only)")]
    [Tooltip("Trigger a shot when maximum charge is reached")]
    public bool automaticReleaseOnCharged;
    [Tooltip("Duration to reach maximum charge")]
    public float maxChargeDuration = 2f;
    [Tooltip("Initial ammo used when starting to charge")]
    public float ammoUsedOnStartCharge = 1f;
    [Tooltip("Additional ammo used when charge reaches its maximum")]
    public float ammoUsageRateWhileCharging = 1f;

    [Header("Audio & Visual")]
    [Tooltip("Optional weapon animator for OnShoot animations")]
    public Animator weaponAnimator;
    [Tooltip("Prefab of the muzzle flash")]
    public GameObject muzzleFlashPrefab;
    [Tooltip("Unparent the muzzle flash instance on spawn")]
    public bool unparentMuzzleFlash;
    [Tooltip("sound played when shooting")]
    public AudioClip shootSFX;
    [Tooltip("Sound played when changing to this weapon")]
    public AudioClip changeWeaponSFX;
    [Tooltip("Amount the weapon camera needs move up vertically to move when attaching sight")]
    public float sightCameraOffset = 0f;

    public GameObject sightParentObject;
    public Vector3 sightPositionOffset = new Vector3(0, 0, 0);
    public GameObject sightPrefab;
    public GameObject ironSight;

    [Tooltip("Continuous Shooting Sound")]
    public bool useContinuousShootSound = false;
    public AudioClip continuousShootStartSFX;
    public AudioClip continuousShootLoopSFX;
    public AudioClip continuousShootEndSFX;
    private AudioSource m_continuousShootAudioSource = null;
    private bool m_wantsToShoot = false;
    private bool m_wantsToReload = false;

    public UnityAction onShoot;
    public event Action OnShootProcessed;

    public float m_CurrentAmmo;
    float m_LastTimeShot = Mathf.NegativeInfinity;
    public float LastChargeTriggerTimestamp { get; private set; }
    Vector3 m_LastMuzzlePosition;

    public GameObject owner { get; set; }
    public GameObject sourcePrefab { get; set; }
    public bool isCharging { get; private set; }
    public float currentAmmoRatio; // { get; private set; }
    public bool isWeaponActive { get; private set; }
    public bool isReloading { get; private set; }
    public float currentCharge { get; private set; }
    public Vector3 muzzleWorldVelocity { get; private set; }
    public float GetAmmoNeededToShoot() => (shootType != WeaponShootType.Charge ? 1f : Mathf.Max(1f, ammoUsedOnStartCharge)) / (maxAmmo * bulletsPerShot);

    AudioSource m_ShootAudioSource;

    // Animator parameters
    const string k_AnimAttackParameter = "Attack";
    const string k_AnimReloadParameter = "Reload";
    const string k_AnimAimParameter = "Aiming";
    const string k_AnimSprintParameter = "Sprinting";
    const string k_AnimSlideParameter = "Sliding";
    const string k_AnimAirParameter = "Air";
    const string k_AnimCrouchParameter = "Crouch";
    const string k_AnimMeleeParameter = "Melee";
    const string k_AnimMoveParameter = "Moving";

    public bool FullAmmo() { return (currentAmmoRatio >= 1); }

    void Awake()
    {
        m_CurrentAmmo = maxAmmo;
        m_LastMuzzlePosition = weaponMuzzle.position;

        //m_ShootAudioSource = GetComponent<AudioSource>();

        //if (useContinuousShootSound)
        //{
        //    m_continuousShootAudioSource = gameObject.AddComponent<AudioSource>();
        //    m_continuousShootAudioSource.playOnAwake = false;
        //    m_continuousShootAudioSource.clip = continuousShootLoopSFX;
        //    m_continuousShootAudioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponShoot);
        //    m_continuousShootAudioSource.loop = true;
        //}
    }

    void Update()
    {
        UpdateAmmo();
        UpdateCharge();
        UpdateContinuousShootSound();

        if (Time.deltaTime > 0)
        {
            muzzleWorldVelocity = (weaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
            m_LastMuzzlePosition = weaponMuzzle.position;
        }
    }

    void UpdateAmmo()
    {
        if (isReloading || m_wantsToReload)
        {
            // reloads weapon over time
            m_CurrentAmmo += ammoReloadRate * Time.deltaTime;

            // limits ammo to max value
            if (m_CurrentAmmo >= maxAmmo) 
            {
                isReloading = false;
                m_wantsToReload = false;
            }
            else
            {
                isReloading = true;
            }
            m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo, 0, maxAmmo);
        }
        else
        {
            isReloading = false;
        }

        if (maxAmmo == Mathf.Infinity)
        {
            currentAmmoRatio = 1f;
        }
        else
        {
            currentAmmoRatio = m_CurrentAmmo / maxAmmo;
        }
    }

    void UpdateCharge()
    {
        if (isCharging)
        {
            if (currentCharge < 1f)
            {
                float chargeLeft = 1f - currentCharge;

                // Calculate how much charge ratio to add this frame
                float chargeAdded = 0f;
                if (maxChargeDuration <= 0f)
                {
                    chargeAdded = chargeLeft;
                }
                else
                {
                    chargeAdded = (1f / maxChargeDuration) * Time.deltaTime;
                }

                chargeAdded = Mathf.Clamp(chargeAdded, 0f, chargeLeft);

                // See if we can actually add this charge
                float ammoThisChargeWouldRequire = chargeAdded * ammoUsageRateWhileCharging;
                if (ammoThisChargeWouldRequire <= m_CurrentAmmo)
                {
                    // Use ammo based on charge added
                    UseAmmo(ammoThisChargeWouldRequire);

                    // set current charge ratio
                    currentCharge = Mathf.Clamp01(currentCharge + chargeAdded);
                }
            }
        }
    }

    private void UpdateContinuousShootSound()
    {
        if (useContinuousShootSound)
        {
            if (m_wantsToShoot && m_CurrentAmmo >= 1f)
            {
                if (!m_continuousShootAudioSource.isPlaying)
                {
                    m_ShootAudioSource.PlayOneShot(shootSFX);
                    m_ShootAudioSource.PlayOneShot(continuousShootStartSFX);
                    m_continuousShootAudioSource.Play();
                }
            }
            else if (m_continuousShootAudioSource.isPlaying)
            {
                m_ShootAudioSource.PlayOneShot(continuousShootEndSFX);
                m_continuousShootAudioSource.Stop();
            }
        }
    }

    public void ShowWeapon(bool show)
    {
        weaponRoot.SetActive(show);

        if (show && changeWeaponSFX)
        {
            m_ShootAudioSource.PlayOneShot(changeWeaponSFX);
        }

        isWeaponActive = show;
    }

    public void UseAmmo(float amount)
    {
        m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo - amount, 0f, maxAmmo);
        m_LastTimeShot = Time.time;
    }

    public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        if (reloadType == WeaponReloadType.Clip && isReloading)
        {
            return false;
        }
        
        m_wantsToShoot = inputDown || inputHeld;
        if (m_wantsToShoot) 
        { 
            m_wantsToReload = false;
            isReloading = false;
        }
        switch (shootType)
        {
            case WeaponShootType.Manual:
                if (inputDown)
                {
                    return TryShoot();
                }
                return false;

            case WeaponShootType.Automatic:
                if (inputHeld)
                {
                    return TryShoot();
                }
                return false;

            case WeaponShootType.Charge:
                if (inputHeld)
                {
                    TryBeginCharge();
                }
                // Check if we released charge or if the weapon shoot autmatically when it's fully charged
                if (inputUp || (automaticReleaseOnCharged && currentCharge >= 1f))
                {
                    return TryReleaseCharge();
                }
                return false;

            default:
                return false;
        }
    }

    bool TryShoot()
    {
        if (m_CurrentAmmo >= 1f 
            && m_LastTimeShot + delayBetweenShots < Time.time)
        {
            HandleShoot();
            m_CurrentAmmo -= 1f;

            return true;
        }

        return false;
    }

    bool TryBeginCharge()
    {
        if (!isCharging
            && m_CurrentAmmo >= ammoUsedOnStartCharge
            && Mathf.FloorToInt((m_CurrentAmmo - ammoUsedOnStartCharge) * bulletsPerShot) > 0
            && m_LastTimeShot + delayBetweenShots < Time.time)
        {
            UseAmmo(ammoUsedOnStartCharge);

            LastChargeTriggerTimestamp = Time.time;
            isCharging = true;

            return true;
        }

        return false;
    }

    bool TryReleaseCharge()
    {
        if (isCharging)
        {
            HandleShoot();

            currentCharge = 0f;
            isCharging = false;

            return true;
        }
        return false;
    }

    void HandleShoot()
    {
        // Trigger attack animation if there is any
        if (weaponAnimator)
        {
            weaponAnimator.SetTrigger(k_AnimAttackParameter);
        }

        // muzzle flash
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position, weaponMuzzle.rotation, weaponMuzzle.transform);
            // Unparent the muzzleFlashInstance
            if (unparentMuzzleFlash)
            {
                muzzleFlashInstance.transform.SetParent(null);
            }

            Destroy(muzzleFlashInstance, 2f);
        }

        m_LastTimeShot = Time.time;

        // play shoot SFX
        if (shootSFX && !useContinuousShootSound)
        {
            m_ShootAudioSource.PlayOneShot(shootSFX);
        }

        // Callback on shoot
        if (onShoot != null)
        {
            onShoot();
        }

        OnShootProcessed?.Invoke();

        StartCoroutine(ShootOneFrameLater());
    }

    public void Reload()
    {
        if (!isCharging && !m_wantsToShoot &&
            m_CurrentAmmo < maxAmmo && 
            m_LastTimeShot + ammoReloadDelay < Time.time)
        {
            m_wantsToReload = true;

            if (weaponAnimator)
            {
                weaponAnimator.SetTrigger(k_AnimReloadParameter);
            }
        }
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = bulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);

        return spreadWorldDirection;
    }

    public void IncreaseDamage(float amt)
    {
        ProjectileStandard projectile = projectilePrefab.GetComponent<ProjectileStandard>();
        projectile.damage += amt;
    }

    public void SetAnimAimParameter(bool aiming, bool sprinting, bool sliding, bool inAir, bool crouching, bool moving)
    {
        // Sets the animator paramters
        weaponAnimator.SetBool(k_AnimAimParameter, aiming);
        weaponAnimator.SetBool(k_AnimSprintParameter, sprinting);
        weaponAnimator.SetBool(k_AnimSlideParameter, sliding);
        weaponAnimator.SetBool(k_AnimAirParameter, inAir);
        weaponAnimator.SetBool(k_AnimCrouchParameter, crouching);
        weaponAnimator.SetBool(k_AnimMoveParameter, moving);
    }

    IEnumerator ShootOneFrameLater()
    {
        // returning 0 will wait for 1 frame
        yield return 0;

        int bulletsPerShotFinal = shootType == WeaponShootType.Charge ? Mathf.CeilToInt(currentCharge * bulletsPerShot) : bulletsPerShot;

        // spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShotFinal; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
            ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position, Quaternion.LookRotation(shotDirection));
            newProjectile.Shoot(this);
        }

        var pcc = owner.GetComponent<PlayerCharacterController>();

        if (pcc)
        {
            pcc.Recoil(recoilAngle);
        }

    }

    public float AttachSight()
    {
        GameObject sight = Instantiate(sightPrefab, 
        sightParentObject.transform.position, 
        sightParentObject.transform.rotation, 
        sightParentObject.transform);

        if (ironSight)
        {
            Destroy(ironSight, 0f);
        }

        sight.transform.Translate(sightPositionOffset, Space.Self);
        sight.transform.Rotate(0, 90, 0, Space.Self);

        return sightCameraOffset;
    }
}
