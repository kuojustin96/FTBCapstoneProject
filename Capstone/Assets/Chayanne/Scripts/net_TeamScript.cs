using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
namespace ckp
{
    public class net_TeamScript : NetworkBehaviour
    {

        public enum Team
        {
            Zero,
            One,
            Two,
            Three
        };

        [SyncVar]
        public Team teamColor;
		void Start(){
			//SetTeam ();
			
		}

        public Color color;

        public void SetTeam(int team)
        {

            teamColor = (Team)team;
			//GameManager.instance.SetUpGame(gameObject, teamColor);
            Debug.Log("Assigned to the " + teamColor.ToString() + " team! " + (int)teamColor);
			//CmdSetTeam ();

        }
//
//		[Command]
//		public void CmdSetTeam(){
//
//			GameManager.instance.SetUpGame (gameObject, teamColor);
//			Debug.Log ("Assigned to the " + teamColor.ToString () + " team!");
//			RpcSetTeam ();
//		}
//		[ClientRpc]
//		public void RpcSetTeam(){
//
//			GameManager.instance.SetUpGame (gameObject, teamColor);
//			Debug.Log ("Assigned to the " + teamColor.ToString () + " team!");
//		}
    }
}