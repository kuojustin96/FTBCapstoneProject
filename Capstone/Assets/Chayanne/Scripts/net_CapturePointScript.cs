    using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class net_CapturePointScript : MonoBehaviour {

    float currentProgress = 0;
    float captureSpeed = 1;

    Slider progressBar;

    [Tooltip("Is the zone active and can be captured?")]
    public bool isActive;

    [Tooltip("Time in seconds")]
    public float timeToCapture;

    public bool DebugTrigger;

    [Space(25)]

    public UnityEvent completionMethods;

	void Start () {
        progressBar = GetComponentInChildren<Slider>();
        SetCanCapture(isActive);
    }

    void Update () {
	

        	
	}

    public void TriggerStay(Collider other)
    {

        if (!isActive)
            return;

        if (other.gameObject.tag == "NetPlayer")
        {

            currentProgress += captureSpeed;

            progressBar.value = GetProgressPercentage();

            if(currentProgress >= 100)
            {
                SetCanCapture(false);
                completionMethods.Invoke();
            }

        }

    }

    public float GetProgressPercentage()
    {
        return currentProgress / timeToCapture;
    }

    public void SetCanCapture(bool val)
    {
        isActive = val;
        progressBar.gameObject.SetActive(val);
        GetComponentInChildren<net_CapturePointTriggerScript>().ShowTrigger(val);
    }





}
