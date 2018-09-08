using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ckp;
using Cinemachine;
using System;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        Color[] Colors;
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public Button colorButton;
        public InputField nameInput;
        public Button readyButton;
        public GameObject readyObject;
        public Button waitingPlayerButton;
        public Button removePlayerButton;

        public GameObject localIcone;
        public GameObject remoteIcone;

        bool countDownStart = false;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar]
        public string playerName = "NOTSET";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        public net_PlayerCameraScript cameraScript;
        public GameObject gamePlayerObject;
        NetworkOutfitScript outfitScript;

        public int playerNum;

        [SyncVar]
        public int outfitNum;



        public override void OnStartClient()
        {
            base.OnStartClient();

        }

        void Awake()
        {
            Colors = PlayerGameProfile.instance.Colors;
            outfitScript = GetComponent<NetworkOutfitScript>();
        }

        public override void OnClientEnterLobby()
        {
            Debug.Log("Local:" + isLocalPlayer);
            Debug.Log("Sever:" + isServer);

            //client, NOT server!
            base.OnClientEnterLobby();
                    

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            //AddLobbyPlayer();
            OnMyColor(playerColor);
        }


        public override void OnStartAuthority()
        {
            Debug.Log("Me, the player joining, is being set up!");
            base.OnStartAuthority();

            
            //if we return from a game, color of text can still be the one for "Ready"
            SetupLocalPlayer(true);
            //AddLobbyPlayer();
        }


        void SetupOtherPlayer()
        {
            LobbyManager.s_Singleton.playerList.CreateName(playerName);
            outfitScript.ChangeHat(outfitNum);
            OnClientReady(false);
        }

        public net_TeamScript.Team GetTeamColor()
        {

            return (net_TeamScript.Team)System.Array.IndexOf(Colors, playerColor);

        }

        void SetupLocalPlayer(bool isHost = false)
        {
            Debug.Log("Setting local player! " + isHost);
            //ps: camera sets itself up
            CinemachineVirtualCameraBase lobbyCam = Net_Camera_Singleton.LobbyCam;
            lobbyCam.enabled = false;


            PlayerGameProfile.instance.SetLobbyPlayer(this);


            ReadyUpManager.instance.getReadyUpText().SetActive(true);


            if (playerColor == Color.white)
            {
                CmdColorChange();
            }

            CmdNameChanged(LobbyManager.s_Singleton.GetLocalPlayerName());

            CmdOutfitChanged(PlayerGameProfile.instance.GetPlayerOutfitSelection());

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        private void CmdOutfitChanged(int outfitIndex)
        {
            outfitNum = outfitIndex;
            outfitScript.CmdChangeHat(outfitIndex);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {

                readyObject.SetActive(true);
                if (isLocalPlayer)
                {
                    ReadyUpManager.instance.getReadyUpText().SetActive(false);
                }
            }
            else
            {

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;

                readyObject.SetActive(false);

                ReadyUpManager.instance.getReadyUpText().SetActive(true);
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            //GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var
        
        public void OnMyName(string newName)
        {
            Debug.Log("OnMyName");
            playerName = newName;
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            //colorButton.GetComponent<Image>().color = newColor;
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            CmdColorChange();
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        //public void OnNameChanged(string str)
        //{
        //    CmdNameChanged(str);
        //}



        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
                
        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {

            //BAD code!

            if(!countDownStart)
            {
                countDownStart = true;
                Net_Camera_Singleton.TransitionCam.gameObject.SetActive(true);
                LobbyManager.s_Singleton.lobbyAnims.PlayOpenDoorAnimation();

                SoundEffectManager.instance.PlaySFX("Door Open", Camera.main.gameObject, 0.2f);
                SoundEffectManager.instance.PlaySFX("Door Open Jingle", Camera.main.gameObject);
                //start fade
                LobbyManager.s_Singleton.cameraHolder.FadeIn();

            }

            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }



        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];

            playerNum = idx;


        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
            RpcCreateNewName(name);
        }

        [ClientRpc]
        public void RpcCreateNewName(string name)
        {
              
            LobbyManager.s_Singleton.playerList.CreateName(name);

        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            if (LobbyManager.s_Singleton.playerList)
            {
                LobbyManager.s_Singleton.playerList.RemoveName(playerName);
            }
            ////My instance is in this list!~
            //LobbyPlayerList._instance.RemovePlayer(this);
            //if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
