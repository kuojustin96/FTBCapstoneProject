using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ckp;
using Cinemachine;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        static Color[] Colors = new Color[] { Color.red, Color.green, Color.magenta, Color.yellow };
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

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f/255.0f, 0.0f, 101.0f/255.0f,1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        public net_PlayerCameraScript cameraScript;

        public override void OnStartClient()
        {
            Debug.Log("=========OnStartClient=========");
            base.OnStartClient();
            Debug.Log("=========OnClientEnterLobby=========");

        }


        public override void OnClientEnterLobby()
        {
            Debug.Log("=========OnClientEnterLobby=========");
            Debug.Log("Local:" + isLocalPlayer);
            Debug.Log("Sever:" + isServer);
            Debug.Log("=========OnClientEnterLobby=========");
            //client, NOT server!
            base.OnClientEnterLobby();
                    
            if(isLocalPlayer && !isServer)
            {
                CmdNameChanged(LobbyManager.s_Singleton.GetLocalPlayerName());
            }

            //How will we get other players' names?
            
            if (isServer)
            {

                //Debug.Log("Regening from cliententer!");
                //Prototype.NetworkLobby.LobbyPlayerList._instance.theList.RpcRegenerateList();
                return;
            }
            else
            {

                if (isLocalPlayer)
                {
                    SetupLocalPlayer();
                }
                else
                {
                    SetupOtherPlayer();
                }

            }
            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            //AddLobbyPlayer();
            OnMyColor(playerColor);

        }

        private void AddLobbyPlayer()
        {
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);
            LobbyPlayerList._instance.AddPlayer(this);
        }

        public override void OnStartAuthority()
        {
            Debug.Log("OnStartAuthority");
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            SetupLocalPlayer();
            //AddLobbyPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            OnClientReady(false);
        }

        public net_TeamScript.Team GetTeamColor()
        {

            return (net_TeamScript.Team)System.Array.IndexOf(Colors, playerColor);

        }

        void SetupLocalPlayer()
        {


            readyObject.SetActive(true);
            Debug.Log("Setting local player!");
            CinemachineVirtualCameraBase playerCamera = LobbySingleton.instance.PlayerCam;
            CinemachineVirtualCameraBase lobbyCam = LobbySingleton.instance.LobbyCam;
            Debug.Assert(playerCamera, "Player Camera not set");
            lobbyCam.enabled = false;
            cameraScript.SwitchToCameraLocal(playerCamera);
            LobbySingleton.instance.getReadyUpText().SetActive(true);
            if (playerColor == Color.white)
                CmdColorChange();

            CmdNameChanged(LobbyManager.s_Singleton.GetLocalPlayerName());


            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
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
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;

                readyObject.SetActive(false);
                LobbySingleton.instance.getReadyUpText().SetActive(false);
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;

                readyObject.SetActive(true);

                LobbySingleton.instance.getReadyUpText().SetActive(true);
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
            colorButton.GetComponent<Image>().color = newColor;
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

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

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
                LobbySingleton.instance.TransitionCam.gameObject.SetActive(true);
                LobbyManager.s_Singleton.GetComponent<LobbyAnimationScript>().PlayOpenDoorAnimation();

                SoundEffectManager.instance.PlaySFX("Door Open", SoundEffectManager.instance.gameObject);
                //start fade
                LobbySingleton.instance.FadeIn();

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


        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            //My instance is in this list!~
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

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
