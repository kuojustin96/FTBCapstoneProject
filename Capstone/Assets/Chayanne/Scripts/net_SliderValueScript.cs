using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ckp
{
    public class net_SliderValueScript : MonoBehaviour
    {

        net_Event_CapturePoint cps;
        Slider slider;

        void Start()
        {
            slider = GetComponent<Slider>();
            cps = GetComponentInParent<net_Event_CapturePoint>();
        }

        // Update is called once per frame
        void Update()
        {

            slider.value = cps.GetProgressPercentage();

        }

    }
}