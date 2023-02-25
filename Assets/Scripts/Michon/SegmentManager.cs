using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    private static SegmentManager _instance;
    public static SegmentManager Instance => _instance;

    [SerializeField] int totalSpawn = 15;

    [SerializeField] GameObject segmentParent;
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] Vector3 nextSpawnPoint;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Start()
    {
        for (int i = 0; i < totalSpawn; i++)
        {
            if (i < 3)
            {
                SpawnTile(false);
            }
            else
            {
                SpawnTile(true);
            }
        }
    }
    public void SpawnTile(bool spawnItems)
    {
        GameObject temp = Instantiate(segmentPrefab, nextSpawnPoint, Quaternion.identity);
        temp.transform.parent = segmentParent.transform;
        nextSpawnPoint = temp.transform.GetChild(1).transform.position;

        if (spawnItems)
        {
            //temp.GetComponent<GroundTile>().SpawnObstacle();
            //temp.GetComponent<GroundTile>().SpawnCoins();
        }
    }
}
