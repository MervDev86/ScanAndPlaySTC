﻿using UnityEngine;
using System;
using System.Collections;

public class PlayerGameHandler : MonoBehaviour
{
    public PlayerStatus currentState = PlayerStatus.Idle;
    public PlayerControl playerControl;
    public SegmentManager segmentManager;
    public int score = 0;

    public string playerName = "PLAYER";
    [Header("HUD")]
    [SerializeField] PlayerHud m_playerHUD;
    [SerializeField] float m_currentTime = 0;
    [SerializeField] float m_maxTime = 60;
    [SerializeField] float m_multiplier = 1;
    [SerializeField] float m_startingSpeed = 0.5f;
    
    
    int m_life = 3;
    bool m_isActive = false;
    private bool m_isSingle = true;

    private void Start()
    {
        GameManager.instance.onChangedGameState += OnGameStateChange;
    }
    
    private void OnDestroy()
    {
        GameManager.instance.onChangedGameState -= OnGameStateChange;
    }

    public void InitPlayer(bool p_isSingle)
    {
        m_isSingle = p_isSingle;

        score = 0;
        m_playerHUD.SetScore(score);
        m_playerHUD.gameObject.SetActive(true);
        segmentManager.EnvMovementSpeed = 0;
        m_isActive = true;
        m_playerHUD.InitHud(p_isSingle);
        currentState = PlayerStatus.Waiting;
    }

    void OnGameStateChange(GameState p_gameState)
    {
        if(!m_isActive)
            return;
        switch (p_gameState)
        {
            case GameState.MAIN_MENU:
                break;
            case GameState.WAITING:
                break;
            case GameState.GAME_STARTED:
                // StartGame();
                break;
            case GameState.GAME_PLAYING:
                break;
            case GameState.GAME_OVER:
                break;
            case GameState.LEADERBOARD:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_gameState), p_gameState, null);
        }
    }

    IEnumerator StartGameSeq()
    {
        m_playerHUD.SetIntroPanel("3");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("2");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("1");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("GO!",180);
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("");
        playerControl.ActivateController(true);
        currentState = PlayerStatus.Playing;
        segmentManager.EnvMovementSpeed = m_startingSpeed;
    }

    private void Update()
    {
        if (currentState == PlayerStatus.Playing)
        {
            m_currentTime += Time.deltaTime * m_multiplier;
            m_playerHUD.SetGameTime(m_currentTime);
            if (m_currentTime >= m_maxTime)
            {
                GameOver();
            }
        }
    }

    public void SetPlayerReady(string p_name)
    {
        m_playerHUD.SetIntroPanel("Ready!",120);
        m_playerHUD.SetGameTime(0);
        playerName = p_name;
        currentState = PlayerStatus.Ready;
        m_playerHUD.SetName(p_name);
    }

    public void StartGame()
    {
        m_currentTime = 0;
        currentState = PlayerStatus.Starting;
        StartCoroutine(StartGameSeq());
    }

    public void GameOver()
    {
        m_isActive = false;
        currentState = PlayerStatus.Gameover;
        playerControl.ActivateController(false);
        m_playerHUD.ShowFinalScore(playerName,score);
        segmentManager.EnvMovementSpeed = 0;
    }
    
    public void Hit()
    {
        if (m_isActive)
        {
            m_life--;
            if (m_life <= 0)
                Die();
            Debug.Log("Life: " + m_life);
        }
    }

    public void AddScore(int p_score)
    {
        score += p_score;
        m_playerHUD.SetScore(score);
    }
    
    public void ResetPlayer()
    {
        m_isActive = false;
        m_playerHUD.DeactivateHud();
        m_playerHUD.gameObject.SetActive(false);
    }

    public void Die()
    {
        m_isActive = false;
        Debug.Log("Player Died!");
    }
}

public enum PlayerStatus
{
    Idle,
    Waiting,
    Ready,
    Starting,
    Playing,
    Gameover,
}