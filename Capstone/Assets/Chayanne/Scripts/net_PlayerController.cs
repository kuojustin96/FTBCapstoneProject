using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cinemachine;

namespace jkuo
{
	[RequireComponent(typeof(Rigidbody))]
	public class net_PlayerController : NetworkBehaviour
	{
		private UIController uic;
		public CinemachineVirtualCamera virtualCam;

		public bool offlineTesting = false;

		private Net_Hud_SugarCounter nhs;

		public Camera cam;
		public GameObject playerUI;
		public Slider staminaSlider;
		public ParticleSystem[] Emotes;
        public bool emoteMenuOpen = false;
		private bool playingEmote = false;
		private PlayerClass player;
		private Rigidbody rb;
		private bool isPaused = false;
		private CursorLockMode lockMode;
		public NetworkAnimator netAnim;

		//movement
		[Header("Movement")]
		public float speed = 50f;
		public float inAirDamping = 5f;
		public float staminaRegenSpeed = 2f;
		public float staminaRegenDelay = 1f;
		private Coroutine staminaResetCoroutine;
		[Range(5, 95)]
		public int maxJumpStamina = 75;
		private Vector3 moveHori;
		private Vector3 moveVert;
		public Vector3 velocity = Vector3.zero;

		bool inFreeLook = false;
		float lookSensitivity = 3.0f;

		//jump
		[Header("Jumping")]
		public float jumpForce = 1000f;
		public float gravity = 100f;
		public float downwardAcceleration = 1f;
		public LayerMask jumpMask;
		private bool isGrounded;
		public bool canJump = true;
		private Vector3 _jumpForce = Vector3.zero;

		public float fatigueSpeed = 1f;
		private float stamina = 100f;
		private float currentStamina = 100f;

		//gliding
		[Header("Gliding")]
		private bool isGliding = false;
		public float gravityDivisor = 20f;
		public float glideFatigueSpeed = 0.25f;


		//Particles
		public GameObject stun;

		CinemachineVirtualCameraBase vCam;
		CinemachineFreeLook freeLook;

		public PlayerSugarPickup sugarPickup;
		// Use this for initialization
		void Start()
		{
			vCam = Net_Camera_Singleton.instance.GetCamera();
			freeLook = vCam.gameObject.GetComponent<CinemachineFreeLook>();

			uic = GetComponent<UIController>();

			staminaSlider.value = staminaSlider.maxValue;
			rb = GetComponent<Rigidbody>();
			player = GetComponent<playerClassAdd>().player;
			LocalCameraCheck();
			Cursor.lockState = CursorLockMode.Locked;

			if (isLocalPlayer) {
				//nhs = GameObject.Find ("Canvas").GetComponent<Net_Hud_SugarCounter> ();
				//nhs.player = player;
			}


			if (isLocalPlayer)
			{
				GameObject virtualCamObj = GameObject.FindGameObjectWithTag("VirtualCamera");

				if (virtualCamObj)
				{
					virtualCam = virtualCamObj.GetComponent<CinemachineVirtualCamera>();
				}
			}

			if (GetComponent<UIController>())
			{
				GetComponent<UIController>().SetUpVariables(player);
			}
		}

		private void LocalCameraCheck()
		{
			if (!GetComponent<net_PlayerController>().isLocalPlayer)
			{
				playerUI.SetActive(false);
                GetComponent<AudioListener>().enabled = false;
			}
		}

		void Update()
		{
			if (isLocalPlayer || offlineTesting)
			{
                if (!player.playerPaused)
                {
                    if (!player.isStunned)
                    {
                        if (!player.craftingUIOpen)
                        {
                            //Movement
                            Movement();

                            //Camera Rotation
                            Rotation();

                            FreeCam();

                            //Jump
                            Jumping();

                            //Emotes
                            UseEmotes();
                        }
                    }
                }
			}
		}

		void FreeCam()
		{
			if (Input.GetKey(KeyCode.LeftAlt))
			{
				inFreeLook = true;
				freeLook.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
				freeLook.m_XAxis.m_InputAxisName = "Mouse X";
			}
			else
			{
				inFreeLook = false;
				freeLook.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
				freeLook.m_XAxis.m_InputAxisName = "";
                freeLook.m_XAxis.m_InputAxisValue = 0.0f;
                freeLook.m_XAxis.Value = 0.0f;
            }
		}


		private void Rotation()
		{

			float xRot;
			float yRot;
			xRot = Input.GetAxisRaw("Mouse Y");
			yRot = Input.GetAxisRaw("Mouse X");

			if (!inFreeLook)
			{
				Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
				rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
			}
		}

		private void Movement()
		{
            Animator anim = netAnim.animator;

			moveHori = transform.right * Input.GetAxis("Horizontal");
			moveVert = transform.forward * Input.GetAxis("Vertical");

            float x = Input.GetAxis("Vertical");
            float y = Input.GetAxis("Horizontal");

            anim.SetFloat("Vertical", x);
            anim.SetFloat("Horizontal", y);
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsGliding", isGliding);

            animateCharacter(x, y, isGrounded, isGliding);

            velocity = (moveHori + moveVert) * speed;

			if (velocity != Vector3.zero) {
				if (!isGrounded) {
					velocity = velocity * 5;
					rb.velocity = new Vector3 (velocity.x, rb.velocity.y, velocity.z);
				}
				else
					rb.velocity = new Vector3 (velocity.x,rb.velocity.y,velocity.z);
			}


			//if (Input.GetKey (KeyCode.W) && isGrounded) {   
			//	if (netAnim.animator.GetInteger ("CurrentState") != 1) {
			//		animateCharacter (1);
			//	}
			//} else if (Input.GetKey (KeyCode.S) && isGrounded) {
			//	if (netAnim.animator.GetInteger ("CurrentState") != 2) {
			//		animateCharacter (2);
			//	}
			//} else if (velocity == Vector3.zero) {
			//	animateCharacter (0);
			//} else if (Input.GetKeyDown (KeyCode.Space)) {
			//	if (netAnim.animator.GetInteger ("CurrentState") != 3) {
			//		animateCharacter (3);
			//	}
			//} else if (Input.GetKey (KeyCode.A) && isGrounded) {
			//	if (netAnim.animator.GetInteger ("CurrentState") != 4 && Input.GetKey (KeyCode.S) == false && Input.GetKey (KeyCode.W) == false   && netAnim.animator.GetInteger ("CurrentState") != 5  ) {				
			//		animateCharacter (4);
			//	}
			//}else if (Input.GetKey (KeyCode.D) && isGrounded) {
			//	if (netAnim.animator.GetInteger ("CurrentState") != 4 && Input.GetKey (KeyCode.S) == false && Input.GetKey (KeyCode.W) == false   && netAnim.animator.GetInteger ("CurrentState") != 5  ) {
			//		animateCharacter (5);
			//	}
			//}


		}


		private void Jumping()
		{
            Animator anim = netAnim.animator;
            float x = Input.GetAxis("Vertical");
            float y = Input.GetAxis("Horizontal");

            anim.SetFloat("Vertical", x);
            anim.SetFloat("Horizontal", y);
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsGliding", isGliding);

            RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, 4.5f, jumpMask))
			{
				if (!isGrounded)
					staminaResetCoroutine = StartCoroutine(RegenStamina());

				isGrounded = true;
				canJump = true;
				isGliding = false;
//				if(netAnim.animator.GetInteger("CurrentState")!=0){
//					animateCharacter (0);
//				}
			}
			else
			{
				if(isGrounded && staminaResetCoroutine != null)
				{
					StopCoroutine(staminaResetCoroutine);
					staminaResetCoroutine = null;
				}

				isGrounded = false;
				//apply gravity

				if (isGliding && currentStamina > 0)
				{
					rb.AddForce(new Vector3(0f, -(gravity / gravityDivisor), 0f), ForceMode.Force);
					currentStamina -= glideFatigueSpeed;
					staminaSlider.value = currentStamina;

					if (currentStamina <= 0)
						isGliding = false;
				}
				else
				{
					rb.AddForce(new Vector3(0f, -gravity, 0f), ForceMode.Force);
					rb.AddForce(Vector3.down * downwardAcceleration, ForceMode.Impulse);
				}
			}

			if (Input.GetKey(KeyCode.Space) && canJump)
			{
				if (currentStamina > stamina - maxJumpStamina)
				{
					_jumpForce = transform.up * (jumpForce * 1000);
					rb.AddForce(_jumpForce * Time.fixedDeltaTime);
					//if (Input.GetKeyDown(KeyCode.Space) && canJump) {
					animateCharacter (x,y,isGrounded,isGliding);
					//}

					currentStamina -= fatigueSpeed;
					staminaSlider.value = currentStamina;
				}
			}

			if (Input.GetKeyDown(KeyCode.Space) && !canJump && !isGliding)
				isGliding = true;


			if (Input.GetKeyUp(KeyCode.Space))
			{
				canJump = false;

				if (isGliding)
					isGliding = false;
			}
		}

		private IEnumerator RegenStamina()
		{
			yield return new WaitForSeconds(staminaRegenDelay);

			while(currentStamina < 100)
			{
				currentStamina += staminaRegenSpeed;
				staminaSlider.value = currentStamina;

				yield return null;
			}
		}

		#region Play Emotes
		private void UseEmotes()
		{
			if (emoteMenuOpen && !playingEmote)
			{
				if (!isLocalPlayer)
					return;

				if (Input.GetKeyDown(KeyCode.C))
				{
                    Debug.Log("CLOSE EMOTE MENU");
					emoteMenuOpen = false;
					uic.HideTicker();
				}

				if (Input.GetKeyDown(KeyCode.Alpha1))
					CmdEmote(0);

				if (Input.GetKeyDown(KeyCode.Alpha2))
					CmdEmote(1);

				if (Input.GetKeyDown(KeyCode.Alpha3))
					CmdEmote(2);

				if (Input.GetKeyDown(KeyCode.Alpha4))
					CmdEmote(3);
			}

			if (Input.GetKeyDown(KeyCode.C) && !playingEmote)
			{
				emoteMenuOpen = true;
				uic.ShowTicker(TickerBehaviors.Emotes);
			}
		}

		[ClientRpc]
		private void RpcEmote(int emoteNum)
		{
			Emotes[emoteNum].Play();
			emoteMenuOpen = false;
			playingEmote = true;

			if (!isLocalPlayer)
			{
				Vector3 temp = Emotes[emoteNum].transform.localScale;
				temp.x = -1;
				Emotes[emoteNum].transform.localScale = temp;
			}
			else
			{
				StartCoroutine(c_EmoteCooldown(emoteNum));
            }
		}

		[Command]
		private void CmdEmote(int emoteNum)
		{
			RpcEmote(emoteNum);
		}

		private IEnumerator c_EmoteCooldown(int emoteNum)
		{
            emoteMenuOpen = false;
            uic.HideTicker();

            float saveTime = Time.time;
			float psDuration = Emotes[emoteNum].main.duration;
			while (Time.time < saveTime + psDuration)
				yield return null;

			playingEmote = false;
		}
		#endregion

		#region Stun Player
		[ClientRpc]
		public void RpcStunPlayer(float duration)
		{
			if (player.currentItem == null) {
				//Debug.Log ("PlayerHasNoItem");
				player.isStunned = true;
				Invoke ("StunWait", duration);
				stun.SetActive (true);
				sugarPickup.StunDropSugar ();
			} 
			else {
				if (player.currentItem.name  == "buttonHolder") {
					//Debug.Log ("buttonHolder");
					player.itemCharges--;
					player.currentItem.SetActive (false);
					player.currentItem = null;
				}
				else{
					//Debug.Log ("PlayerHasNoItem");
					player.isStunned = true;
					Invoke ("StunWait", duration);
					stun.SetActive (true);
					sugarPickup.StunDropSugar ();
				}

			}
		}
		[Command]
		public void CmdStunPlayer(float duration)
		{
			RpcStunPlayer (duration);
		}

		public void StunPlayerCoroutine(float duration)
		{
			if (!isLocalPlayer)
				return;
			CmdStunPlayer (duration);
		}

		public void StunWait(){
			player.isStunned = false;
			stun.SetActive (false);
		}	

		public void animateCharacter(float x, float y, bool jump, bool glide){
			//netAnim.animator.SetInteger ("CurrentState",a);
			CmdAnimateCharacter (x,y,jump,glide);
		}


        [Command]
		public void CmdAnimateCharacter(float x, float y, bool jump, bool glide)
        {
			//netAnim.animator.SetInteger ("CurrentState",a);
			RpcAnimateCharacter (x,y,jump,glide);
		}
		[ClientRpc]
		public void RpcAnimateCharacter(float x, float y, bool jump, bool glide)
        {
			netAnim.animator.SetFloat ("Horizontal",x);

            netAnim.animator.SetFloat("Vertical", y);

            netAnim.animator.SetBool("IsGrounded", jump);

            netAnim.animator.SetBool("IsGliding", glide);
        }
	}
	#endregion
}