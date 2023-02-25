using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject m_startMenu;
    [SerializeField] GameObject m_endScreenMenu;

    [SerializeField] GameObject playerLifeParent;
    [SerializeField] Transform[] playerLiveObjs;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        playerLiveObjs = playerLifeParent.GetComponentsInChildren<Transform>();
    }

    public void OnLifeChange()
    {

    }

    public void StartGame()
    {
        GameManager.instance.ChangeGameState(GameState.GAME_START);
    }

    public void ShowEndScreen()
    {
        GameManager.instance.ChangeGameState(GameState.GAME_END);
    }
}
