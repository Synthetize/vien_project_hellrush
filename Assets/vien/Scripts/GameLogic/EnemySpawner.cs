// ...existing code...
using UnityEngine;
using System.Collections;

public enum EnemiesToSpawn { CACODEMON, EXECUTIONER, ALL }

public class EnemySpawner : MonoBehaviour
{
    public EnemiesToSpawn enemiesToSpawn;
    public Transform[] spawnPoints;

    public Cacodemon cacodemonPrefab;
    public ExecutionerDemon executionerPrefab;

    [Header("Counts")]
    public int totalCacodemonToSpawn = 10;
    public int totalExecutionerToSpawn = 20;

    [Header("Timing")]
    public float spawnInterval = 3f;
    public bool spawnOnStart = false;

    // runtime
    int _remainingCacodemon;
    int _remainingExecutioner;
    int _aliveCount;
    Coroutine _spawnRoutine;
    bool _isSpawning;

    private HellGate_Controller hellGateController;

    // public flag other systems can read
    [HideInInspector] public bool allEnemiesCleared = false;

    void Start()
    {
        // if spawnPoints not set in inspector, collect child transforms
        if ((spawnPoints == null || spawnPoints.Length == 0) && transform.childCount > 0)
        {
            spawnPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) spawnPoints[i] = transform.GetChild(i);
        }

        _remainingCacodemon = totalCacodemonToSpawn;
        _remainingExecutioner = totalExecutionerToSpawn;

        // if there's nothing to spawn at all, consider cleared
        if (!HasAnyRemainingToSpawn() && _aliveCount == 0)
        {
            allEnemiesCleared = true;
        }
        else
        {
            allEnemiesCleared = false;
        }

        if (spawnOnStart) StartSpawning();

        GameObject parentObj = transform.parent.gameObject;
        hellGateController = parentObj.GetComponentInChildren<HellGate_Controller>();

    }

	void Update()
	{
		if (allEnemiesCleared)
        {
            hellGateController.ToggleHellGate();
            allEnemiesCleared = false;
        }
	}

	// call this to begin spawning
	public void StartSpawning()
    {
        if (_isSpawning) return;

        // reset state
        _isSpawning = true;
        allEnemiesCleared = false;

        // ensure remaining counts are initialized (in case Start wasn't used to init)
        _remainingCacodemon = Mathf.Max(0, _remainingCacodemon == 0 ? totalCacodemonToSpawn : _remainingCacodemon);
        _remainingExecutioner = Mathf.Max(0, _remainingExecutioner == 0 ? totalExecutionerToSpawn : _remainingExecutioner);

        _spawnRoutine = StartCoroutine(SpawnLoop());

        // if nothing to spawn and no alive, mark cleared immediately
        if (!HasAnyRemainingToSpawn() && _aliveCount == 0)
        {
            allEnemiesCleared = true;
            StopSpawning();
        }
    }

    public void StopSpawning()
    {
        if (!_isSpawning) return;
        _isSpawning = false;
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        _spawnRoutine = null;
    }

    IEnumerator SpawnLoop()
    {
        while (_isSpawning && (HasAnyRemainingToSpawn()))
        {
            if (spawnPoints == null || spawnPoints.Length == 0) yield break;

            // decide what to spawn this tick
            EnemiesToSpawn choice = enemiesToSpawn;
            if (enemiesToSpawn == EnemiesToSpawn.ALL)
            {
                // pick a type that still has remaining; if one exhausted, pick the other
                if (_remainingCacodemon <= 0 && _remainingExecutioner > 0) choice = EnemiesToSpawn.EXECUTIONER;
                else if (_remainingExecutioner <= 0 && _remainingCacodemon > 0) choice = EnemiesToSpawn.CACODEMON;
                else choice = (Random.value < 0.5f) ? EnemiesToSpawn.CACODEMON : EnemiesToSpawn.EXECUTIONER;
            }

            // spawn selected type if available
            if (choice == EnemiesToSpawn.CACODEMON && _remainingCacodemon > 0)
            {
                Spawn(cacodemonPrefab);
                _remainingCacodemon--;
            }
            else if (choice == EnemiesToSpawn.EXECUTIONER && _remainingExecutioner > 0)
            {
                Spawn(executionerPrefab);
                _remainingExecutioner--;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        // wait until all alive spawned enemies are killed
        while (_aliveCount > 0 && _isSpawning)
        {
            yield return null;
        }

        // mark cleared and stop
        allEnemiesCleared = true;
        _isSpawning = false;
        _spawnRoutine = null;
    }

    bool HasAnyRemainingToSpawn()
    {
        switch (enemiesToSpawn)
        {
            case EnemiesToSpawn.CACODEMON: return _remainingCacodemon > 0;
            case EnemiesToSpawn.EXECUTIONER: return _remainingExecutioner > 0;
            case EnemiesToSpawn.ALL: return _remainingCacodemon > 0 || _remainingExecutioner > 0;
            default: return false;
        }
    }

    void Spawn(MonoBehaviour prefab)
    {
        if (prefab == null || spawnPoints == null || spawnPoints.Length == 0) return;
        int idx = Random.Range(0, spawnPoints.Length);
        Transform sp = spawnPoints[idx];
        var instance = Instantiate(prefab, sp.position, Quaternion.identity);
        // attach tracker to know when it dies
        var tracker = instance.gameObject.AddComponent<EnemyTracker>();
        tracker.Init(this);
        _aliveCount++;
    }

    // called by EnemyTracker when an enemy is destroyed
    internal void OnEnemyDestroyed()
    {
        _aliveCount = Mathf.Max(0, _aliveCount - 1);

        // if nothing left to spawn and no alive, mark cleared
        if (!HasAnyRemainingToSpawn() && _aliveCount == 0)
        {
            allEnemiesCleared = true;
            Debug.Log("EnemySpawner: All enemies cleared.");
        }
    }

    // small helper component attached to spawned enemies to notify spawner on destroy
    class EnemyTracker : MonoBehaviour
    {
        EnemySpawner _spawner;
        public void Init(EnemySpawner spawner) { _spawner = spawner; }
        void OnDestroy()
        {
            if (_spawner != null) _spawner.OnEnemyDestroyed();
        }
    }
}
// ...existing code...