using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/EnemySpawn")] 
public class config_EnemySpawn : ScriptableObject {
    [SerializeField] protected GameObject enemyPrefab;
    [SerializeField] protected int baseSpawnBudget = 10;
    public int BaseSpawnBudget { get { return baseSpawnBudget; } }


    public void SpawnEnemyAt(Vector3 position) {
        position.z = 0;
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
}
