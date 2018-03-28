using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace ckp {
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class net_Event_CapturePoint_Trigger : NetworkBehaviour
    {

        net_Event_CapturePoint captureScript;

        public bool ShowInGame;

        void Start() {


            if (!ShowInGame)
                GetComponent<MeshRenderer>().enabled = false;

            captureScript = GetComponentInParent<net_Event_CapturePoint>();

        }

        void Update()
        {

        }

        void OnTriggerStay(Collider other)
        {
            captureScript.TriggerStay(other);
        }

        public void ShowTrigger(bool val)
        {
            GetComponent<MeshRenderer>().enabled = val;
        }

    }
}