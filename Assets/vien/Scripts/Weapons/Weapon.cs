using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject.SpaceFighter;


public class Weapon : MonoBehaviour {
	[Header("Setup")]
	public WeaponDefinition definition;
	public List<WeaponModule> modules = new();  // equipped modules
	public HudController hudController;
	public GameObject weaponCamera;
	

	[Header("Runtime (readonly)")]

	PlayerLoadoutController playerLoadoutController;

	WeaponLoadout _equippedLoadout;
	GameObject _weaponMuzzle;
	float _shoot_cooldown;
	bool _isFiring = false;
	bool _isReloading = false;
	bool _canShoot = true;
	float _reloadTimer;

	LineRenderer _laserLineRenderer;


    // Active buffs
    class ActiveBuff { public BuffDefinition def; public float remaining; }
    readonly List<ActiveBuff> _buffs = new();

	void Awake()
	{
		playerLoadoutController = GetComponent<PlayerLoadoutController>();
		hudController = GetComponent<HudController>();
		weaponCamera = GameObject.FindWithTag("WeaponCamera");

		// _equippedLoadout = playerLoadoutController.
		// definition = _equippedLoadout.definition;
		// modules = _equippedLoadout.defaultModules;
		// Recalculate(); // calculate initial stats
		// _currentAmmo = definition.magazineSize;
		// UpdateAmmoDisplay();
	}
	
	void Update() {
		if (_buffs.Count > 0)
		{
			bool changed = false;
			// iterate backwards to allow removal
			for (int i = _buffs.Count - 1; i >= 0; i--)
			{
				_buffs[i].remaining -= Time.deltaTime;
				if (_buffs[i].remaining <= 0f)
				{
					_buffs.RemoveAt(i);
					changed = true;
				}
			}
			if (changed) Recalculate();
		}

		if (_shoot_cooldown > 0f)
		{
			_shoot_cooldown -= Time.deltaTime;
		}

		if (_isReloading)
		{
			_reloadTimer -= Time.deltaTime;
			if (_reloadTimer <= 0f)
			{
				CompleteReload();
			}
		}

		if (_isFiring && !_isReloading) TryFire();

	}

    // API semplici
    public void EquipModule(WeaponModule mod) {
		if (!modules.Contains(mod)) 
		{
			modules.Add(mod); 
			Recalculate();
		} 
	}
    public void UnequipModule(WeaponModule mod) {
		if (modules.Remove(mod)) Recalculate(); 
		}

    public void ApplyBuff(BuffDefinition buff) {
        var existing = _buffs.Find(b => b.def.buffId == buff.buffId);
        if (existing != null) existing.remaining = buff.duration; // refresh
        else _buffs.Add(new ActiveBuff { def = buff, remaining = buff.duration });
        Recalculate();
    }

	public bool TryFire()
	{

		if (_shoot_cooldown > 0f)
		{
			return false;
		}
		_shoot_cooldown = 1f / Mathf.Max(0.01f, definition.fireRate);
		if (definition.currentMagazineAmmo <= 0)
		{
			StartReload();
			// Trigger reload or play empty sound
			return false;
		}
		definition.currentMagazineAmmo--;
		//SpawnProjectile();

		hudController.UpdateAmmo(definition.currentMagazineAmmo, definition.totalAmmo);
		// HERE you do your raycast / spawn projectile and use "damage"
		int excludeMask = ~LayerMask.GetMask("AttackRange");

		Instantiate(definition.muzzleFlashPrefab, _weaponMuzzle.transform.position, _weaponMuzzle.transform.rotation, _weaponMuzzle.transform);

		if (definition.fireClip != null)
		{
			AudioSource.PlayClipAtPoint(definition.fireClip, Camera.main.transform.position);
		}

		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, excludeMask))
		{
			GameObject bulletImpact = GameObject.Instantiate(definition.bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));

			Destroy(bulletImpact, 1f);
			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				if (definition.weaponName == WeaponName.Pistol) return true;
				damageable.TakeDamage(definition.damage);
				
			}

		}
		return true;
	}



	public void TryReload()
	{
		int neededAmmo = definition.magazineSize - definition.currentMagazineAmmo;
		int ammoToLoad = Mathf.Min(neededAmmo, definition.totalAmmo);
		definition.currentMagazineAmmo += ammoToLoad;
		definition.totalAmmo -= ammoToLoad;
		_isReloading = false;
		hudController.UpdateAmmo(definition.currentMagazineAmmo, definition.totalAmmo);
	}
	


    // --- calcolo stats super-lineare: applica Add, poi Mul, in ordine base -> moduli -> buff ---
    public void Recalculate() {
		//Debug.Log("Recalculating weapon stats...");
		//Debug.Log($"Base name: {definition.weaponName}, damage: {definition.damage}, fireRate: {definition.fireRate}, reloadTime: {definition.reloadTime}, magazineSize: {definition.magazineSize}");
        // 1) Add di tutti i moduli
        ApplyAll(modules, applyMul:false);
        // 2) Mul di tutti i moduli
        ApplyAll(modules, applyMul:true);
        // 3) Add dei buff attivi
        ApplyAllBuffs(applyMul:false);
        // 4) Mul dei buff attivi
        ApplyAllBuffs(applyMul:true);
    }

    void ApplyAll(List<WeaponModule> list, bool applyMul) {
        foreach (var m in list) if (m && m.modifiers != null)
            foreach (var mod in m.modifiers) ApplyMod(mod, applyMul);
    }

    void ApplyAllBuffs(bool applyMul) {
        foreach (var ab in _buffs) if (ab.def && ab.def.modifiers != null)
            foreach (var mod in ab.def.modifiers) ApplyMod(mod, applyMul);
    }

    void ApplyMod(StatMod mod, bool applyMulPhase) {
        if ((mod.op == StatOp.Mul) != applyMulPhase) return; // applica solo nella fase giusta
        switch (mod.stat) {
			case "damage":
				definition.damage = Apply(definition.damage, mod); 
				break;
			case "fireRate":
				definition.fireRate = Apply(definition.fireRate, mod); 
				break;
			case "reloadTime":
				definition.reloadTime = Apply(definition.reloadTime, mod); 
				break;
            case "magazineSize": definition.magazineSize = Mathf.RoundToInt(Apply(definition.magazineSize, mod)); 
			break;
        }
    }

	static float Apply(float baseVal, StatMod m) =>
		m.op == StatOp.Add ? baseVal + m.value : baseVal * m.value;

	public void SetFiring(bool isFiring)
	{
		_isFiring = isFiring;
	}
	
	public void SetReloading(bool isReloading)
	{
		_isReloading = isReloading;
	}

	public void StartReload()
    {
        if (_isReloading) return;
        if (definition.currentMagazineAmmo >= definition.magazineSize) return;
		if (definition.totalAmmo <= 0) return;
	
		if (definition.reloadClip != null)
		{
			AudioSource.PlayClipAtPoint(definition.reloadClip, Camera.main.transform.position);
		}
		float reloadLength = (definition.reloadClip != null) ? definition.reloadClip.length : definition.reloadTime;

        _isReloading = true;
        _reloadTimer = reloadLength;
    }

	void CompleteReload()
	{
		int neededAmmo = definition.magazineSize - definition.currentMagazineAmmo;
		int ammoToLoad = Mathf.Min(neededAmmo, definition.totalAmmo);
		definition.currentMagazineAmmo += ammoToLoad;
		definition.totalAmmo -= ammoToLoad;

		_isReloading = false;
		hudController.UpdateAmmo(definition.currentMagazineAmmo, definition.totalAmmo);
	}


	public void SetEquippedWeaponStats(WeaponLoadout newLoadout, GameObject weaponMuzzle)
	{
		_equippedLoadout = newLoadout;
		definition = _equippedLoadout.definition;
		modules = _equippedLoadout.defaultModules;
		Recalculate();
		//Debug.Log("Equipped weapon: " + _weaponMuzzle);
		hudController.UpdateAmmo(definition.currentMagazineAmmo, definition.totalAmmo);
		_weaponMuzzle = weaponMuzzle;
		if (definition.weaponEquip != null)
		{
			AudioSource.PlayClipAtPoint(definition.weaponEquip, Camera.main.transform.position);
		}
	}

	void SpawnProjectile()
	{
		GameObject projectile = GameObject.Instantiate(definition.projectilePrefab, _weaponMuzzle.transform.position, _weaponMuzzle.transform.rotation);
		Rigidbody rb = projectile.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.linearVelocity = _weaponMuzzle.transform.forward * definition.projectileSpeed;
		}
	}

	public bool GetIsReloading()
	{
		return _isReloading;
	}
	

}



