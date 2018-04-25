﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jkuo;

public enum SurfaceType
{
    Wood = 0,
    Metal = 1,
    Carpet = 2,
    Gliding = 3,
}

//Put on PlayerPrefab
public class SFXWalkController : MonoBehaviour {

    public LayerMask layerMask;
    public SurfaceType currentSurface { get; protected set; }
    public float stepTime = 0.1f;
    private bool overrideSFX;
    private bool canStep = true;

    private SoundEffectManager sfm;
    private SFXOverrideTrigger sot;
    private net_PlayerController npc;

	// Use this for initialization
	void Start () {
        sfm = SoundEffectManager.instance;
        npc = GetComponent<net_PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (!overrideSFX && npc.velocity != Vector3.zero)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 4.5f, layerMask))
            {
                int layerNum = hit.collider.gameObject.layer;
                string layerName = LayerMask.LayerToName(layerNum);

                if (layerName == "WoodSurface")
                    currentSurface = SurfaceType.Wood;
                else if (layerName == "MetalSurface")
                    currentSurface = SurfaceType.Metal;
                else if (layerName == "CarpetSurface")
                    currentSurface = SurfaceType.Carpet;
            }
            else//Not on any surface, play gliding sound if gliding
            {
                currentSurface = SurfaceType.Gliding;
            }

            if(canStep)
                PlayCorrespondingSFX();
        }
	}

    private void PlayCorrespondingSFX()
    {
        switch (currentSurface)
        {
            case SurfaceType.Wood:
                sfm.PlaySFX("(Footsteps) Wood");
                sfm.StopSFX("Gliding");
                break;

            case SurfaceType.Metal:
                sfm.PlaySFX("(Footsteps) Metal");
                sfm.StopSFX("Gliding");
                break;

            case SurfaceType.Carpet:
                sfm.PlaySFX("(Footsteps) Carpet");
                sfm.StopSFX("Gliding");
                break;

            case SurfaceType.Gliding:
                sfm.PlaySFX("Gliding");
                break;
        }

        canStep = false;
        Invoke("AllowStep", stepTime);
    }

    private void AllowStep()
    {
        canStep = true;
    }

    private void OnTriggerStay(Collider other)
    {
        sot = other.GetComponent<SFXOverrideTrigger>();
        if (sot)
        {
            overrideSFX = true;
            
            if(npc.velocity != Vector3.zero)
                sfm.PlaySFX(sot.SFXOverrideName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SFXOverrideTrigger temp = other.GetComponent<SFXOverrideTrigger>();
        if (!temp)
            return;

        if (temp == sot)
        {
            overrideSFX = false;
            sot = null;
        }
    }
}