using UnityEngine;

[CreateAssetMenu(menuName="Weapons/Simple/Weapon Definition")]
public class WeaponDefinition : ScriptableObject {
    public string displayName = "Rifle";
    public float damage = 10f;
    public float fireRate = 10f;
    public float reloadTime = 2f;
    public int magazineSize = 30;
    public int totalAmmo = 300;
}