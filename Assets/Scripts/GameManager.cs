using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

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

    [SerializeField] int score;

    [Header("Game Values")]
    [SerializeField] GameState m_gameState;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Player Values")]
    [SerializeField] int m_playerCount;
    [SerializeField] ForwardMovement m_playerForwardMovement;
    [SerializeField] int m_totalMovementPoints = 3;

    [Tooltip("Calculated using Chunk Bounds")]
    [SerializeField] float _movementDistance = 3.33f;//Calculated using Chunk Bounds

    [Header(" Movement")]

    [SerializeField] float envMovementSpeed = 0.5f;
    [SerializeField] float speedIncreasePerPoint = 0.1f;
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

    [SerializeField] Vector3[] MovementSpawnPositions;




    public event Action<float> OnEnvValueChanged;
    private Action<GameState> onChangedGameState;

    public float GetGlobalSpeed()
    {
        return envMovementSpeed;
    }

    private void OnValidate()
    {

    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.LogWarning($"Distance Set to: {_movementDistance}");
    }

    #region Score
    public void IncrementScore()
    {
        score++;
        scoreText.text = score.ToString();
        // Increase the player's speed
        //m_playerForwardMovement.speed += m_playerForwardMovement.speedIncreasePerPoint;
        EnvMovementSpeed += speedIncreasePerPoint;
    }

    public int GetScore()
    {
        return score;
    }

    #endregion

    #region Spawn and Movement
    public int GetTotalMovementPoints => m_totalMovementPoints;
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