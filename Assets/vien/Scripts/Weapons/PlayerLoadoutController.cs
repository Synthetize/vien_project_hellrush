using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class WeaponLoadout {
    public string weaponId;                 // un ID unico (es: "pistol", "smg")
    public WeaponDefinition definition;     // dai tuoi ScriptableObject
    public List<WeaponModule> defaultModules;   // moduli di default per QUELL’arma    

}

[RequireComponent(typeof(Weapon))]
public class PlayerLoadoutController : MonoBehaviour {
    public Weapon weapon;          // il tuo script Weapon (quello con BuffManager)
    public List<WeaponDefinition> availableWeapons; // armi disponibili nel gioco
    public List<WeaponLoadout> equippedWeapons; // armi equipaggiate

    GameObject weaponCamera;


    void Start()
    {
        weapon = GetComponent<Weapon>();
        AddWeaponToLoadout(WeaponName.Pistol);
        AddWeaponToLoadout(WeaponName.SMG);
        weaponCamera = GameObject.FindWithTag("WeaponCamera");
        EquipWeaponAtIndex(0);
    }

    void AddWeaponToLoadout(WeaponName weaponName)
    {
        bool isWeaponAlreadyEquipped = false;
        foreach (var weaponLoadout in equippedWeapons)
        {
            if (weaponLoadout.weaponId == weaponName.ToString().ToLower())
            {
                isWeaponAlreadyEquipped = true;
                break;
            }
        }

        if (!isWeaponAlreadyEquipped)
        {
            equippedWeapons.Add(new WeaponLoadout
            {
                weaponId = weaponName.ToString().ToLower(),
                definition = Instantiate(availableWeapons.Find(w => w.weaponName == weaponName)),
                defaultModules = new List<WeaponModule>(),
            });
        }

    }

    void EquipWeaponAtIndex(int index)
    {
        if (index < 0 || index >= equippedWeapons.Count) return;

        var weaponLoadout = equippedWeapons[index];
        GameObject weaponMuzzle = SpawnWeaponModel(weaponLoadout.definition);
        weapon.SetEquippedWeaponStats(weaponLoadout, weaponMuzzle);
    }

    GameObject SpawnWeaponModel(WeaponDefinition definition)
    {
        foreach (Transform child in weaponCamera.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject weaponModel = Instantiate(definition.weaponPrefab, weaponCamera.transform);
        weaponModel.transform.localPosition = definition.localTransformPosition;
        weaponModel.transform.localEulerAngles = definition.localTransformRotation;
        weaponModel.transform.localScale = definition.localTransformScale;

        GameObject weaponMuzzle = weaponModel.transform.Find("Muzzle").gameObject;
        return weaponMuzzle;
    }

    public void NextWeapon()
    {
        if (weapon.GetIsReloading()) return;
        int currentIndex = equippedWeapons.FindIndex(w => w.definition == weapon.definition);
        int nextIndex = (currentIndex + 1) % equippedWeapons.Count;
        EquipWeaponAtIndex(nextIndex);
    }
    
    public void PreviousWeapon()
    {
        if (weapon.GetIsReloading()) return;
        int currentIndex = equippedWeapons.FindIndex(w => w.definition == weapon.definition);
        int previousIndex = (currentIndex - 1 + equippedWeapons.Count) % equippedWeapons.Count;
        EquipWeaponAtIndex(previousIndex);
    }

}
