using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveConfig {
    // The wave selected may modify it's budget based on how many waves have been completed
    public int budgetPerIncursion;

    // The number of enemy incursions that are spawned by the wave
    public int numberOfIncursions;
}

[CreateAssetMenu(menuName = "Wave/WaveCollection")]
public class config_WaveCollection : ScriptableObject
{
    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> currentWave;

    [SerializeField] protected List<WaveConfig> waveCollection;
    public int NumWaves { get { return waveCollection.Count; } }

    public WaveConfig GetCurrentWave() {
        int waveIndex = currentWave.Value - 1;
        if (waveIndex >= 0 && waveIndex < waveCollection.Count) {
            return (waveCollection[waveIndex]);
        } else {
            Debug.LogWarning("Wave Index out of Bounds");
            return waveCollection[0];
        }
    }
}
