using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Weapon : MonoBehaviour {
    [Header("Setup")]
    public WeaponDefinition definition;
	public List<WeaponModule> modules = new();  // equipped modules
	public TextMeshProUGUI ammoDisplay;

    [Header("Runtime (readonly)")]

	PlayerLoadoutController playerLoadoutController;
	WeaponLoadout _equippedLoadout;
	int _currentAmmo;
	float _shoot_cooldown;
	bool _isFiring = false;
	bool _isReloading = false;
	bool _canShoot = true;
	float _reloadTimer;

    // Active buffs
    class ActiveBuff { public BuffDefinition def; public float remaining; }
    readonly List<ActiveBuff> _buffs = new();

    void Awake() {
		playerLoadoutController = GetComponent<PlayerLoadoutController>();
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
		if (_currentAmmo <= 0)
		{
			StartReload();
			// Trigger reload or play empty sound
			return false;
		}
		_currentAmmo--;
		UpdateAmmoDisplay();
		// HERE you do your raycast / spawn projectile and use "damage"
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
		{
			// Apply damage to the target
			//print("Hit " + hit.collider.name + " for " + damage + " damage.");
		}
		return true;
	}

	public void TryReload()
	{
		int neededAmmo = definition.magazineSize - _currentAmmo;
		int ammoToLoad = Mathf.Min(neededAmmo, definition.totalAmmo);
		_currentAmmo += ammoToLoad;
		definition.totalAmmo -= ammoToLoad;
		_isReloading = false;
		UpdateAmmoDisplay();
	}
	


    // --- calcolo stats super-lineare: applica Add, poi Mul, in ordine base -> moduli -> buff ---
    public void Recalculate() {
		Debug.Log("Recalculating weapon stats...");
		Debug.Log($"Base name: {definition.weaponName}, damage: {definition.damage}, fireRate: {definition.fireRate}, reloadTime: {definition.reloadTime}, magazineSize: {definition.magazineSize}");
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

	void UpdateAmmoDisplay()
	{
		if (ammoDisplay != null)
			ammoDisplay.text = $"{_currentAmmo} / {definition.totalAmmo}";
		Debug.Log($"Ammo: {_currentAmmo} / {definition.totalAmmo}");
		// Update your ammo UI here
	}
	
	public void StartReload()
    {
        if (_isReloading) return;
        if (_currentAmmo >= definition.magazineSize) return;
        if (definition.totalAmmo <= 0) return;

        _isReloading = true;
        _reloadTimer = definition.reloadTime;
    }

	void CompleteReload()
	{
		int neededAmmo = definition.magazineSize - _currentAmmo;
		int ammoToLoad = Mathf.Min(neededAmmo, definition.totalAmmo);
		_currentAmmo += ammoToLoad;
		definition.totalAmmo -= ammoToLoad;
		_isReloading = false;
		UpdateAmmoDisplay();
	}
	

	public void SetEquippedWeaponStats(WeaponLoadout newLoadout)
	{
		_equippedLoadout = newLoadout;
		definition = _equippedLoadout.definition;
		modules = _equippedLoadout.defaultModules;
		_currentAmmo = definition.magazineSize;
		Recalculate();
		UpdateAmmoDisplay();
	}

}



