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
    GAME_COUNTDOWN,
    GAME_START,
    GAME_END
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

    [Header("Movement")]
    [SerializeField] float envMovementSpeed = 0.5f;
    [SerializeField] float startingSpeed = 0.2f;
    [SerializeField] private Player m_player1;
    [SerializeField] private Player m_player2;

    private bool m_isSinglePlayer = true; 
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeGameState(GameState.MAIN_MENU);
        SessionsHandler.OnInitializeGame += OnInitializeGame;
        SessionsHandler.OnPlayer1SetName += m_player1.SetPlayerName;
        SessionsHandler.OnPlayer2SetName += m_player2.SetPlayerName;
        SessionsHandler.OnMovePlayer1 += m_player1.MoveToPositionIndex;
        SessionsHandler.OnMovePlayer2 += m_player2.MoveToPositionIndex;
    }

    private void OnDestroy()
    {
        SessionsHandler.OnInitializeGame -= OnInitializeGame;
    }

    private void Update()
    {
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
            m_player1.SetPlayerName("Mabin");
        }
        
        if (Input.GetKeyDown(KeyCode.F2)) //Start 2 player game
        {
            OnInitializeGame(2);
            m_player1.SetPlayerName("Mervin");
            m_player2.SetPlayerName("Rainne");
        }
        
    }

    public float EnvMovementSpeed
    {
        get { return envMovementSpeed; }
        set
        {
            if (value != envMovementSpeed)
            {
                envMovementSpeed = value;
                OnEnvValueChanged?.Invoke(value);
            }
        }
    }

    public void OnInitializeGame(int m_playerCount) //TODO listen to server to start the game
    {
        m_isSinglePlayer = m_playerCount == 1;
        if (m_playerCount == 1)
        {
            m_player1.InitPlayer(m_isSinglePlayer);
            m_player2.ResetPlayer();
        }
        else
        {
            m_player1.InitPlayer(m_isSinglePlayer);
            m_player2.InitPlayer(m_isSinglePlayer);
        }
    }

    public float GetGlobalSpeed()
    {
        return envMovementSpeed;
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
            case GameState.GAME_COUNTDOWN:
                EnvMovementSpeed = 0;
                break;
            case GameState.GAME_START:
                EnvMovementSpeed = startingSpeed;
                break;
            case GameState.GAME_END:
                // UIManager.Instance.ShowEndScreen();
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