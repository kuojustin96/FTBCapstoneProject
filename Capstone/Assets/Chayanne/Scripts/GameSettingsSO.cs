using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ckp
{
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
            public int pBaseBodySlamStunDur;
            public float pBaseMoveSpeed;
            public float pBaseJumpHeight;

            public float pBaseSneakSpeed;

            public float pMinFallHeightVelocity;
            public float pMaxFallHeightVelocity;

            public float pBaseSugarPickupSize;
            public float pBaseFootStepVolume;
            public float pFootstepFalloff;


        }

        public net_Settings Settings;

    }
}