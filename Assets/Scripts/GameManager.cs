using UnityEngine;
using System.Collections;
using System;
using NetworkClientHandler;
using UnityEngine.SceneManagement;
using DG.Tweening; 

public enum GameState
{
    MAIN_MENU,
    WAITING,
    GAME_STARTED,
    GAME_PLAYING,
    GAME_OVER,
    LEADERBOARD,
}

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event Action<float> OnEnvValueChanged;
    public Action<GameState> onChangedGameState;
    
    [Header("Game Values")]
    [SerializeField] GameState m_gameState;

    [Tooltip("Calculated using Chunk Bounds")]
    [SerializeField] float _movementDistance = 3.33f;//Calculated using Chunk Bounds


    [SerializeField] private PlayerGameHandler m_playerHandler1;
    [SerializeField] private PlayerGameHandler m_playerHandler2;
    [SerializeField] private Leaderboards m_leaderBoard;
    [SerializeField] float m_maxTime = 60;
    [SerializeField] float m_startingSpeed = 1f;

    [SerializeField] GameObject m_introPanel;

    private bool m_isSinglePlayer = true;

    string[] m_config;
    [SerializeField] float m_gameOverShowTime = 5;
    [SerializeField] float m_leaderboardShowTime = 5;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        m_config = System.IO.File.ReadAllLines(Application.dataPath + "/StreamingAssets/GameConfig.ini");
        m_maxTime = float.Parse(m_config[0].Split("=")[1]);
        m_gameOverShowTime = float.Parse(m_config[1].Split("=")[1]);
        m_leaderboardShowTime = float.Parse(m_config[2].Split("=")[1]);

        ChangeGameState(GameState.MAIN_MENU);
        m_leaderBoard.gameObject.SetActive(false);
        SessionsHandler.OnInitializeGame += OnInitializeGame;
        SessionsHandler.OnStartGame += StartGame;
        SessionsHandler.OnRestartGame += Restart;
        SessionsHandler.OnPlayer1SetName += m_playerHandler1.SetPlayerReady;
        SessionsHandler.OnPlayer2SetName += m_playerHandler2.SetPlayerReady;
        SessionsHandler.OnMovePlayer1 += m_playerHandler1.playerControl.MoveToPositionIndex;
        SessionsHandler.OnMovePlayer2 += m_playerHandler2.playerControl.MoveToPositionIndex;
    }

    private void OnDestroy()
    {
        SessionsHandler.OnInitializeGame -= OnInitializeGame;
        SessionsHandler.OnStartGame -= StartGame;
        SessionsHandler.OnRestartGame -= Restart;
        SessionsHandler.OnPlayer1SetName -= m_playerHandler1.SetPlayerReady;
        SessionsHandler.OnPlayer2SetName -= m_playerHandler2.SetPlayerReady;
        SessionsHandler.OnMovePlayer1 -= m_playerHandler1.playerControl.MoveToPositionIndex;
        SessionsHandler.OnMovePlayer2 -= m_playerHandler2.playerControl.MoveToPositionIndex;
    }

    private void Update()
    {
        if (m_gameState == GameState.GAME_PLAYING)
        {
            if (m_playerHandler1.currentState == PlayerStatus.Gameover)
            {
                if (m_isSinglePlayer || (!m_isSinglePlayer && m_playerHandler2.currentState == PlayerStatus.Gameover))
                {
                    ChangeGameState(GameState.GAME_OVER);
                    //Debug.Log("GAME OVER");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {  
            Restart();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {  
            Application.Quit();
        }
        
        //For Debug Only
        if (Input.GetKeyDown(KeyCode.F1)) //Start 1 player game
        {
            OnInitializeGame(1);
            m_introPanel.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.F2)) //Start 2 player game
        {
            OnInitializeGame(2);
            m_introPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.F3)) //Simulate Player1 Ready
        {
            m_playerHandler1.SetPlayerReady("Mabin");
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.F4)) //Simulate Player1 Ready
        {
            m_playerHandler2.SetPlayerReady("Rainne");
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.F5)) //Simulate Player1 Ready
        {
            m_playerHandler1.GameOver();
            if (!m_isSinglePlayer)
                m_playerHandler2.GameOver();
        }

        if (Input.GetKeyDown(KeyCode.Z)) //Simulate Player1 Ready
        {
            m_playerHandler1.AddPoints(5);
        }

        if (Input.GetKeyDown(KeyCode.X)) //Simulate Player1 Ready
        {
            m_playerHandler2.AddPoints(5);
        }
    }

    void StartGame()
    {
        if (m_isSinglePlayer && m_playerHandler1.currentState == PlayerStatus.Ready)
        {
            m_playerHandler1.StartGame();
        }
        else if (!m_isSinglePlayer 
            && m_playerHandler1.currentState == PlayerStatus.Ready 
            && m_playerHandler2.currentState == PlayerStatus.Ready) //Start game if both players are ready
        {
            m_playerHandler1.StartGame();
            m_playerHandler2.StartGame();
        }
        ChangeGameState(GameState.GAME_PLAYING);
    }



    public void OnInitializeGame(int m_playerCount) //TODO listen to server to start the game
    {
        m_isSinglePlayer = m_playerCount == 1;
        if (m_playerCount == 1)
        {
            m_playerHandler1.InitGame(m_isSinglePlayer,m_maxTime,m_startingSpeed);
            m_playerHandler2.ResetPlayer();
        }
        else
        {
            m_playerHandler1.InitGame(m_isSinglePlayer, m_maxTime, m_startingSpeed);
            m_playerHandler2.InitGame(m_isSinglePlayer, m_maxTime, m_startingSpeed);
        }
        ChangeGameState(GameState.GAME_STARTED);
    }
    

    #region Spawn and Movement
    public float GetMovementDistance => _movementDistance;
    #endregion

    #region GameState

    public GameState GetCurrentGameState()
    {
        return m_gameState;
    }

    public void ChangeGameState(GameState p_gameState)
    {
        m_gameState = p_gameState;
        onChangedGameState?.Invoke(p_gameState);

        switch (p_gameState)
        {
            case GameState.MAIN_MENU:
                m_introPanel.SetActive(true);
                break;

            case GameState.WAITING:
                m_introPanel.SetActive(false);
                break;
                
            case GameState.GAME_STARTED:
                m_introPanel.SetActive(false);
                break;

            case GameState.GAME_PLAYING:
                break;

            case GameState.GAME_OVER:
                StartCoroutine(SaveScoreAndShowLeaderBoards());
                break;

            default:
                break;
        }
    }

    IEnumerator SaveScoreAndShowLeaderBoards()
    {
        //Debug.Log("Save2coreAndShowLeaderBoards");
        m_leaderBoard.SaveScore(m_playerHandler1.playerName,"0", m_playerHandler1.score.ToString());
        if(!m_isSinglePlayer)
            m_leaderBoard.SaveScore(m_playerHandler2.playerName, "0", m_playerHandler2.score.ToString());
        yield return new WaitForSeconds(m_gameOverShowTime);
        ChangeGameState(GameState.LEADERBOARD);
        //Debug.Log("Show leaderboards");
        m_leaderBoard.gameObject.SetActive(true);
        m_leaderBoard.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        m_leaderBoard.InitLeaderboard();
        yield return new WaitForSeconds(m_leaderboardShowTime);
        Restart();
    }
    
    void Restart()
    {
        SceneManager.LoadScene(0);
    }
    
    #endregion

    #region Debug

    [ExecuteInEditMode]
    public void DistanceBounds(float p_distance)
    {
        _movementDistance = p_distance;
        Debug.LogWarning($"Distance Bound Set : {p_distance}");
    }

    #endregion
}