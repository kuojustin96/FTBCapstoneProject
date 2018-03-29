using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Camera_Tracker : MonoBehaviour {

    public Cinemachine.CinemachineVirtualCamera[] cameras;
    int currentIndex = 0;

    void Start()
    {
        //brain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
    }


    void Update()
    {

        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Switch!");
                currentIndex++;  
            if(currentIndex >= cameras.Length)
            {
                currentIndex = 0;
            }

            for(int i = 0; i < cameras.Length; i++)
            {
                cameras[i].enabled = false;
            }
            cameras[currentIndex].enabled = true;
        }



    }

}
