using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class net_TeamScript : MonoBehaviour {

    public enum Team
    {
        Red,
        Green,
        Purple,
        Yellow
    };

    public Team teamColor;


    public void SetTeam(int team)
    {

        teamColor = (Team)team;

        Debug.Log("Assigned to the " + teamColor.ToString() + " team!");

    }
    

}
