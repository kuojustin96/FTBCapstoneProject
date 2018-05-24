using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;

        public TMP_InputField ipInput;

        public string defaultMatchName = "Memes";

        public bool isHosting = false;

        public void OnEnable()
        {
            lobbyManager.topPanel.ToggleVisibility(true);

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);

            //matchNameInput.onEndEdit.RemoveAllListeners();
            //matchNameInput.onEndEdit.AddListener(onEndEditGameName);
        }

        public void OnClickHost()
        {

            Debug.Log("LobbyMainMenu!");
            bool didHost = (lobbyManager.StartHost() != null);
            Debug.Log("LobbyMainMenu!");
            if (didHost)
            {
                LobbyManager.s_Singleton.TransitionToLobbyMenu();
                isHosting = true;
                SoundEffectManager.instance.PlaySFX("MouseClick", Camera.main.gameObject, 0.2f, true);
            }
            else
            {


                MainMenuManager.instance.ShowConnectionError();

            }
        }

        public void LeaveLobby()
        {

            lobbyManager.lobbyAnims.PlayCloseBoxAnimation();
            lobbyManager.lobbyAnims.PlayCameraZoomOut();

            Debug.Log("isHosting is " + isHosting);
            if (isHosting)
            {
                Debug.Log("Stopping Server");
                isHosting = false;
                lobbyManager.StopHost();
                lobbyPanel.gameObject.SetActive(false);
                lobbyManager.mainMenuPanel.gameObject.SetActive(true);
            }
            else
            {
                lobbyManager.StopClient();
                Debug.Log("Stopping Client");
                lobbyPanel.gameObject.SetActive(false);
                lobbyManager.mainMenuPanel.gameObject.SetActive(true);
            }
            Debug.Log("HJSDHDSAODD");
        }

        public void OnClickJoin()
        {
            Debug.Log("Joining!");
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();
            

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
            SoundEffectManager.instance.PlaySFX("MouseClick", Camera.main.gameObject, 0.2f, true);
        }

        public void OnClickDedicated()
        {
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

        public void OnClickCreateMatchmakingGame()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.matchMaker.CreateMatch(
                defaultMatchName,
                (uint)lobbyManager.maxPlayers,
                true,
				"", "", "", 0, 0,
				lobbyManager.OnMatchCreate);

            lobbyManager.backDelegate = lobbyManager.StopHost;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }

    }
}
