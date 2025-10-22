using UnityEngine;

[CreateAssetMenu(menuName="Weapons/Simple/Module")]
public class WeaponModule : ScriptableObject {
    public string slotId = "generic";   // es: "muzzle", "magazine"
    public StatMod[] modifiers;
}