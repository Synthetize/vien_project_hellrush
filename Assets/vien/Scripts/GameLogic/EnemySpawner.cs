using UnityEngine;
using System.Collections;

public enum EnemiesToSpawn { CACODEMON, EXECUTIONER, ALL, BOSSWAVE }

public class EnemySpawner : MonoBehaviour
{
    public EnemiesToSpawn enemiesToSpawn;
    public Transform[] spawnPoints;

    public Cacodemon cacodemonPrefab;
    public ExecutionerDemon executionerPrefab;
    public DemonKing demonKingPrefab;

    [Header("Boss Wave")]
    public Transform bossSpawnPoint; // point where boss will spawn (assign in Inspector)

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
    public AudioSource audioSource;
    private WaveManager waveManager;

    // public flag other systems can read
    [HideInInspector] public bool allEnemiesCleared = false;

    public float audioFadeDuration = 2f; // durata fade in secondi
    Coroutine _fadeRoutine;
    bool _isFading;

    bool _bossSpawned = false;
    bool _waveAdvanced = false;

    void Start()
    {
        // if spawnPoints not set in inspector, collect child transforms
        if ((spawnPoints == null || spawnPoints.Length == 0) && transform.childCount > 0)
        {
            spawnPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) spawnPoints[i] = transform.GetChild(i);
        }

        // initialize counters
        _remainingCacodemon = totalCacodemonToSpawn;
        _remainingExecutioner = totalExecutionerToSpawn;
        _aliveCount = 0;

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

        // try get HellGate_Controller from parent
        if (transform.parent != null)
        {
            GameObject parentObj = transform.parent.gameObject;
            hellGateController = parentObj.GetComponentInChildren<HellGate_Controller>();
        }
        waveManager = FindFirstObjectByType<WaveManager>();
    }

    void Update()
    {
        if (allEnemiesCleared && !_isFading)
        {
            if (!_waveAdvanced)
            {

                // consume the flag and start fade out which will toggle gate when done
                Debug.Log("update: All enemies cleared, fading out audio and toggling gate.");
                waveManager.AdvanceWave();
                _fadeRoutine = StartCoroutine(FadeOutAudioAndToggleGate(audioFadeDuration));
                _waveAdvanced = true;
            }
            allEnemiesCleared = false;
        }
    }

    IEnumerator FadeOutAudioAndToggleGate(float duration)
    {
        _isFading = true;

        if (audioSource == null)
        {
            if (hellGateController != null) hellGateController.ToggleHellGate();
            _isFading = false;
            yield break;
        }

        float startVol = audioSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / Mathf.Max(0.0001f, duration));
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        // restore original volume for next play (optional)
        audioSource.volume = startVol;

        if (hellGateController != null) hellGateController.ToggleHellGate();

        _isFading = false;
        _fadeRoutine = null;
    }

    IEnumerator FadeInAudioAndToggleGate(float duration)
    {
        _isFading = true;
        float targetVol = Mathf.Clamp01(audioSource.volume);
        audioSource.volume = 0f;
        audioSource.Play();

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVol, t / Mathf.Max(0.0001f, duration));
            yield return null;
        }

        audioSource.volume = targetVol;

        _isFading = false;
        _fadeRoutine = null;
    }

    // call this to begin spawning
    public void StartSpawning()
    {
        if (_isSpawning) return;

        // reset state
        _isSpawning = true;
        allEnemiesCleared = false;
        _bossSpawned = false;

        // reset remaining counters to configured totals
        _remainingCacodemon = totalCacodemonToSpawn;
        _remainingExecutioner = totalExecutionerToSpawn;

        // spawn boss immediately for boss wave if configured
        if (enemiesToSpawn == EnemiesToSpawn.BOSSWAVE && !_bossSpawned && demonKingPrefab != null && bossSpawnPoint != null)
        {
            var bossInstance = Instantiate(demonKingPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            var bossTracker = bossInstance.gameObject.AddComponent<EnemyTracker>();
            bossTracker.Init(this);
            _aliveCount++;
            _bossSpawned = true;
        }

        _spawnRoutine = StartCoroutine(SpawnLoop());
        _fadeRoutine = StartCoroutine(FadeInAudioAndToggleGate(audioFadeDuration));

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

            // for boss wave, spawn normal enemies as ALL behavior
            if (enemiesToSpawn == EnemiesToSpawn.BOSSWAVE) choice = EnemiesToSpawn.ALL;

            if (choice == EnemiesToSpawn.ALL)
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
            case EnemiesToSpawn.BOSSWAVE: return _remainingCacodemon > 0 || _remainingExecutioner > 0;
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