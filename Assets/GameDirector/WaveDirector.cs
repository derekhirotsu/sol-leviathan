using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDirector : MonoBehaviour
{
    
    [SerializeField] protected List<config_EnemySpawn> TEST_ENEMIES;

    [Header("TEST Missiles")]
    
    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> playerMissileAmmoCapacity;

    [SerializeField]
    protected ScriptableVariables.IntVariable playerMissileAmmo;

    [Header("UI Fields")]

    protected MenuController menuController;

    [Header("Player Tracking")]

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> playerActive;

    protected bool playerDeathTrigger = false;

    [Header("Wave Collections")]
    [SerializeField] protected config_WaveCollection fullWaveCollection;
    [SerializeField] protected ActiveSpawnRegions ActiveSpawnRegions;

    [Header("Pre-Wave Configs")]
    // An encounter can be made from one or many waves of enemies.

    [SerializeField]
    protected ScriptableVariables.IntVariable currentWave;

    [SerializeField]
    protected ScriptableVariables.BoolVariable waveInProgress;

    [SerializeField]
    protected ScriptableVariables.BoolVariable upgradeAvailable;

    [Header("Wave Tracking")]
    [SerializeField] protected EntityLookup activeEnemies;
    protected WaveConfig activeWaveConfig;
    protected List<config_EnemySpawn> enemiesToSpawnThisIncursion; // We're going to spawn these

    // Start is called before the first frame update
    void Start() {
        // Get components
        menuController = GetComponent<MenuController>();

       // Start first wave
        currentWave.SetValue(0);

        playerDeathTrigger = false;

        waveInProgress.SetValue(false);
        upgradeAvailable.SetValue(false);
        
        StartNewWave();
    }

    void OnEnable() {
        playerActive.Subscribe(OnPlayerActiveChange);
    }

    void OnDisable() {
        playerActive.Unsubscribe(OnPlayerActiveChange);
    }

    // This checks for changes in PlayerActive as long as this component exists.
    // We could set flags for when to check so that the player could not
    // die in between waves for example.
    void OnPlayerActiveChange(bool newValue) {
        // Player is alive OR death trigger has already activated.
        if (newValue || playerDeathTrigger) {
            return;
        }

        playerDeathTrigger = true;
        InputBuffer.ToggleActionMap(InputBuffer.PlayerInputActions.UI);
        menuController.ShowMenu("game_over");
        this.StopAllCoroutines();
    }

    protected IEnumerator activeWaveCoroutine;
    public void StartNewWave() {
        // 1. Set the new wave value
        currentWave.ApplyChange(1);

        // int_currentMissileCount.Value = int_totalMissileCount.Value;
        playerMissileAmmo.SetValue(playerMissileAmmoCapacity);

        waveInProgress.SetValue(true);
        upgradeAvailable.SetValue(false);

        if (currentWave.Value > fullWaveCollection.NumWaves) {
            Debug.Log("The final wave has been completed.");
            return;
        }

        // 2. Get the Current Wave
        activeWaveConfig = fullWaveCollection.GetCurrentWave();

        // 3. Begin the Coroutine to track the incursions
        if (activeWaveCoroutine != null) {
            StopCoroutine(activeWaveCoroutine);
        }

        // 4. Set Wave Active Flag and begin tracking coroutine
        activeWaveCoroutine = TrackActiveWaveProgress();
        StartCoroutine(activeWaveCoroutine);
    }

    protected int currentIncursion = 0;
    protected bool NeedNewIncursionSpawn {
        get {
            return activeEnemies.Items.Count <= 0;
        }
    }

    protected IEnumerator TrackActiveWaveProgress() {
        // 1. Display Wave UI
        menuController.ShowMenu("wave_start", true);

        yield return new WaitForSeconds(3f);

        // 2. Hide Wave UI
        menuController.ShowMenu("player_hud");

        // This loop will run for each incursion
        for (int i = 0; i < activeWaveConfig.numberOfIncursions; ++i) {

            // 1. Spawn incursion
            SpawnIncursion();
            Debug.Log(" The incursion " + (i+1) + " of " + activeWaveConfig.numberOfIncursions + " has begun, " + activeEnemies.Items.Count + " enemies spawned");

            // 2. Wait until a new Incursion needs to spawn
            while (!NeedNewIncursionSpawn) {
                yield return new WaitForFixedUpdate();
            }


            Debug.Log(" The incursion " + (i+1) + " of " + activeWaveConfig.numberOfIncursions + " is over, as current enemies on field is " + activeEnemies.Items.Count);
            yield return new WaitForSeconds(2f);
        }

        menuController.ShowMenu("wave_over");

        yield return new WaitForSeconds(3f);

        menuController.ShowMenu("player_hud");

        waveInProgress.SetValue(false);
        upgradeAvailable.SetValue(true);

        // Clean up coroutine reference
        activeWaveCoroutine = null;
        
    }

    protected void SpawnIncursion() {
        // TODO : Get these from some additional config
        enemiesToSpawnThisIncursion = new List<config_EnemySpawn>();

        foreach (config_EnemySpawn spawn in TEST_ENEMIES) {
            enemiesToSpawnThisIncursion.Add(spawn);
        }

        
        foreach(config_EnemySpawn enemy in enemiesToSpawnThisIncursion) {
            SpawnEnemy(enemy);
        }
    }

    protected void SpawnEnemy(config_EnemySpawn enemy) {
        
        // 1. Lets get a random region
        int randomRegionIndex = Random.Range(0, ActiveSpawnRegions.Items.Count);
        SpawnRegion randomEnemySpawnRegion = ActiveSpawnRegions.Items[randomRegionIndex];

        // 2. Get a random position in the region
        Vector3 positionInRegion = randomEnemySpawnRegion.GetRandomPosInBounds();

        // 3. Spawn the enemy
        Debug.Log(enemy.name + " is spawning at " + positionInRegion);
        enemy.SpawnEnemyAt(positionInRegion);
    }
}
