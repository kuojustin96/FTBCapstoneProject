using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cinemachine;

namespace jkuo
{

    public class net_PlayerController : NetworkBehaviour
    {
        private UIController uic;
        private NetworkSoundController nsc;
        private float defaultMaxDist;
        private NetworkParticleController netparticle;
        private StatManager sm;
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
        private bool isPaused = false;
        private CursorLockMode lockMode;
        public NetworkAnimator netAnim;

        //movement
        [Header("Movement")]
        public float baseSpeed = 50f;
        [SerializeField]
        float currentSpeedMult;
        public float glideSpeedMult = 3.0f;
        public float inAirDamping = 5f;
        public float staminaRegenSpeed = 2f;
        public float staminaRegenDelay = 1f;
        private Coroutine staminaResetCoroutine;
        [Range(5, 95)]
        public int maxJumpStamina = 75;
        private Vector3 moveHori;
        private Vector3 moveVert;
        public Vector3 velocity = Vector3.zero;

        private Vector3 m_MoveDir = Vector3.zero;

        bool inFreeLook = false;
        public float lookSensitivity = 3.0f;
        [HideInInspector]
        public Coroutine pausedGravity;

        //jump
        [Header("Jumping")]
        public float jumpForce = 1000f;
        public LayerMask jumpMask;
        private bool isGrounded;
        public bool canJump = true;
        private Vector3 _jumpForce = Vector3.zero;
        public float sugarStaminaFatigue = 0.05f;
        public float fatigueSpeed = 1f;
        private float stamina = 100f;
        private float currentStamina = 100f;

        //gliding
        [Header("Gliding")]
        private bool isGliding = false;
        public float gravityDivisor = 20f;
        public float glideFatigueSpeed = 0.25f;
        private GameObject glidingParticleEffect;
        [SerializeField]
        bool canJumpHover = false;

        CharacterController character;

        //Particles
        public GameObject stun;

        CinemachineVirtualCameraBase vCam;
        CinemachineFreeLook freeLook;
        public float gravityScale = 1.0f;
        bool jumped = false;

        public PlayerSugarPickup sugarPickup;
        // Use this for initialization
        void Start()
        {
            currentSpeedMult = 1.0f;
            character = GetComponent<CharacterController>();
            vCam = Net_Camera_Singleton.instance.GetCamera();
            freeLook = vCam.gameObject.GetComponent<CinemachineFreeLook>();

            uic = GetComponent<UIController>();
            nsc = GetComponent<NetworkSoundController>();
            defaultMaxDist = SoundEffectManager.instance.defaultMaxDist;
            netparticle = GetComponent<NetworkParticleController>();
            sm = StatManager.instance;

            staminaSlider.value = staminaSlider.maxValue;
            player = GetComponent<playerClassAdd>().player;
            LocalCameraCheck();
            Cursor.lockState = CursorLockMode.Locked;



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
            if (!isLocalPlayer)
            {
                playerUI.SetActive(false);
                GetComponent<AudioListener>().enabled = false;
            }
        }

        void FixedUpdate()
        {
            if (isLocalPlayer || offlineTesting)
            {
                if (!player.craftingUIOpen)
                {
                    if (!player.playerPaused)
                    {

                    }
                    else
                    {

                    }

                    //Movement
                    FixedUpdateMovement();

                }
            }
        }

        protected virtual void FixedUpdateMovement()
        {
            if (character.isGrounded)
            {
                //m_MoveDir.y = -m_StickToGroundForce;

                if (jumped)
                {
                    jumped = false;
                    m_MoveDir.y = jumpForce;
                }

                if (player.playerPaused)
                {
                    m_MoveDir.x = 0f;
                    m_MoveDir.z = 0f;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * gravityScale * Time.fixedDeltaTime;

                //if ((player.playerPaused && pausedGravity == null) || player.isStunned)
                //    pausedGravity = StartCoroutine(c_PausedGravity());
            }
            character.Move(m_MoveDir * Time.fixedDeltaTime);
        }

        void Update()
        {

            bool localCheck = isLocalPlayer || offlineTesting;
            if (localCheck)
            {

                bool menuOpen = player.craftingUIOpen || player.playerPaused;

                if (!menuOpen)
                {
                    //Camera Rotation
                    Rotation();

                    FreeCam();

                    //Emotes
                    UseEmotes();
                }
                else
                {
                    PauseFreeCam();
                }

                //Movement
                Movement();

                //Jump
                Jumping();

                
                UpdateAnimationParameters();
            }
        }



        void PauseFreeCam()
        {

            freeLook.m_YAxis.m_InputAxisValue = 0.0f;
            freeLook.m_YAxis.m_InputAxisName = "";

        }


        void FreeCam()
        {

            freeLook.m_YAxis.m_InputAxisName = "Mouse Y";

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

                freeLook.m_YAxis.m_AccelTime = 0.0f;
                freeLook.m_YAxis.m_DecelTime = 0.0f;
                freeLook.m_YAxis.m_MaxSpeed = lookSensitivity * 1.5f;
            }
        }


        protected virtual void Rotation()
        {

            float xRot;
            float yRot;
            xRot = Input.GetAxisRaw("Mouse Y");
            yRot = Input.GetAxisRaw("Mouse X");

            if (!inFreeLook)
            {
                Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
                transform.rotation = (transform.rotation * Quaternion.Euler(rotation));
            }
        }

        protected virtual void Movement()
        {
            if (!player.playerPaused && !player.isStunned)
            {
                Animator anim = netAnim.animator;
                if (character.isGrounded && Input.GetKeyDown(KeyCode.Space))
                {
                    jumped = true;
                }
                moveHori = transform.right * Input.GetAxis("Horizontal");
                moveVert = transform.forward * Input.GetAxis("Vertical");

                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = (moveHori + moveVert) * currentSpeedMult;
                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, character.radius, Vector3.down, out hitInfo,
                                   character.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                m_MoveDir.x = desiredMove.x * baseSpeed * currentSpeedMult;
                m_MoveDir.z = desiredMove.z * baseSpeed * currentSpeedMult;
            }


        }

        protected virtual void Jumping()
        {

            //IsGrounded check

            bool groundedLastFrame = isGrounded;

            //RaycastHit hit;
            //isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 5f, jumpMask);

            isGrounded = character.isGrounded;
            if (isGrounded)
            {
                if (!groundedLastFrame)
                {
                    staminaResetCoroutine = StartCoroutine(RegenStamina());
                    StopGlide();
                }

                canJump = true;
                isGliding = false;
                canJumpHover = true;
            }
            else
            {
                canJump = false;

                //stop stamina regen
                StopStaminaRegen();
                GlideUpdate();
            }
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                
                JumpParticle();
            }

            //Jumping/Gliding stop
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Debug.Log("getkeyup space ");
                canJumpHover = false;
                canJump = false;
                StopGlide();
            }

            //begin jump glide
            if (Input.GetKey(KeyCode.Space) && canJumpHover)
            {
                Debug.Log(canJumpHover);
                //hovering up
                if (currentStamina > stamina - maxJumpStamina)
                {
                    Debug.Log("jumping hover");
                    JumpingHover();
                }
            }

            //Gliding 
            bool canGlide = !canJump && !isGliding && currentStamina > 1.0f
                && Input.GetAxis("Vertical") > 0.0f;

            if (Input.GetKeyDown(KeyCode.Space) && canGlide)
            {
                StartGlide();
            }



        }

        private void UpdateAnimationParameters()    
        {
            Animator anim = netAnim.animator;
            float x = Input.GetAxis("Vertical");
            float y = Input.GetAxis("Horizontal");

            //Animation based on movement inputs
            anim.SetFloat("Vertical", character.velocity.x);
            anim.SetFloat("Horizontal", character.velocity.y);
            anim.SetBool("IsGrounded", character.isGrounded);
            anim.SetBool("IsGliding", isGliding);

            animateCharacter(x, y, character.isGrounded, isGliding);
        }

        private void StopStaminaRegen()
        {
            if (staminaResetCoroutine != null)
            {
                StopCoroutine(staminaResetCoroutine);
                staminaResetCoroutine = null;
            }
        }

        private void JumpingHover()
        {
            _jumpForce = transform.up * (jumpForce);
            m_MoveDir.y = _jumpForce.y;
            currentStamina -= fatigueSpeed;
            staminaSlider.value = currentStamina;
        }

        private void GlideUpdate()
        {
            if (isGliding && currentStamina > 0)
            {
                //Gliding
                //rb.AddForce(new Vector3(0f, -(gravity / gravityDivisor), 0f), ForceMode.Force);
                m_MoveDir.y = -4.0f;
                currentStamina -= (glideFatigueSpeed + (player.sugarInBackpack * sugarStaminaFatigue));
                staminaSlider.value = currentStamina;

                if (currentStamina <= 0)
                {
                    StopGlide();
                }

            }
        }

        void StartGlide()
        {
            isGliding = true;
            currentSpeedMult = glideSpeedMult;
            CmdPlayGlideParticle();
            nsc.CmdPlaySFX("Gliding", gameObject, 0.5f, defaultMaxDist, false, false);
            nsc.CmdPlaySFX("Jump", gameObject, 0.3f, defaultMaxDist, true, false);
        }

        void StopGlide()
        {
            currentSpeedMult = 1.0f;
            isGliding = false;

            if (glidingParticleEffect)
                CmdStopGlideParticle();

            nsc.CmdStopSFX("Gliding", gameObject);
        }

        private void JumpParticle()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            netparticle.CmdPlayParticleEffect("Walk Particle", gameObject, pos, 5f);
            nsc.CmdPlaySFX("Jump", gameObject, 0.3f, defaultMaxDist, true, false);
        }

        public virtual void ApplyJumpForce(Vector3 force)
        {
            m_MoveDir.y = force.y;
        }

        private IEnumerator RegenStamina()
        {
            yield return new WaitForSeconds(staminaRegenDelay);

            while (currentStamina < 100)
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

                if (Input.GetKeyDown(KeyCode.C))
                {
                    emoteMenuOpen = false;
                    uic.HideTicker();
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    nsc.CmdPlaySFX("Angry", gameObject, 1f, defaultMaxDist, false, false);
                    CmdEmote(0);
                    sm.UpdateStat(Stats.NumEmotesUsed);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    nsc.CmdPlaySFX("Taunt", gameObject, 1f, defaultMaxDist, false, false);
                    CmdEmote(1);
                    sm.UpdateStat(Stats.NumEmotesUsed);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    nsc.CmdPlaySFX("Cheer", gameObject, 1f, defaultMaxDist, false, false);
                    CmdEmote(2);
                    sm.UpdateStat(Stats.NumEmotesUsed);
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    nsc.CmdPlaySFX("Cheer", gameObject, 1f, defaultMaxDist, false, false);
                    CmdEmote(3);
                    sm.UpdateStat(Stats.NumEmotesUsed);
                }
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
                // code run on other players
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

        #region Glide Particle Effect
        [Command]
        private void CmdPlayGlideParticle()
        {
            RpcPlayGlideParticle();
        }

        [ClientRpc]
        private void RpcPlayGlideParticle()
        {
            glidingParticleEffect = ObjectPoolManager.instance.SpawnObject("Glide Particle", 3f);
            glidingParticleEffect.transform.parent = transform;
            glidingParticleEffect.transform.position = transform.position;
        }

        [Command]
        public void CmdStopGlideParticle()
        {
            RpcStopGlideParticle();
        }

        [ClientRpc]
        private void RpcStopGlideParticle()
        {
            if (glidingParticleEffect)
            {
                float delay = glidingParticleEffect.GetComponent<ParticleSystem>().main.duration;
                ObjectPoolManager.instance.RecycleObject("Glide Particle", glidingParticleEffect, delay);
                glidingParticleEffect.transform.parent = null;
                glidingParticleEffect = null;
            }
        }
        #endregion

        #region Stun Player
        [ClientRpc]
        public void RpcStunPlayer(float duration)
        {
            if (player.currentItem == null)
            {
                //Debug.Log ("PlayerHasNoItem");
                player.isStunned = true;
                Invoke("StunWait", duration);
                stun.SetActive(true);
                sugarPickup.StunDropSugar();
            }
            else
            {
                if (player.currentItem.name == "buttonHolder")
                {
                    //Debug.Log ("buttonHolder");
                    player.itemCharges--;
                    player.currentItem.SetActive(false);
                    player.currentItem = null;
                }
                else
                {
                    //Debug.Log ("PlayerHasNoItem");
                    player.isStunned = true;
                    Invoke("StunWait", duration);
                    stun.SetActive(true);
                    sugarPickup.StunDropSugar();
                }

            }
        }
        [Command]
        public void CmdStunPlayer(float duration)
        {
            RpcStunPlayer(duration);
        }

        public void StunPlayerCoroutine(float duration)
        {
            if (!isLocalPlayer)
                return;

            sm.UpdateStat(Stats.TimeSpentStunned, duration);
            CmdStunPlayer(duration);
        }

        public void StunWait()
        {
            player.isStunned = false;
            stun.SetActive(false);
        }

        public void animateCharacter(float x, float y, bool jump, bool glide)
        {
            //netAnim.animator.SetInteger ("CurrentState",a);
            CmdAnimateCharacter(x, y, jump, glide);
        }


        [Command]
        public void CmdAnimateCharacter(float x, float y, bool jump, bool glide)
        {
            //netAnim.animator.SetInteger ("CurrentState",a);
            RpcAnimateCharacter(x, y, jump, glide);
        }
        [ClientRpc]
        public void RpcAnimateCharacter(float x, float y, bool jump, bool glide)
        {
            netAnim.animator.SetFloat("Horizontal", x);

            netAnim.animator.SetFloat("Vertical", y);

            netAnim.animator.SetBool("IsGrounded", jump);

            netAnim.animator.SetBool("IsGliding", glide);
        }
    }
    #endregion
}