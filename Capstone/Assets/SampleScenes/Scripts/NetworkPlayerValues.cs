using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]

public class NetworkPlayerValues : NetworkBehaviour
{
	
	[SyncVar]
	public Color color;
	[SyncVar]
	public string playerName;


}

