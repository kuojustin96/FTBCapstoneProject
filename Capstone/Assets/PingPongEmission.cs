using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongEmission : MonoBehaviour {

    private Material[] rendMats;
    public int materialIndex = 0;
    private Material material;
    public float baseValue = 0.5f;
    public float maxValue = 1.5f;
    public float glowTime = 1f;

	// Use this for initialization
	void Start () {
        rendMats = GetComponent<Renderer>().materials;

        material = rendMats[materialIndex];
	}
	
	// Update is called once per frame
	void Update () {
        float intensity = baseValue + Mathf.PingPong(Time.time * glowTime, maxValue);
        material.SetColor("_EmissionColor", material.color * intensity);
    }
}
