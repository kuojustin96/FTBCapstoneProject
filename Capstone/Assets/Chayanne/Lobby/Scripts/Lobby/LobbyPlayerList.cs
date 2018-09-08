using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    //List of players in the lobby
    public class LobbyPlayerList : MonoBehaviour
    {
        public static LobbyPlayerList _instance = null;

        public RectTransform playerListContentTransform;
        public GameObject warningDirectPlayServer;
        public Transform addButtonRow;

        protected VerticalLayoutGroup _layout;
        protected Dictionary<LobbyPlayer,LobbyListName> _players = new Dictionary<LobbyPlayer, LobbyListName>();

        public LobbyBetterPlayerList theList;

        public void OnEnable()
        {
            _instance = this;
            _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
        }

        public void DisplayDirectServerWarning(bool enabled)
        {
            if(warningDirectPlayServer != null)
                warningDirectPlayServer.SetActive(enabled);
        }

        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            
            if(_layout)
                _layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        }

        public void AddPlayer(LobbyPlayer player)
        {

            Debug.Log("Adding to player list: " + player.playerName); 
            string theName = player.playerName;

            //if (_players.ContainsKey(player))
            //{
            //    Debug.Log("Lobby player is already in the list!");
            //    return;
            //}

            //_players.Add(player,theList.RpcCreateName(theName));



            //player.transform.SetParent(playerListContentTransform, false);
            //addButtonRow.transform.SetAsLastSibling();


            //PlayerListModified();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            _players.Remove(player);
            PlayerListModified();
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (KeyValuePair<LobbyPlayer, LobbyListName> p in _players)
            {
                //p.Key.OnPlayerListChanged(i);
                ++i;
            }
        }
    }
}
