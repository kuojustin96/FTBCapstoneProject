using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAnimationScript : MonoBehaviour {

    public Animator DoorAnimator;
    public Animator BoxtopAnimator;
    public Animator CameraAnimator;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayOpenDoorAnimation()
    {

        DoorAnimator.SetBool("isOpen", true);
    }

    public void PlayOpenBoxAnimation()
    {

        BoxtopAnimator.SetBool("isOpen", true);


    }

    public void PlayCloseBoxAnimation()
    {

        BoxtopAnimator.SetBool("isOpen", false);


    }

    public void PlayCameraZoom()
    {
        CameraAnimator.SetBool("DoZoom", true);
    }


    public void PlayCameraZoomOut()
    {
        CameraAnimator.SetBool("DoZoom", false);
    }

}
