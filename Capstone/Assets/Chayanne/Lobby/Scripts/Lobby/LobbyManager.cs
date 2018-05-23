using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;


namespace Prototype.NetworkLobby
{

    //This is our singleton. Please use it.
    
    public class LobbyManager : NetworkLobbyManager
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;


        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel topPanel;

        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        protected RectTransform currentPanel;

        public Button backButton;

        public Text statusInfo;
        public Text hostInfo;

        public LobbySingleton cameraHolder;


        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;

        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;


        [Header("Emote UI")]
        public RawImage tickerBackgroud;
        public GameObject emoteUI;
        public CanvasGroup emoteUICG;


        [Header("Other")]

        public MeshRenderer boxTopMesh;

        public GameObject lobbySpawn;

        public LobbyAnimationScript lobbyAnims;

        bool inGame = false;

        [SerializeField]
        TMPro.TMP_InputField nameField;

        public Transform playerListTransform;

        public LobbyPlayer host;

        public LobbyBetterPlayerList playerList;

        string pendingName;

        public LobbyMainMenu mainMenu;


        bool madeHost = false;
        void OnLevelWasLoaded()
        {   
        }

        public static bool IsPlayScene()
        {
            return SceneManager.GetActiveScene().name == LobbyManager.s_Singleton.playScene;
        }


        public static bool IsLobbyScene()
        {
            return SceneManager.GetActiveScene().name == LobbyManager.s_Singleton.lobbyScene;
        }

        public string GetLocalPlayerName()
        {
            return nameField.text;
        }

        public static bool IsLocalPlayerHost()
        {
            return s_Singleton.mainMenu.isHosting;
        }

        //TODO: Please call this when we make out in game menu so that the box top mesh can be unloaded properly
        public void SetInGame(bool val)
        {
            inGame = val;
        }



        void Start()
        {
            s_Singleton = this;
            _lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            //lobbyAnims = GetComponent<LobbyAnimationScript>();
            DontDestroyOnLoad(gameObject.transform.parent);


            SetServerInfo("Offline", "None");
        }



        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {

            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (topPanel.isInGame)
                {
                    ChangeTo(lobbyPanel);
                    if (_isMatchmaking)
                    {
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {

                            backDelegate = StopHostClbk;
                        }
                        else
                        {

                            backDelegate = StopClientClbk;
                        }
                    }
                }
                else
                {
                    ChangeTo(mainMenuPanel);
                }

                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                //GameObject player = conn.playerControllers[0].gameObject;

                //string theName = PlayerGameProfile.instance.GetPlayerData().name;
                //Color theColor = PlayerGameProfile.instance.GetPlayerData().color;
                //player.GetComponent<NetworkProfile>().CmdUpdateProfile(theName,theColor);
                Debug.Log("serverclientscenechanged");
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));

                //backDelegate = StopGameClbk;
                topPanel.isInGame = true;
                topPanel.ToggleVisibility(false);
            }


        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
                _isMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            //infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            //backDelegate();
            //topPanel.isInGame = false;
        }

        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }

        public void StopHostClbk()
        {
            if (_isMatchmaking)
            {
                matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
                _disconnectServer = true;
            }
            else
            {
                Debug.Log("Stopping");
                StopHost();
            }


            ChangeTo(mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        public void StopServerClbk()
        {
            StopServer();
            ChangeTo(mainMenuPanel);
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }




        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            //lobbyAnims.PlayOpenBoxAnimation();

            base.OnStartHost();

            //TransitionToLobbyMenu();
            ChangeTo(lobbyPanel);
            //backDelegate = StopHostClbk;
            //SetServerInfo("Hosting", networkAddress);
        }

        public void TransitionToLobbyMenu()
        {
            lobbyAnims.PlayOpenBoxAnimation();
            MainMenuManager.instance.FadeMenu(true);
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        public override void OnDestroyMatch(bool success, string extendedInfo)  
        {
            base.OnDestroyMatch(success, extendedInfo);
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------


        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject playerObj = CreateAndSetupPlayer();

            LobbyPlayer newPlayer = playerObj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);

            if(!madeHost)
            {
                host = newPlayer;
                madeHost = true;
            }

            

            Debug.Log("OnLobbyServerCreateLobbyPlayer! " + gameObject.name);
            //Prototype.NetworkLobby.LobbyPlayerList._instance.theList.RpcRegenerateList();

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return playerObj;
        }

        private GameObject CreateAndSetupPlayer()
        {

            Transform spawn = GetStartPosition();

            //Create our 3D Player here
            GameObject playerObj = Instantiate(lobbyPlayerPrefab.gameObject,spawn.position,spawn.rotation) as GameObject;

            //Lobby_Player_Setup setup = playerObj.GetComponent<Lobby_Player_Setup>();
            //setup.SetupPosition();


            Debug.Log("CreateAndSetupPlayer! " + gameObject.name);
            return playerObj;
        }

        //private void CreateLobbyGamePlayer()
        //{
        //    GameObject gameobj = Instantiate(gamePlayerPrefab.gameObject, spawnPointLobby.transform.position, spawnPointLobby.rotation) as GameObject;
        //    jkuo.net_PlayerController player = gameobj.GetComponent<jkuo.net_PlayerController>();
        //    Camera.main.enabled = false;
        //    NetworkServer.Spawn(gameobj);

        //}

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {

            //Prototype.NetworkLobby.LobbyPlayerList._instance.theList.RpcRegenerateList();

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            //todo: find the first callback that triggers after player spawns

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {


            bool allready = true;
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                    allready &= lobbySlots[i].readyToBegin;
            }

            if (allready)
                StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);



            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            TransitionScene();
        }

        private void TransitionScene()
        {
            SetInGame(true);
            ServerChangeScene(playScene);

        }


        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            TransitionToLobbyMenu();

            infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);


            Debug.Log("Client Connected!");
            
            

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                Debug.Log("Pure client");
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenuPanel);


        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
            infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            //for(int i = 0; i < s_Singleton.lobbySlots.Length; i++)
            //{
            //    if (s_Singleton.lobbySlots[i])
            //    {
            //        NetworkLobbyPlayer obj = s_Singleton.lobbySlots[i];
            //        Debug.Log("Player: " + obj.gameObject.name);

            //        GameObject go = obj.gameObject;
            //        LobbyPlayer lp = go.GetComponent<LobbyPlayer>();

            //        Debug.Assert(obj.GetComponent<LobbyPlayer>());
            //    }
            //}
            //Debug.Log(LobbyPlayerList._instance);
            //Debug.Log(LobbyPlayerList._instance.theList);
            //LobbyBetterPlayerList list = LobbyPlayerList._instance.theList;
            //Debug.Assert(list);
            //list.RpcRegenerateList(); 
            //Debug.Log("OnServerAddPlayer: " + conn.playerControllers[0].gameObject.name);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            Debug.Log("serverscenechanged");
            if (sceneName != lobbyScene)
            {

               

            }

        }

    }
}
