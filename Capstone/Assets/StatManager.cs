using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum TickerMessageType
{
    Win = 0,
    Notification = 1,
    NonPriority = 2,
}

public enum Stats
{
    SugarCollected = 0,
    SugarStolen = 1,
    TimeStunnedOthers = 2,
    TimeSpentStunned = 3,
    ItemsCraft = 4,
    NumEmotesUsed = 5,
    TimesVisitedBase = 6,
    NumTimesJumped = 7,
}

public class StatManager : NetworkBehaviour {

    public static StatManager instance = null;

    public Dictionary<Stats, int> statTracker { get; protected set; }

    public TextAsset priorityText;
    public TextAsset nonpriorityText;

    public float minTickerTime = 10f;
    public float maxTickerTime = 30f;
    [SyncVar] private float timeUntilTicker;
    [SyncVar] private int randNum;
    [SyncVar] private string tickerMessage;
    private Coroutine tickerTimeCoroutine;

    private int numSugarToWin;
    private List<UIController> uic = new List<UIController>();

    public class TickerTextType {
        public bool isPriority;
        public List<string> tickerText = new List<string>();
        
        public string GetRandomText()
        {
            int rand = Random.Range(0, tickerText.Count);
            return tickerText[rand];
        }
    }

    private Dictionary<string, TickerTextType> PriorityTickerTextTypes = new Dictionary<string, TickerTextType>();
    private Dictionary<string, TickerTextType> NonPriorityTickerTextTypes = new Dictionary<string, TickerTextType>();

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        statTracker = new Dictionary<Stats, int>();
    }

    void Start()
    {
        if(GameManager.instance == null)
            numSugarToWin = 50; // FOR TESTING
        else
            numSugarToWin = GameManager.instance.numSugarToWin;

        ResetTickerTimer();
        ReadAndOrganizeTextFile(priorityText, true);
        ReadAndOrganizeTextFile(nonpriorityText, false);

        //Test Code
        //PlayerClass ply = new PlayerClass();
        //ply.currentPlayerScore = 10;
        //Debug.Log(GetTickerMessage(TickerMessageType.NonPriority, ply));
    }

    public void UpdateStat(Stats stat)
    {
        statTracker[stat] += 1;
    }

    private void ReadAndOrganizeTextFile(TextAsset textAsset, bool isPriority)
    {
        TickerTextType currentHeader = null;
        if (!string.IsNullOrEmpty(textAsset.text)) //Make sure text file isn't empty
        {
            string[] sentances = textAsset.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None); //Split text based on new line

            if (sentances[0] == "Priority") //Check if this is a priority statement
                isPriority = true;

            foreach (string s in sentances)
            {
                if (s.Contains("/h")) //Check if it's a header
                {
                    //Split string again, create a new header, and organize it into dictionary
                    string[] str = s.Split(' ');
                    TickerTextType ttt = new TickerTextType();

                    if (isPriority)
                        PriorityTickerTextTypes.Add(str[1], ttt);
                    else
                        NonPriorityTickerTextTypes.Add(str[1], ttt);

                    //Update current header to add to later
                    currentHeader = ttt;
                }
                else //Still using last header created, add to current header dictionary
                {
                    if(currentHeader != null)
                        currentHeader.tickerText.Add(s);
                }
            }
        } else
        {
            Debug.LogError("StatManager: Text file is empty");
        }
    }

    #region Ticker Control
    public string GetTickerMessage(TickerMessageType tmt, PlayerClass player)
    {
        if (!isServer) return null;

        string message;
        //Check if message is a priority message
        if (tmt != TickerMessageType.NonPriority)
        {
            message = PriorityTickerTextTypes[tmt.ToString()].GetRandomText();
        }
        else
        {
            message = NonPriorityTickerTextTypes[tmt.ToString()].GetRandomText();
        }

        message = message.Replace("/player", "Player " + (player.playerNum + 1))
            .Replace("/numLeft", (numSugarToWin - player.currentPlayerScore).ToString())
            .Replace("/numHave", player.currentPlayerScore.ToString());

        return message;
    }

    public void SetUIController(UIController uic)
    {
        this.uic.Add(uic);
    }

    public void ResetTickerTimer()
    {
        if (!isServer) return;

        timeUntilTicker = Random.Range(minTickerTime, maxTickerTime);
        randNum = Random.Range(0, GameManager.instance.playerList.Count);

        if (isServer)
            RpcEnableTickerMessages(tickerMessage, false);
        else
            CmdEnableTickerMessages(tickerMessage, false);
    }

    [ClientRpc]
    private void RpcResetTickerTimer(float timeUntilTicker, int rand)
    {
        if (tickerTimeCoroutine != null)
            StopCoroutine(tickerTimeCoroutine);

        tickerTimeCoroutine = StartCoroutine(TickerTimer(timeUntilTicker, rand));
    }

    private IEnumerator TickerTimer(float timeUntilTicker, int randNum)
    {
        float saveTime = Time.time;
        while (Time.time < saveTime + timeUntilTicker)
            yield return null;

        tickerMessage = GetTickerMessage(TickerMessageType.NonPriority, GameManager.instance.playerList[randNum]);

        if (isServer)
            RpcEnableTickerMessages(tickerMessage, false);
        else
            CmdEnableTickerMessages(tickerMessage, false);
    }

    public void CallTickerMessage(TickerMessageType tmt, bool isPriority)
    {
        if (isPriority)
            StopCoroutine(tickerTimeCoroutine);

        tickerMessage = GetTickerMessage(tmt, GameManager.instance.playerList[randNum]);

        if (isServer)
            RpcEnableTickerMessages(tickerMessage, isPriority);
        else
            CmdEnableTickerMessages(tickerMessage, isPriority);
    }


    [Command]
    private void CmdEnableTickerMessages(string message, bool isPriority)
    {
        RpcEnableTickerMessages(tickerMessage, isPriority);
    }

    [ClientRpc]
    private void RpcEnableTickerMessages(string message, bool isPriority)
    {
        foreach (UIController u in uic)
            u.ShowTicker(TickerBehaviors.TickerText, message, isPriority);
    }
    #endregion
}