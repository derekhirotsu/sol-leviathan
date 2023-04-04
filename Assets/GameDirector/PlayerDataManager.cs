using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableVariables;
using System.Security.Cryptography;

public class PlayerDataManager : MonoBehaviour {
    [SerializeField]
    protected TextAsset jsonData;

    public PlayerData playerData;

    public FloatVariable MaxHealth;
    // public FloatVariable CurrentHealth;

    public BoolVariable MissilesUnlocked;
    public IntVariable MissileCapacity;
    public IntVariable CurrentMissileAmmo;
    public FloatVariable MissileBlastRadius;

    public BoolVariable AirMeleeUnlocked;

    public BoolVariable AirBlastShotUnlocked;

    public BoolVariable RollAttackUnlocked;

    public BoolVariable HoverUnlocked;
    public FloatVariable MaxHoverTime;

    public BoolVariable GroundSlamUnlocked;
    public FloatVariable GroundSlamRadius; 

    // public ConfigAttack missilesConifg;

    void Start() {
        LoadPlayerData();
        UpdatePlayerState();
    }

    public void UpdatePlayerState() {
        MaxHealth.SetValue(playerData.MaxHealth);
        // CurrentHealth.SetValue(playerData.CurrentHealth);
        MissilesUnlocked.SetValue(playerData.MissilesUnlocked);
        MissileCapacity.SetValue(playerData.MissileCapacity);
        CurrentMissileAmmo.SetValue(playerData.CurrentMissileAmmo);
        MissileBlastRadius.SetValue(playerData.MissileBlastRadius);
        AirMeleeUnlocked.SetValue(playerData.AirMeleeUnlocked);
        AirBlastShotUnlocked.SetValue(playerData.AirBlastShotUnlocked);
        RollAttackUnlocked.SetValue(playerData.RollAttackUnlocked);
        HoverUnlocked.SetValue(playerData.HoverUnlocked);
        MaxHoverTime.SetValue(playerData.MaxHoverTime);
        GroundSlamUnlocked.SetValue(playerData.GroundSlamUnlocked);
        GroundSlamRadius.SetValue(playerData.GroundSlamRadius);

        // missilesConifg.config = playerData.missileConfigData;
    }

    public void LoadPlayerData() {
        playerData = PlayerData.CreateFromJSON(jsonData.text);
    }

    public void SavePlayerData() {
        string saveData = JsonUtility.ToJson(playerData, true);

        Debug.Log(saveData);
    }

    

    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public void SetPlayerData() {
        playerData.MaxHealth = MaxHealth.Value;
        // playerData.CurrentHealth = CurrentHealth.Value;
        playerData.MissilesUnlocked = MissilesUnlocked.Value;
        playerData.MissileCapacity = MissileCapacity.Value;
        playerData.CurrentMissileAmmo = CurrentMissileAmmo.Value;
        playerData.MissileBlastRadius = MissileBlastRadius.Value;
        playerData.AirMeleeUnlocked = AirMeleeUnlocked.Value;
        playerData.AirBlastShotUnlocked = AirBlastShotUnlocked.Value;
        playerData.RollAttackUnlocked = RollAttackUnlocked.Value;
        playerData.HoverUnlocked = HoverUnlocked.Value;
        playerData.MaxHoverTime = MaxHoverTime.Value;
        playerData.GroundSlamUnlocked = GroundSlamUnlocked.Value;
        playerData.GroundSlamRadius = GroundSlamRadius.Value;
    }
}
