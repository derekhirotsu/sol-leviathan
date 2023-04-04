using UnityEngine;

[CreateAssetMenu(menuName = "StatProfile")]
public class StatProfile : ScriptableObject
{
    [SerializeField] protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    public float BaseMaxHealth { get { return maxHealth; } } // Should be named like this for player health modifications?

    [Header("Speed and Force")]
    [SerializeField] protected float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    [Range(0f, 1.5f)] [SerializeField] protected float weightMod = 1f;
    public float WeightMod { get { return weightMod; } }

    [SerializeField]
    ScriptableVariables.IntVariable baseDamage;
}
