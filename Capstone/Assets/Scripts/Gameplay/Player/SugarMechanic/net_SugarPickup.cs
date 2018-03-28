using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ckp
{
    public class net_SugarPickup : NetworkBehaviour
    {

        public float pickupSpeed = 0.2f;
        public float lifeTime = 0.5f;
        float currentLife = 0.0f;
        public GameObject target;

        [Command]
        public void CmdCollectMe(GameObject go)
        {
            net_PlayerSugarScript obj = go.GetComponent<net_PlayerSugarScript>();
            obj.AddSugar();
            enabled = true;
            target = obj.gameObject;
        }

        // Update is called once per frame
        void Update()
        {

            currentLife += Time.fixedDeltaTime;

            if (currentLife > lifeTime)
            {
                gameObject.SetActive(false);
            }

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, pickupSpeed);

        }

        ////Make this a rpc?
        //void OnTriggerEnter(Collider other)
        //{

        //    if(other.gameObject.tag == "NetPlayer")
        //    {
        //        //send command
        //    }
            

        //}


        void CollectMe()
        {

        }
    }
}
