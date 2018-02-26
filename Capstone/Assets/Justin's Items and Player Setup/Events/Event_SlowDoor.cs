using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ckp;

namespace jkuo
{
    [RequireComponent(typeof(net_Event_MultiplayerTrigger))]
    public class Event_SlowDoor : NetworkBehaviour
    {

        public float openSpeed = 1f;
        public GameObject door;
        public float doorOpenY = 50f;

        private net_Event_MultiplayerTrigger eventTrigger;
        private int numPlayers = 0;
        private int minPlayers = 0;
        private Vector3 doorOpenPos;
        private Vector3 doorClosePos;
        private Coroutine co;

        // Use this for initialization
        void Start()
        {
            eventTrigger = GetComponent<net_Event_MultiplayerTrigger>();
            doorClosePos = door.transform.position;
            doorOpenPos = new Vector3(doorClosePos.x, doorClosePos.y + doorOpenY, doorClosePos.z);
            minPlayers = eventTrigger.minPlayers;
        }

        public void OpenDoor()
        {
            numPlayers = eventTrigger.numTriggered;
            Debug.Log(numPlayers);
            Debug.Log(co);
            if (co == null)
            {
                co = StartCoroutine(OpenDoorCoroutine());
            }
        }

        private IEnumerator OpenDoorCoroutine()
        {
            Vector3 finalDest = doorOpenPos;

            while(door.transform.position != finalDest)
            {
                if (numPlayers >= minPlayers)
                    finalDest = doorOpenPos;
                else
                    finalDest = doorClosePos;

                door.transform.position = Vector3.MoveTowards(door.transform.position, finalDest, Time.deltaTime * openSpeed);
                yield return null;
            }

            co = null;

            if (door.transform.position == doorOpenPos)
                SugarManager.instance.CmdDropSugar(eventTrigger.numSugarDrops, eventTrigger.sugarDropPos.position);
        }
    }
}