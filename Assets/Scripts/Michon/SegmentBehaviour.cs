using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    //[Header("Debugger")]
    [SerializeField] GameObject endOfSegmentIndicator;
    [SerializeField] bool showEndSegment = false;
    private void OnTriggerExit(Collider other)
    {
        SegmentManager.Instance.SpawnTile(false);
        Destroy(gameObject, 2);
    }
    private void Start()
    {
#if !UNITY_EDITOR
    showEndSegment = false;
#endif
        endOfSegmentIndicator.SetActive(showEndSegment);
    }


}