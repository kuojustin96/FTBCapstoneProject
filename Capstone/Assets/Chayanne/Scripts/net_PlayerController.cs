﻿using System.Collections;
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

        public CinemachineVirtualCamera virtualCam;

        public bool offlineTesting = false;

		private Net_Hud_SugarCounter nhs;

        public Camera cam;
        public GameObject playerUI;
        public Slider staminaSlider;
        private PlayerClass player;
        private Rigidbody rb;
        private bool isPaused = false;
        private CursorLockMode lockMode;

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
        private Vector3 velocity = Vector3.zero;

        //look rotation
        [Header("Player / Camera Rotation")]
        public Transform cameraCube;
        public float lookSensitivity = 3f;
        [Range(10, 80)]
        public float yViewAngle = 30f;
        private float xRot;
        private float yRot;
        private Vector3 rotation = Vector3.zero;
        private bool inFreeLook = false;

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

        // Use this for initialization
        void Start()
        {
            staminaSlider.value = staminaSlider.maxValue;
            rb = GetComponent<Rigidbody>();
            player = GetComponent<playerClassAdd>().player;
            LocalCameraCheck();
			Cursor.lockState = CursorLockMode.Locked;

			if (isLocalPlayer) {
                //nhs = GameObject.Find ("Canvas").GetComponent<Net_Hud_SugarCounter> ();
                //nhs.player = player;
            }


            GameObject virtualCamObj = GameObject.FindGameObjectWithTag("VirtualCamera");

            if (virtualCamObj)
            {
                virtualCam = virtualCamObj.GetComponent<CinemachineVirtualCamera>();
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
                cam.GetComponent<Camera>().enabled = false;
                cam.GetComponent<AudioListener>().enabled = false;
                playerUI.SetActive(false);
            }
        }

        private void UpdateCursorLock()
        {
#if (DEBUG_MODE)
            if (Input.GetMouseButton(0))
            {
                isPaused = false;
                Cursor.visible = false;
                lockMode = CursorLockMode.Locked;
            }
#endif

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;

                if (isPaused)
                {
                    lockMode = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = false;
                    lockMode = CursorLockMode.Locked;
                }
            }


            Cursor.lockState = lockMode;
        }

        void Update()
        {
            if (isLocalPlayer || offlineTesting)
            {
                //UpdateCursorLock();

                if (!player.isStunned)
                {
                    if (!player.craftingUIOpen)
                    {


                        //Movement
                        Movement();

                        //Camera Rotation

                        LookCamera();

                        //Jump
                        Jumping();
                    }
                }
            }
        }

        private void Movement()
        {
            moveHori = transform.right * Input.GetAxis("Horizontal");
            moveVert = transform.forward * Input.GetAxis("Vertical");
                

            velocity = (moveHori + moveVert) * speed;

            if (velocity != Vector3.zero)
                rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        private void LookCamera()
        {

            xRot = Input.GetAxisRaw("Mouse Y");
            yRot = Input.GetAxisRaw("Mouse X");

            if (Input.GetKey(KeyCode.LeftAlt))
                inFreeLook = true;

            if (Input.GetKeyUp(KeyCode.LeftAlt) && inFreeLook)
            {
                inFreeLook = false;
                cameraCube.localEulerAngles = Vector3.zero;
            }

            if (inFreeLook)
            {
                if (virtualCam)
                {
                    virtualCam.Follow = cameraCube;
                    virtualCam.LookAt = cameraCube;
                }

                rotation = cameraCube.localEulerAngles + (new Vector3(-xRot, yRot, 0f) * lookSensitivity);
            }
            else
            {
                if (virtualCam)
                {
                    virtualCam.Follow = gameObject.transform;
                    virtualCam.LookAt = gameObject.transform;
                }
                rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
            }

            if (!inFreeLook)
                rotation = cameraCube.localEulerAngles + (new Vector3(-xRot, 0f, 0f) * lookSensitivity);

            float minYViewAngle = 360 - yViewAngle;
            float halfPoint = (minYViewAngle + yViewAngle) / 2;

            if(rotation.x > yViewAngle && rotation.x <= halfPoint)
                rotation.x = yViewAngle;
            else if(rotation.x < minYViewAngle && rotation.x > halfPoint)
                rotation.x = -yViewAngle;

            cameraCube.localEulerAngles = rotation;
        }

        private void Jumping()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 4.5f, jumpMask))
            {
                if (!isGrounded)
                    staminaResetCoroutine = StartCoroutine(RegenStamina());

                isGrounded = true;
                canJump = true;
                isGliding = false;
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


		[ClientRpc]
        public void RpcStunPlayer(float duration)
        {

			if (player.currentItem == null) {
				Debug.Log ("PlayerHasNoItem");
				player.isStunned = true;
				Invoke ("StunWait", duration);
				stun.SetActive (true);
			} 
			else {
				if (player.currentItem.name  == "buttonHolder") {
					Debug.Log ("buttonHolder");
					player.itemCharges--;
					player.currentItem.SetActive (false);
					player.currentItem = null;
				}
				else{
					Debug.Log ("PlayerHasNoItem");
					player.isStunned = true;
					Invoke ("StunWait", duration);
					stun.SetActive (true);
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
    }
}