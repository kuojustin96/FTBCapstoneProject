using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public static ScoreController instance;
    public TextMeshProUGUI[] playerScores;
    public TextMeshProUGUI[] playerBackpackScores;

    void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}

        //foreach(TextMeshProUGUI t in playerScores)
        //{
        //    t.gameObject.SetActive(false);
        //}
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUpScoreController(int playerNum)
    {
        playerScores[playerNum].gameObject.SetActive(true);
    }

    public void UpdateBackpackScore(int playerNum, int currentBPScore)
    {
        playerBackpackScores[playerNum].text = currentBPScore.ToString();
    }

    public void UpdateScore(int playerNum, int currentScore) {
        playerScores[playerNum].text = currentScore.ToString();
        //playerBackpackScores[playerNum].text = "0";
    }
}
