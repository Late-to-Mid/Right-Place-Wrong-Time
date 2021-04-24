using System.Collections.Generic;
using UnityEngine;

public class WeaponHUDManager : MonoBehaviour
{
    [Tooltip("UI panel containing the layoutGroup for displaying weapon ammos")]
    public RectTransform ammosPanel;
    [Tooltip("Prefab for displaying weapon ammo")]
    public GameObject ammoCounterPrefab;

    PlayerWeaponsManager m_PlayerWeaponsManager;
    List<AmmoCounter> m_AmmoCounters = new List<AmmoCounter>();

    void Start()
    {
        m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();

        m_PlayerWeaponsManager.onAddedWeapon += AddWeapon;

    }

    void AddWeapon(WeaponController weapon)
    {
        WeaponController activeWeapon = m_PlayerWeaponsManager.weapon;

        GameObject ammoCounterInstance = Instantiate(ammoCounterPrefab, ammosPanel);
        AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();

        newAmmoCounter.Initialize(activeWeapon, 0);

        m_AmmoCounters.Add(newAmmoCounter);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ammosPanel);
    }
}
