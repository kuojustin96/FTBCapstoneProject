using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerAttachCamera : NetworkBehaviour {

    void Start()
    {
        if(isLocalPlayer)
        {
            if (Net_Camera_Singleton.instance)
            {
                NetworkServer.Spawn(Net_Camera_Singleton.instance.gameObject);
                Net_Camera_Singleton.instance.RpcSetupCamera(gameObject);
            }
            else
            {
                Debug.LogError("Have fun without a camera manager!");
            }
        }
    }
}
