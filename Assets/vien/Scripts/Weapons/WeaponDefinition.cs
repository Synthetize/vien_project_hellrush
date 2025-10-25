using UnityEngine;

public enum FireMode { Semi, Auto, Burst }
public enum WeaponName { Pistol, SMG, Rifle, Shotgun, Sniper }

[CreateAssetMenu(menuName = "Weapons/Simple/Weapon Definition")]
public class WeaponDefinition : ScriptableObject {

    [Header("Identity")]
    public WeaponName weaponName = WeaponName.Rifle;


    [Header("Prefabs & Attach Points")]
    public GameObject weaponPrefab; // what you hold in hands
    public Vector3 localTransformPosition = new (0.364f, -0.517f, 1);
    public Vector3 localTransformRotation = new (-90, -45, -48);
    public Vector3 localTransformScale = new (0.03f, 0.03f, 0.03f);
    public Vector3 muzzleLocalPosition = new (0, 0, 0.5f);

    [Header("Shooting & Stats")]
    public FireMode fireMode = FireMode.Auto;
    [Min(0f)] public float damage = 20f;
    [Min(0.01f)] public float fireRate = 10f; // rounds per second
    [Min(0f)] public float spreadDegrees = 1.5f; // cone spread
    [Min(1)] public int burstCount = 3;
    [Min(0f)] public float burstInterval = 0.07f; // time between burst shots


    [Header("Ammo")]
    public int magazineSize = 30;
    public int totalAmmo = 90;
    public float reloadTime = 1.8f;
    public int currentMagazineAmmo = 30;


    [Header("Projectile vs Hitscan")]
    public bool useProjectile = false;
    public GameObject projectilePrefab; // if useProjectile
    public float projectileSpeed = 60f;
    public float projectileGravity = 0f;
    public float range = 120f; // hitscan range


    [Header("FX & Audio")]
    public ParticleSystem muzzleFlashPrefab;
    public AudioClip fireClip;
    public AudioClip reloadClip;
}