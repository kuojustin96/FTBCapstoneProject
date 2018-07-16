using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using jkuo;
using Cinemachine;
public abstract class ChadController : NetworkBehaviour
{

    [Header("Abilities")]
    public bool canDoGlide = true;
    //public bool canDoJumpHover = false;


    public bool offlineTesting = false;

    protected NetworkSoundController nsc;
    protected NetworkParticleController netparticle;
    protected float defaultMaxDist;
    public CinemachineVirtualCamera virtualCam;

    //movement
    [Header("Movement")]
    public float baseSpeed = 50f;
    [SerializeField]
    protected float currentSpeedMult;
    public float glideSpeedMult = 3.0f;
    public float inAirDamping = 5f;

    [Header("Stamina and Fatigue")]
    public float staminaRegenSpeed = 2f;
    public float staminaRegenDelay = 1f;
    [Range(5, 95)]
    public int maxJumpStamina = 75;
    public float sugarStaminaFatigue = 0.05f;
    public float fatigueSpeed = 1f;
    protected float stamina = 100f;
    protected float currentStamina = 100f;
    protected Coroutine staminaResetCoroutine;

    public abstract float GetGlidingFatigue();

    [Header("Animation")]
    protected bool isPaused = false;
    protected CursorLockMode lockMode;
    public NetworkAnimator netAnim;


    [Header("Jumping")]
    public float jumpForce = 1000f;
    public float gravityScale = 5.0f;
    public LayerMask jumpMask;
    [SerializeField]
    protected bool isGrounded;
    public bool canJump = true;

    [Header("Turning")]
    public float lookSensitivity = 3.0f;

    protected Vector3 moveHori;
    protected Vector3 moveVert;
    protected Vector3 velocity = Vector3.zero;
    protected Vector3 m_MoveDir = Vector3.zero;

    protected bool inFreeLook = false;


    protected Vector3 _jumpForce = Vector3.zero;


    //gliding
    [Header("Gliding")]
    [SerializeField]
    protected bool isGliding = false;
    public float gravityDivisor = 5.0f;
    public float glideFatigueSpeed = 0.4f;
    protected GameObject glidingParticleEffect;
    [SerializeField]
    protected bool canJumpHover = false;
    protected CharacterController character;

    protected CinemachineVirtualCameraBase vCam;
    protected CinemachineFreeLook freeLook;
    protected bool jumped = false;


    protected virtual void Start()
    {
        Debug.LogWarning("Start!");
        currentSpeedMult = 1.0f;
        character = GetComponent<CharacterController>();
        vCam = Net_Camera_Singleton.instance.GetCamera();
        freeLook = vCam.gameObject.GetComponent<CinemachineFreeLook>();

        nsc = GetComponent<NetworkSoundController>();
        netparticle = GetComponent<NetworkParticleController>();
        defaultMaxDist = SoundEffectManager.instance.defaultMaxDist;

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
    }

    protected void FreeCam()
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
    public abstract void UpdateSliders();

    public abstract bool IsMenuOpen();

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
    protected virtual void PauseFreeCam()
    {

        freeLook.m_YAxis.m_InputAxisValue = 0.0f;
        freeLook.m_YAxis.m_InputAxisName = "";

    }

    protected virtual void DecideMovement()
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
                JumpParticle();
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
            nsc.CmdPlaySFX("Jump", gameObject, 0.3f, defaultMaxDist, true, false);
        }

        //Jumping/Gliding stop
        if (Input.GetKeyUp(KeyCode.Space))
        {
            canJumpHover = false;
            canJump = false;
            StopGlide();
        }

        //begin jump glide
        if (Input.GetKey(KeyCode.Space) && canJumpHover)
        {
            //hovering up
            if (currentStamina > stamina - maxJumpStamina)
            {
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

    protected virtual void JumpingHover()
    {
        _jumpForce = transform.up * (jumpForce);
        m_MoveDir.y = _jumpForce.y;
        currentStamina -= fatigueSpeed;
    }

    protected void FixedUpdate()
    {
        bool localCheck = isLocalPlayer || offlineTesting;
        if (localCheck)
        {
            FixedUpdateMovement();
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
        }
        else
        {
            m_MoveDir += Physics.gravity * gravityScale * Time.fixedDeltaTime;

            //if ((player.playerPaused && pausedGravity == null) || player.isStunned)
            //    pausedGravity = StartCoroutine(c_PausedGravity());
        }
        character.Move(m_MoveDir * Time.fixedDeltaTime);
    }

    protected virtual void Update()
    {

        bool localCheck = isLocalPlayer || offlineTesting;
        if (localCheck)
        {
            UpdateSliders();
            if (!IsMenuOpen())
            {
                MenuClosedUpdate();
            }
            else
            {
                MenuOpenUpdate();
            }

            //Movement
            DecideMovement();

            //Jump
            Jumping();
            UpdateAnimationParameters();
        }
    }

    protected virtual void MenuOpenUpdate()
    {
        PauseFreeCam();
    }

    protected virtual void MenuClosedUpdate()
    {
        //Camera Rotation
        Rotation();

        FreeCam();

    }

    protected abstract void LocalCameraCheck();

    #region Animation
    protected virtual void UpdateAnimationParameters()
    {
        Animator anim = netAnim.animator;
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");

        //Animation based on movement inputs
        anim.SetFloat("Vertical", character.velocity.x);
        anim.SetFloat("Horizontal", character.velocity.y);
        anim.SetBool("IsGrounded", character.isGrounded);
        anim.SetBool("IsGliding", isGliding);

        AnimateCharacter(x, y, character.isGrounded, isGliding);
    }


    public void AnimateCharacter(float x, float y, bool jump, bool glide)
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



    #endregion
    #region Gliding
    protected virtual void GlideUpdate()
    {
        if (!canDoGlide)
            return;
        if (isGliding && currentStamina > 0)
        {
            //Gliding
            //rb.AddForce(new Vector3(0f, -(gravity / gravityDivisor), 0f), ForceMode.Force);
            m_MoveDir.y = -4.0f;
            currentStamina -= GetGlidingFatigue();

            if (currentStamina <= 0)
            {
                StopGlide();
            }

        }
    }

    protected virtual void StartGlide()
    {
        if (!canDoGlide)
            return;
        isGliding = true;
        currentSpeedMult = glideSpeedMult;
        CmdPlayGlideParticle();
        nsc.CmdPlaySFX("Gliding", gameObject, 0.5f, defaultMaxDist, false, false);
        nsc.CmdPlaySFX("Jump", gameObject, 0.3f, defaultMaxDist, true, false);
    }

    protected virtual void StopGlide()
    {
        if (!canDoGlide)
            return;
        currentSpeedMult = 1.0f;
        isGliding = false;

        if (glidingParticleEffect)
            CmdStopGlideParticle();

        nsc.CmdStopSFX("Gliding", gameObject);
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
    #region Jumping and Stamina
    protected IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(staminaRegenDelay);

        while (currentStamina < 100)
        {
            currentStamina += staminaRegenSpeed;

            yield return null;
        }
    }
    protected void StopStaminaRegen()
    {
        if (staminaResetCoroutine != null)
        {
            StopCoroutine(staminaResetCoroutine);
            staminaResetCoroutine = null;
        }
    }
    protected void JumpParticle()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
        netparticle.CmdPlayParticleEffect("Walk Particle", gameObject, pos, 5f);
    }

    public virtual void ApplyJumpForce(Vector3 force)
    {
        m_MoveDir.y = force.y;
    }
    #endregion





}
