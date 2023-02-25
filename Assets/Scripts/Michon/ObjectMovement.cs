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
        speedDelta = GameManager.instance.GetGlobalSpeed();
    }

    private void onGlobalSpeedChanged(float p_val)
    {
        speedDelta = p_val;
    }

    void FixedUpdate()
    {
        //transform.Translate(speedDelta);
        transform.Translate(new Vector3(0,0,-speedDelta));
    }
}
