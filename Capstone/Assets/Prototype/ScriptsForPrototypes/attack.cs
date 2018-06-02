using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using jkuo;

public class attack : NetworkBehaviour {
    public GameObject keyPrefab;
    public GameObject keySpawn;
	public GameObject fireballSpawn;
	public GameObject fireballPrefab;
	public GameObject tornadoPrefab;
	public GameObject tornadoSpawn;
	public GameObject blizzardPrefab;
	public GameObject blizzardSpawn;

	public NetworkAnimator keyAnim;
    public ParticleSystem keySwingParticle;
	//public NetworkAnimator matchAnim;
	public bool attacking;
	private PlayerClass player;
	public GameObject shooter;

	public GameObject magnetSize;
    public float magnetMultiplier = 5f;
    private float origMagnetRadius;
    private NetworkSoundController nsc;
	public bool attackable;
	// Use this for initialization
	void Start () {
		 player = GetComponent<playerClassAdd>().player;
		attackable = true;
        nsc = GetComponent<NetworkSoundController>();
        origMagnetRadius = magnetSize.GetComponent<SphereCollider>().radius;
	}
	
	// Update is called once per frame
	void Update () {
	if (!isLocalPlayer)
		return;
		if (Input.GetButtonDown ("Fire1")&& attackable) {
			if (player.currentItem == null)
				return;
			if (player.currentItem.name == "keyHolder") {
				CmdKeyAttacking ();
				Debug.Log (player.currentItem.name);
				attackable = false;
				Invoke ("Attacking", 2);
                //nsc.CmdPlaySFX("SwordSwing", gameObject, 0.5f, false);
            }
			if (player.currentItem.name == "matchHolder" ) {
				CmdMatchAttacking ();
				Debug.Log (player.currentItem.name);
				attackable = false;
				Invoke ("Attacking", 2);
                //nsc.CmdPlaySFX("Fireball_Interactions", gameObject, 0.5f, false);
            }
			if (player.currentItem.name == "iceHolder" ) {
				CmdBlizzard ();
				Debug.Log (player.currentItem.name);
				attackable = false;
				Invoke ("Attacking", 2);
               // nsc.CmdPlaySFX("IceHolder", gameObject, 0.5f, false);
            }
			if (player.currentItem.name == "fanHolder" ) {
				CmdTornado();
				Debug.Log (player.currentItem.name);
				attackable = false;
				Invoke ("Attacking", 2);
              //  nsc.CmdPlaySFX("FanHolder", gameObject, 0.5f, false);
            }
		}
	}

	public void Attacking(){
		attackable = true;
	}

	//KEY
	[Command]
	//send attack to the server specifically
	//rpc isnt used here, because then rpc would need to be called twice, and rpc is less efficient than a cmd.
	public void CmdKeyAttacking(){
		
		if(keyAnim.animator.GetCurrentAnimatorStateInfo(0).IsName("idle")&& !player.isStunned ){
            GameObject swing = Instantiate(keyPrefab, keySpawn.transform.position, transform.rotation);
            NetworkServer.Spawn(swing);
            Invoke ("CmdKeyStopAttacking", .5f);
		    RpcKeyAnimSend (swing,gameObject);
			Debug.Log ("keyaattack");
			}
		 
	}
	[Command]
	public void CmdKeyStopAttacking(){
		RpcKeyAnimStop ();

	}
	[ClientRpc]
	//send anim to all clients
	public void RpcKeyAnimSend(GameObject swing, GameObject player){
        //attackTrigger.SetActive (true);\
        swing.GetComponent<attackTrigger>().parentPlayer = player;
        keyAnim.animator.SetInteger ("keyAttack",1);
        keySwingParticle.Play();
	}

	[ClientRpc]
	public void RpcKeyAnimStop(){
		//attackTrigger.SetActive (false);
		keyAnim.animator.SetInteger ("keyAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges <= 0) {

			CmdobjectTurnoff ();
		}
	}
	//Key

	//Match
	[Command]
	public void CmdMatchAttacking(){
		if (!player.isStunned) {
			//Invoke ("CmdFireball", 1.5f);
			CmdFireball ();
		}
		RpcMatchAnimSend ();
		Invoke ("CmdMatchStopAttacking", 2.458f);
	}

	[Command]
	public void CmdFireball(){
		Debug.Log ("fireball");
		GameObject fireball = Instantiate (fireballPrefab, fireballSpawn.transform.position, fireballSpawn.transform.rotation);
		NetworkServer.Spawn (fireball);
		RpcFireball (fireball);
	}
	[ClientRpc]
	public void RpcFireball(GameObject Fireball){
		Fireball.GetComponent<Rigidbody> ().AddForce(shooter.transform.forward * 10000);

	}

	[Command]
	public void CmdMatchStopAttacking(){
		RpcMatchAnimStop ();
	}
	[ClientRpc]
	//send anim to all clients
	public void RpcMatchAnimSend(){
		//matchAnim.animator.SetInteger ("matchAttack",1);
	}
	[ClientRpc]
	public void RpcMatchAnimStop (){
		//matchAnim.animator.SetInteger ("matchAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges <= 0) {

			CmdobjectTurnoff ();
		}
	}

	//Match


	//Magnet

	public void MagnetTurnOn(){

		magnetSize.GetComponent<SphereCollider> ().radius = origMagnetRadius * magnetMultiplier;
		Invoke("MagnetTurnOff", 40);
	}
	public void MagnetTurnOff(){

		magnetSize.GetComponent<SphereCollider> ().radius = origMagnetRadius;
		CmdobjectTurnoff ();
	}

	// Magnet


	//Blizzard
	[Command]
	public void CmdBlizzard(){
		if (!player.isStunned) {
			CmdBlizzardSpawn ();
		}
		Invoke ("CmdBlizzardStopAttacking", 2.458f);
	}

	[Command]
	public void CmdBlizzardSpawn(){
		Debug.Log ("blizzard");
		GameObject blizzard = Instantiate (blizzardPrefab, blizzardSpawn.transform.position,transform.rotation);
		NetworkServer.Spawn (blizzard);

		RpcBlizzard (blizzard,gameObject);
	}
	[ClientRpc]
	public void RpcBlizzard(GameObject Blizzard, GameObject player){
		Blizzard.GetComponent<blizzardTrigger>().parentPlayer = player;
	}

	[Command]
	public void CmdBlizzardStopAttacking(){
		RpcBlizzardAnimStop ();
	}
	[ClientRpc]
	public void RpcBlizzardAnimStop (){
		//matchAnim.animator.SetInteger ("matchAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges <= 0) {

			CmdobjectTurnoff ();
		}
	}
	// Blizzard

	//Tornado
	[Command]
	public void CmdTornado(){
		if (!player.isStunned) {
			CmdTornadoSpawn ();
		}
		Invoke ("CmdTornadoStopAttacking", 2.458f);
	}

	[Command]
	public void CmdTornadoSpawn(){
		Debug.Log ("tornado");
		GameObject tornado = Instantiate (tornadoPrefab, tornadoSpawn.transform.position,transform.rotation);
		NetworkServer.Spawn (tornado);

		RpcTornado (tornado,gameObject);
	}
	[ClientRpc]
	public void RpcTornado(GameObject Tornado, GameObject player){
		Tornado.GetComponent<tornadoTrigger>().parentPlayer = player;
	}

	[Command]
	public void CmdTornadoStopAttacking(){
		RpcTornadoAnimStop ();
	}
	[ClientRpc]
	public void RpcTornadoAnimStop (){
		//matchAnim.animator.SetInteger ("matchAttack", 0);
		player.itemCharges--;
		if (player.currentItem != null && player.itemCharges <= 0) {

			CmdobjectTurnoff ();
		}
	}
	// Tornado

	[Command]
	public void CmdobjectTurnoff(){
		RpcobjectTurnoff ();
	}
	[ClientRpc]
	public void RpcobjectTurnoff(){
		if (player.currentItem != null) {
			player.currentItem.SetActive (false);
			player.currentItem = null;
		}
		GetComponent<UIController> ().ResetUIItemTexture ();

	}


}
