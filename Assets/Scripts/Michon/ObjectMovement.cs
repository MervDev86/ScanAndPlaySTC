using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [Header("Value")]
    //[SerializeField] Vector3 speedDelta;
    [SerializeField] float speedDelta;

    private void Start()
    {
        GameManager.instance.OnEnvValueChanged += onGlobalSpeedChanged;
        GameManager.instance.onChangedGameState += OnGameEnded;
        speedDelta = GameManager.instance.GetGlobalSpeed();
    }

    private void onGlobalSpeedChanged(float p_val)
    {
        speedDelta = p_val;
    }  
    
    private void OnGameEnded(GameState p_val)
    {
        if (p_val.Equals(GameState.GAME_END))
        {
            speedDelta = 0;
        }
    }

    void FixedUpdate()
    {
        //transform.Translate(speedDelta);
        transform.Translate(new Vector3(0,0,-speedDelta));
    }
}
