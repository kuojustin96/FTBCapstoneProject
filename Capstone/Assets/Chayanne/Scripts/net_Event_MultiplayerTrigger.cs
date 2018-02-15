using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace ckp
{
    //Script that runs events
    //Knows how many players are on trigger(s) and runs events if the right number of players is reached

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
            MultipleTriggers, OneBigTrigger
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

        public bool isRepeatable = false;

        [Range(0, 4)]
        public int minPlayers = 1;

        [Tooltip("Require minimum players in order to run functions")]
        public bool RequireMinPlayers = true;

        public int numTriggered { protected set; get; }

        [Tooltip("Time the players have to stand on triggers for the event to activate")]
        public float waitTime = 0f;
        private Coroutine co;

        [Space(10)]
        public UnityEvent completionMethods;
        public DebugInfo debugInfo;
        public void UpdateButton()
        {

            if (isServer)
            {
//                if (debugInfo.finishedEvent && !isRepeatable)
//                    return;

                if (mode == net_TriggerMode.MultipleTriggers )
                {
                    if (RequireMinPlayers)
                    {
                        for (int i = 0; i < triggers.Length; i++)
                        {
                            if (triggers[i] == null)
                                continue;

                            if (!(triggers[i].numPlayersInTrigger > 0))
                            {
                                if (co != null)
                                {
                                    StopCoroutine(co);
                                    co = null;
                                }

                                return;
                            }
                        }


						if (waitTime == 0) {
							Debug.Log ("executeCheck1");
							RpcExectuteMethods ();
						}
                        else
                            co = StartCoroutine(waitTimer());
                    }
                    else
                    {
                        RpcExectuteMethods();
                    }

                }
                else
                {
                    numTriggered = triggers[0].numPlayersInTrigger;

                    if (RequireMinPlayers)
                    {
                        if (numTriggered >= minPlayers)
                        {
                            if (waitTime == 0)
                                RpcExectuteMethods();
                            else
                                co = StartCoroutine(waitTimer());
                        }
                        else
                        {
                            if (co != null)
                            {
                                StopCoroutine(co);
                                co = null;
                            }
                        }
                        debugInfo.debugNumPlayers = numTriggered;
                    }
                    else
                    {
                        RpcExectuteMethods();
                    }
                }
            }

        }

        private IEnumerator waitTimer()
        {
            Debug.Log("Running Wait Timer for " + waitTime + " seconds");
            yield return new WaitForSeconds(waitTime);
            RpcExectuteMethods();
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