using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameController : Singleton<GameController>
{
    public static event Action OnInit = delegate { };
    public static event Action OnARReset = delegate { };
    //public static event Action OnDisableSplashScreen = delegate { };
    public GameConfig gameConfig;
    public FoodConfig foodConfig;

    //state manager
    //public GameObject PlayARCornhole;
    //public GameObject PlayCornhole;
    //public GameObject Menu;

    private GameState CurrentGameState;


    public string player1
    {
        get
        {
            return gameConfig.player1Name;
        }
    }

    public string player2
    {
        get
        {
            return gameConfig.player1Name;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        EnterGameState(gameConfig.gameState);
        //RulesPanel.OnReset += RulesPanel_OnReset;
    }

    private void OnDestroy()
    {
        //RulesPanel.OnReset -= RulesPanel_OnReset;
    }

    void RulesPanel_OnReset()
    {
    
        Debug.Log("destroyed gamecontroller");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

    }


    public void InitGame(
        //GameType gameType
        )
    {
        //Debug.LogFormat("GameController.InitGame(): " + gameType);

        OnInit();

        gameConfig.Reset();

        //switch (gameType)
        //{
        //    case GameType.NonARGame:
        //        SceneManager.LoadScene("NonARGame", LoadSceneMode.Single);
        //        NonARBagSettings();
        //        break;
        //    case GameType.ARKitGame:
        //        SceneManager.LoadScene("ARKitGame", LoadSceneMode.Single);
        //        ARBagSettings();
        //        break;

        //    case GameType.ARCoreGame:
        //        SceneManager.LoadScene("ARCoreGame", LoadSceneMode.Single);
        //        ARBagSettings();
        //        break;

        //}

    }


    void ARBagSettings() {
        foodConfig.throwForce = 0.07f;
        foodConfig.loftForce = 13f;
    }


    #region GameStateManager

    public void EnterGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                {
                    break;
                }

            case GameState.Init:
                {

                    OnInit();

                    break;
                }

            case GameState.Play:
                {


                    break;
                }



            case GameState.Pause:
                {



                    break;
                }

            case GameState.GameOver:
                {


                    break;
                }



        }

        CurrentGameState = gameState;
    }


    public GameState GetCurrentGameState()
    {
        return CurrentGameState;
    }

    #endregion

}

