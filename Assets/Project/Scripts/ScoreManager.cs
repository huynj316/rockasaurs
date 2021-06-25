using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[SerializeField]
public enum PointType
{
    None, Foul, Woody, Hole
}


public class ScoreManager : Singleton<ScoreManager>
{
    public Vector3 DEBUG_TOSS_VECTOR = new Vector3(-31.3f,0,140.9f); //For in editor bug reproducing of an issue where foods fall slowly off the board

    public GameConfig gameConfig;

    public static event Action OnGameOver = delegate { };
    public static event Action OnNextRound = delegate { };
    public static event Action OnNextThrow = delegate { };
    public static event Action<int, Player> OnScoreUpdate = delegate { };

    const int GameOverValue = 21;

    public int player1Points { get; private set; }
    public int player2Points { get; private set; }
    public int winnerPoints {
        get {
            if (player1Points > player2Points)
                return player1Points;
            return player2Points;
        }
    }

    List<PointType> player1Round = new List<PointType>();
    List<PointType> player2Round = new List<PointType>();

    public int currentThrow = 0;
  
    public int currentRound = 0;

    int pointAddedThrow = 0;
    int pointDeductedThrow = 0;

    float roundDelay = 3f;

    //rules
    public GameObject rules;

    // scoreboard
    public TMP_Text player1Name;
    public TMP_Text player2Name;
    public TMP_Text player1Score;
    public TMP_Text player2Score;

    public ThrowScore[] throwScore1;
    public ThrowScore[] throwScore2;
    //public ThrowIndicator[] currentThrowType;

    public Image currentPlayerLine, currentScoreLine;
    public GameObject startIndicator;

    public Player currentPlayer;


    // end game panel
    public GameObject endIndicator;
    public TMP_Text winningPlayer;
    public TMP_Text player1EndScore;
    public TMP_Text player2EndScore;

    private void Start()
    {
        PointTrigger.OnPointTrigger += PointTrigger_OnPointTrigger;
        //GameOverPanel.OnPlayAgain += GameOverPanel_OnPlayAgain;
        Reset();
    }

    private void OnDestroy()
    {
        PointTrigger.OnPointTrigger -= PointTrigger_OnPointTrigger;
        //GameOverPanel.OnPlayAgain -= GameOverPanel_OnPlayAgain;
    }


    public void Reset()
    {
        player1Points = 0;
        player2Points = 0;

        currentRound = 0;
        currentPlayer = Player.One;

        player1Name.text = gameConfig.player1Name;
        player2Name.text = gameConfig.player2Name;
        player1Score.text = player1Points.ToString();
        player2Score.text = player2Points.ToString();

        player1Round = new List<PointType> { PointType.None, PointType.None, PointType.None, PointType.None };
        player2Round = new List<PointType> { PointType.None, PointType.None, PointType.None, PointType.None };

        SetPlayer1Color();
        endIndicator.gameObject.SetActive(false);
        StartCoroutine(StartGameIndicator(3f));

    }

    private void GameOver()
    {
        StartCoroutine(EndGameIndicator(1.5f));
        OnGameOver();
    }

    public IEnumerator NextRound()
    {
        Debug.Log("MD*** Next round scoremanager");
        //Whats calling this?
        yield return new WaitForSeconds(roundDelay);
        currentThrow = 0;
        currentRound++;

        int p1 = 0;
        int p2 = 0;

        for (int i = 0; i < 4; i++)
        {
            p1 += ValueForPointType(player1Round[i]);
            p2 += ValueForPointType(player2Round[i]);
        }


        if (p1 >= p2)
        {
            Debug.Log("Picking player 1" + p1 + "     " + p2);
            player1Points += (p1 - p2);
            currentPlayer = Player.One;
        }
        else
        {
            Debug.Log("Picking player 2" + p1 + "     " + p2);

            player2Points += (p2 - p1);
            currentPlayer = Player.Two;
        }

        player1Score.text = player1Points.ToString();
        player2Score.text = player2Points.ToString();

        if (player1Points >= GameOverValue || player2Points >= GameOverValue)
        {
            GameOver();
        } else {
            ClearRoundPoints();
            OnNextRound();
        }

    }

    private void ClearRoundPoints()
    {
        player1Round.Clear();
        player2Round.Clear();

        for (int i = 0; i < 4; i++)
        {
            player1Round.Add(PointType.None);
            player2Round.Add(PointType.None);
            throwScore1[i].Clear();
            throwScore2[i].Clear();
        }
    }

    public static int ValueForPointType(PointType pointType)
    {
        switch (pointType)
        {
            case PointType.Foul:
                return 0;

            case PointType.None:
                return 0;

            case PointType.Woody:
                return 1;

            case PointType.Hole:
                return 3;

        }
        return 0;
    }

    public PointType GetPointForPlayerAtIndex(int index, Player player)
    {
        return Round(player)[index];
    }

    public int GetPointValue(int index, Player player)
    {
        PointType point = GetPointForPlayerAtIndex(index, player);
        return ValueForPointType(point);
    }



    public List<PointType> Round(Player player)
    {
        return (player == Player.One) ? player1Round : player2Round;
    }

    public int ThrowIndexForPlayer(Player player) {

        return Round(player).IndexOf(PointType.None);
    }

    void GameOverPanel_OnPlayAgain()
    {
        ClearRoundPoints();
        Reset();
    }
    void PointTrigger_OnPointTrigger(PointType pointType, Food food)
    {
        currentThrow = ThrowIndexForPlayer(food.player);
        food.pointType = pointType;

        EvaluatePoints(food);

        food.pushed = false; //Commented out below since it's always false

        if (
//            food.pushed == false && 
            RoundOver(food.player) && !food.TriggeredNextTurn)
        {
            Debug.Log("**!!!abotu to start next round coroutine from scoremanager");

            food.TriggeredNextTurn = true;

            StartCoroutine(NextRound());
            return;
        }

        if (
//            food.pushed == false) 
            !food.TriggeredNextTurn)
        {
            food.TriggeredNextTurn = true;

            currentPlayer = (currentPlayer == Player.One) ? Player.Two : Player.One;
            OnNextThrow();
        }
    }

    public void NextThrow()
    {
        OnNextThrow();
    }   
    
    public bool OnRoundOver(Player foodPlayer)
    {
        return RoundOver(foodPlayer);
    }   



    private bool RoundOver(Player player)
    {

        bool roundOver = true;
        if (player == Player.One)
        {
            foreach (var pointType in player2Round)
            {
                if (pointType == PointType.None)
                {
                    roundOver = false;
                    break;
                }
            }
        }
        else
        {
            foreach (var pointType in player1Round)
            {
                if (pointType == PointType.None)
                {
                    roundOver = false;
                    break;
                }
            }
        }
        return roundOver;
    }

    private void SetPlayer1Color()
    {
        Color lineColor = new Color();
        if (ColorUtility.TryParseHtmlString("#ED1C24", out lineColor))
            currentPlayerLine.GetComponent<Image>().color = lineColor;
        currentScoreLine.GetComponent<Image>().color = lineColor;
    }

    private void SetPlayer2Color()
    {
        Color lineColor = new Color();
        if (ColorUtility.TryParseHtmlString("#D3A26A", out lineColor))
            currentPlayerLine.GetComponent<Image>().color = lineColor;
        currentScoreLine.GetComponent<Image>().color = lineColor;
    }

    IEnumerator StartGameIndicator(float delay)
    {
        yield return new WaitForSeconds(delay);
        startIndicator.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        startIndicator.gameObject.SetActive(false);
        FoodManager.instance.StartCoroutine("SpawnFood", 0f);


    }

    IEnumerator EndGameIndicator(float delay)
    {
        yield return new WaitForSeconds(delay);
        endIndicator.gameObject.SetActive(true);

        player1EndScore.text = player1Points.ToString("D2");
        player2EndScore.text = player2Points.ToString("D2");

        if (player1Points >= GameOverValue )
        {
            winningPlayer.text = "Winner - Player 1";

            Color p1Color = new Color();
            if (ColorUtility.TryParseHtmlString("#ED1C24", out p1Color))
                currentPlayerLine.GetComponent<Image>().color = p1Color;
           

        } else {
            winningPlayer.text = "Winner - Player 2";

            Color p2Color = new Color();
            if (ColorUtility.TryParseHtmlString("#D3A26A", out p2Color))
                currentPlayerLine.GetComponent<Image>().color = p2Color;
            
            
        }

    }

    public void EvaluatePoints(Food food)
    {
        var index = food.throwIndex;
        //Debug.Log("food index on eval points: " + food.throwIndex + "  " +  food.TriggeredNextTurn);
        if (food.player == Player.One)
            player1Round[index] = food.pointType;

        if (food.player == Player.Two)
            player2Round[index] = food.pointType;

        int score = ValueForPointType(food.pointType);

        if (food.player == Player.One)
        {
            throwScore1[index].score = score;
            SetPlayer2Color();
        }
        else
        {
            throwScore2[index].score = score;
            SetPlayer1Color();
        }
    }
}
