using System.Collections.Generic;
using UnityEngine;

public class WeaponHUDManager : MonoBehaviour
{
    [Tooltip("UI panel containing the layoutGroup for displaying weapon ammos")]
    public RectTransform ammosPanel;
    [Tooltip("Prefab for displaying weapon ammo")]
    public GameObject ammoCounterPrefab;

    List<AmmoCounter> m_AmmoCounters = new List<AmmoCounter>();

    public void AddWeapon(WeaponController weapon)
    {
        WeaponController activeWeapon = weapon;

        GameObject ammoCounterInstance = Instantiate(ammoCounterPrefab, ammosPanel);
        AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();

        newAmmoCounter.Initialize(activeWeapon, 0);

        m_AmmoCounters.Add(newAmmoCounter);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ammosPanel);
    }
}
