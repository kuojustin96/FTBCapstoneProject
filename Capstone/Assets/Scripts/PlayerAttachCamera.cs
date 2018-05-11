using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerAttachCamera : NetworkBehaviour {

    public GameObject camTarget;

    void Start()
    {
        if(isLocalPlayer)
        {
                Net_Camera_Singleton.instance.SetupCamera(camTarget);
            //  Debug.LogError("Have fun without a camera manager!");
        }
    }
}
