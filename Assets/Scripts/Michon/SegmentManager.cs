using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    private static SegmentManager _instance;
    public static SegmentManager Instance;

    [SerializeField] GameObject segmentPrefab;
    [SerializeField] Vector3 segmentSpawnOffset;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
