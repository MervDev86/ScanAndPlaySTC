using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{ 
    [SerializeField] int totalSpawn = 3;
    [SerializeField] Transform segmentParent;
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] Vector3 nextSpawnPoint;
    
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
        GameObject temp = Instantiate(segmentPrefab,segmentParent);
        temp.transform.localPosition = nextSpawnPoint;
        nextSpawnPoint = temp.transform.GetChild(1).transform.localPosition;

        if (spawnItems)
        {
            //temp.GetComponent<GroundTile>().SpawnObstacle();
            //temp.GetComponent<GroundTile>().SpawnCoins();
        }
    }
}
