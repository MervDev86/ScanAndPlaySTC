using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] Vector3 speedDelta;
    //[Header("Debugger")]
    //[SerializeField] GameObject endOfSegmentIndicator;

    private void Start()
    {
        //endOfSegmentIndicator.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(speedDelta);
    }
}
