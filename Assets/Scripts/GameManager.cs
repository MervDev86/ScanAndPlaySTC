using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using NetworkClientHandler;
using UnityEngine.SceneManagement;

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

    [SerializeField] GameObject m_introPanel;

    private bool m_isSinglePlayer = true; 
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeGameState(GameState.MAIN_MENU);
        SessionsHandler.OnInitializeGame += OnInitializeGame;
        SessionsHandler.OnPlayer1SetName += m_playerHandler1.SetPlayerReady;
        SessionsHandler.OnPlayer2SetName += m_playerHandler2.SetPlayerReady;
        SessionsHandler.OnMovePlayer1 += m_playerHandler1.playerControl.MoveToPositionIndex;
        SessionsHandler.OnMovePlayer2 += m_playerHandler2.playerControl.MoveToPositionIndex;
    }

    private void OnDestroy()
    {
        SessionsHandler.OnInitializeGame -= OnInitializeGame;
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
                    Debug.Log("GAME OVER");
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
            m_playerHandler1.AddScore(5);
        }

        if (Input.GetKeyDown(KeyCode.X)) //Simulate Player1 Ready
        {
            m_playerHandler2.AddScore(5);
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
            m_playerHandler1.InitGame(m_isSinglePlayer);
            m_playerHandler2.ResetPlayer();
        }
        else
        {
            m_playerHandler1.InitGame(m_isSinglePlayer);
            m_playerHandler2.InitGame(m_isSinglePlayer);
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

                break;
            case GameState.WAITING:

                break;
            case GameState.GAME_STARTED:

                break;
            case GameState.GAME_PLAYING:
                
                break;
            case GameState.GAME_OVER:

                break;
            default:
                break;
        }
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