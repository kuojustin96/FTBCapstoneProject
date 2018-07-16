using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using jkuo;

public enum SurfaceType
{
    Wood = 0,
    Metal = 1,
    Carpet = 2,
    Gliding = 3,
}

//Put on PlayerPrefab
public class SFXWalkController : NetworkBehaviour {

    public LayerMask layerMask;
    public SurfaceType currentSurface { get; protected set; }
    private bool overrideSFX;
    private string overrideName;
    private bool canStep = true;

    private NetworkParticleController netparticle;
    private SoundEffectManager sfm;
    private SFXOverrideTrigger sot;
    private ChadController npc;
    private NetworkSoundController nsc;
    private Vector3 walkParticlePos;

	// Use this for initialization
	void Start () {
        sfm = SoundEffectManager.instance;
        nsc = transform.root.GetComponent<NetworkSoundController>();
        npc = transform.root.GetComponent<ChadController>();
        netparticle = transform.root.GetComponent<NetworkParticleController>();
	}

    public void PlayStep()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, layerMask))
        {
            if (!overrideSFX)
            {
                int layerNum = hit.collider.gameObject.layer;
                string layerName = LayerMask.LayerToName(layerNum);

                if (layerName == "WoodSurface")
                    nsc.PlaySFXLocal("(Footsteps) Wood", transform.root.gameObject, 1f, 100f, false, false);
                else if (layerName == "MetalSurface")
                    nsc.PlaySFXLocal("(Footsteps) Metal", transform.root.gameObject, 1f, 100f, false, false);
                else if (layerName == "CarpetSurface")
                    nsc.PlaySFXLocal("(Footsteps) Carpet", transform.root.gameObject, 1f, 100f, true, false);
            }
            else
            {
                nsc.PlaySFXLocal(overrideName, transform.root.gameObject, 1f, 100f, false, false);
            }

            walkParticlePos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            if (netparticle)
            {
                netparticle.PlayParticleEffectLocal("Walk Particle", npc.gameObject, walkParticlePos, 2f);
                npc.CmdStopGlideParticle();
            }
            //nsc.StopSFXLocal("Gliding", transform.root.gameObject);
            //nsc.CmdStopSFX("Gliding", transform.root.gameObject);
        }
    }

    private void AllowStep()
    {
        canStep = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<SFXOverrideTrigger>())
            sot = other.GetComponent<SFXOverrideTrigger>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (sot)
        {
            overrideSFX = true;
            overrideName = sot.SFXOverrideName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SFXOverrideTrigger temp = other.GetComponent<SFXOverrideTrigger>();
        if (temp)
        {
            if (temp == sot)
            {
                overrideSFX = false;
                sot = null;
                overrideName = null;
            }
        }
    }
}
