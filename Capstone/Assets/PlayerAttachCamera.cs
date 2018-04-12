using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerAttachCamera : NetworkBehaviour {

    void Start()
    {
        if(isLocalPlayer)
        {
                Net_Camera_Singleton.instance.SetupCamera(gameObject);
            //  Debug.LogError("Have fun without a camera manager!");
        }
    }
}
