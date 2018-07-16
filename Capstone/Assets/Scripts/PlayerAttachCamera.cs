using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerAttachCamera : NetworkBehaviour {

    public GameObject camTarget;
    public bool offlineTesting = false;

    void Start()
    {
        if(isLocalPlayer || offlineTesting)
        {
            if(offlineTesting)
            {
                Net_Camera_Singleton.instance.DebugPrepare();
                Debug.LogWarning("Adding sound listener");
                gameObject.AddComponent<AudioListener>();
            }
                Net_Camera_Singleton.instance.SetupCamera(camTarget);
            //  Debug.LogError("Have fun without a camera manager!");
        }
    }
}
