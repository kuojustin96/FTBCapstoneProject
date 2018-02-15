using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAnimationScript : MonoBehaviour {

    public Animator DoorAnimator;
    public Animator BoxtopAnimator;

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
        //TODO: Fix Menu rigidbodies on repeat

        BoxtopAnimator.SetBool("isOpen", true);


    }

}
