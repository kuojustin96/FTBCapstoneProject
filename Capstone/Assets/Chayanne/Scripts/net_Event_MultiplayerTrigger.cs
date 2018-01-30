using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace ckp
{
    public class net_Event_MultiplayerTrigger : NetworkBehaviour
    {

        [Serializable]
        public class DebugInfo
        {
            public int debugNumPlayers = 0;
            public bool finishedEvent = false;
        }
        public enum net_TriggerMode
        {
            EachTriggerNeedsAtLeastOne, TriggersTogetherNeedMinimum
        }

        /*
        Trigger modes: 
        1. Each trigger needs to have at least 1 player
        2. Either trigger needs to have at least x players         
        */
        public net_Event_MultiplayerTrigger_Trigger[] triggers;
        public net_TriggerMode mode;

        [Tooltip("Amount how much sugar to drop")]
        public int numSugarDrops = 10;

        [Range(1, 4)]
        public int minPlayers = 2;

        [Space(10)]
        public UnityEvent completionMethods;

        public DebugInfo debugInfo;
        public void UpdateButton()
        {
            if (isServer)
            {



                if (debugInfo.finishedEvent)
                    return;


                if (mode == net_TriggerMode.EachTriggerNeedsAtLeastOne )
                {
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        if (triggers[i] == null)
                            continue;

                        if (!(triggers[i].numPlayersInTrigger > 0))
                        {
                            return;
                        }

                    }

                    RpcExectuteMethods();

                }
                else
                {
                    int numTriggered = 0;
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        if (triggers[i].IsTriggered())
                        {
                            numTriggered++;
                        }
                    }

                    if (numTriggered >= minPlayers)
                    {
                        RpcExectuteMethods();

                    }

                    debugInfo.debugNumPlayers = numTriggered;
                }






            }

        }

        public GameObject CreateTrigger()
        {
            GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Vector3 offset = new Vector3(0, 5, 0);
            trigger.AddComponent<BoxCollider>();
            trigger.gameObject.name = "Trigger";
            trigger.AddComponent<net_Event_MultiplayerTrigger_Trigger>();
            trigger.transform.position = transform.position + offset;
            trigger.transform.parent = transform;
            trigger.GetComponent<BoxCollider>().isTrigger = true;
            return trigger;
        }


        [ClientRpc]
        private void RpcExectuteMethods()
        {
            debugInfo.finishedEvent = true;
            completionMethods.Invoke();
        }
    }

}