using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cinemachine;
using System;

//The in game player
namespace jkuo
{

    public class net_PlayerController : ChadController
    {

        //in game player
        private UIController uic;
        private StatManager sm;
        private PlayerClass player;
        [Header("In-Game Objects")] 
        public GameObject playerUI;
        //Particles
        public GameObject stun;
        public PlayerSugarPickup sugarPickup;
        public Slider staminaSlider;
        public ParticleSystem[] Emotes;
        public bool emoteMenuOpen = false;
        private bool playingEmote = false;
        private Net_Hud_SugarCounter nhs;


        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            player = GetComponent<playerClassAdd>().player;
            uic = GetComponent<UIController>();
            Debug.Assert(uic, "Goodbye UI!");
            uic.SetUpVariables(player);

            sm = StatManager.instance;

        }

        protected override void MenuClosedUpdate()
        {
            base.MenuClosedUpdate();

            UseEmotes();
        }

        protected override void LocalCameraCheck()
        {
            if (!isLocalPlayer)
            { 
                playerUI.SetActive(false);
                GetComponent<AudioListener>().enabled = false;
            }
        }

        protected override void FixedUpdateMovement()
        {
            if (player.playerPaused)
            {
                m_MoveDir.x = 0f;
                m_MoveDir.z = 0f;
            }
            base.FixedUpdateMovement();
        }

        public override void UpdateSliders()
        {
            staminaSlider.value = currentStamina;
        }
        public override float GetGlidingFatigue()
        {
            return (glideFatigueSpeed + (player.sugarInBackpack * sugarStaminaFatigue));
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

        public override bool IsMenuOpen()
        {
            return player.craftingUIOpen || player.playerPaused;;
        }
    }
    #endregion
}