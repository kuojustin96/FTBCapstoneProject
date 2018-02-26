using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ckp;

namespace jkuo
{
    [RequireComponent(typeof(net_Event_MultiplayerTrigger))]
    public class event_DoorDisappear : NetworkBehaviour
    {
        public float openSpeed = 1f;
        public GameObject door;
        public float doorOpenY = 50f;
        private net_Event_MultiplayerTrigger eventTrigger;

        // Use this for initialization
        void Start()
        {
            eventTrigger = GetComponent<net_Event_MultiplayerTrigger>();
        }

        public void OpenDoor()
        {
            StartCoroutine(OpenDoorCoroutine());
        }

        private IEnumerator OpenDoorCoroutine()
        {
            Vector3 openPos = new Vector3(door.transform.position.x, door.transform.position.y + doorOpenY, door.transform.position.z);
            while(door.transform.position.y < openPos.y)
            {
                door.transform.position = Vector3.MoveTowards(door.transform.position, openPos, Time.deltaTime * openSpeed);
                yield return null;
            }

            Debug.Log("Door open, drop sugar");
            SugarManager.instance.CmdDropSugar(eventTrigger.numSugarDrops, eventTrigger.sugarDropPos.position);
        }
    }
}