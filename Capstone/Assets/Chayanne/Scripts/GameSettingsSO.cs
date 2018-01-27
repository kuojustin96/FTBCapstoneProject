using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode Settings", menuName = "Settings/Gamemode Settings", order = 1)]
public class GameSettingsSO : ScriptableObject
{

    [System.Serializable]
    public class net_Settings
    {
        [Header("Inventory Settings")]
        public int maxSugarLimit;
        public float baseCraftingTime;
        public int minSugarNeededToCraft;


        [Header("Player Settings")]
        public int baseBodySlamStunDur;
        public float baseMoveSpeed;
        public float baseJumpHeight;
    }

    public net_Settings Settings;

}
