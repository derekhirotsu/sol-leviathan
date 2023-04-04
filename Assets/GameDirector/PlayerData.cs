using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    public float MaxHealth;
    // public float CurrentHealth;
    
    public bool MissilesUnlocked;
    public int MissileCapacity;
    public int CurrentMissileAmmo;
    public float MissileBlastRadius;

    public bool AirMeleeUnlocked;

    public bool AirBlastShotUnlocked;

    public bool RollAttackUnlocked;

    public bool HoverUnlocked;
    public float MaxHoverTime;

    public bool GroundSlamUnlocked;
    public float GroundSlamRadius; 

    public ProjectileConfigData MissileProjectileConfig;

    public ProjectileConfigData AirShotConfig;

    // public MeleeConfigData

    public static PlayerData CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<PlayerData>(jsonString);
    }
}


[System.Serializable]
public class ProjectileConfigData {
    public DamageType DamageType;

    public float Damage;

    public float FireRate;

    public Vector3 ForceImpulse;

    public int NumberOfProjectiles;

    public float ProjectileSpeed;
}

[System.Serializable]
public class HitscanConfigData {
    public DamageType DamageType;

    public float Damage;

    public float FireRate;

    public Vector3 ForceImpulse;

    public int NumberOfProjectiles;

    public float RaycastDistance;
}

[System.Serializable]
public class MeleeConfigData {
    public DamageType DamageType;

    public float Damage;

    public float FireRate;

    public Vector3 ForceImpulse;

    public float WindupTime;

    public float SwingTime;

    public float RecoveryTime;
    
    public int MoveSpeedWhileStriking;

    public float HorizontalScale;
    
    public float VerticalScale;

    public Vector3 DirectionOffset;

    public Vector3 PositionOffset;
}
