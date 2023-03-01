using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    [Header("Spawn Values")]
    [Tooltip("Gameobject that handled the Spawning of this Object")]
    [SerializeField] GameObject segmentParent;
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] int totalSpawn = 15;
    [SerializeField] Vector3 nextSpawnPoint;
    [Space]
    [Tooltip("If theres already a preset spawn enabled")]
    [SerializeField] GameObject initSpawn;
    [Space]
    [Tooltip("Found in Prefab as 'Next Point'. Sets where the next spawn object will be.")]
    [SerializeField] float m_spawnPositionDiff;

    private void OnValidate()
    {
        m_spawnPositionDiff = segmentPrefab.transform.GetChild(1).transform.position.z;
    }

    private void Start()
    {
        for (int spawnIndex = 0; spawnIndex <= totalSpawn; spawnIndex++)
        {
            if (initSpawn != null && spawnIndex < 1)
            {
                nextSpawnPoint = initSpawn.transform.GetChild(1).transform.position;
                continue;
            }
            SpawnTile(spawnIndex);
        }
    }

    public void SpawnTile(int? p_name = null)
    {
        GameObject temp = Instantiate(segmentPrefab, nextSpawnPoint, Quaternion.identity);
        temp.GetComponent<SegmentBehaviour>().SetSpawnParent(this);
        temp.transform.parent = segmentParent.transform;
        temp.name = p_name != null ? $"Segment {p_name} " : "Segment";
        nextSpawnPoint = temp.transform.GetChild(1).transform.position;
    }

    public float GetLastSpawnPosition() => (totalSpawn) * m_spawnPositionDiff;
    public int GetTotalSpawnCount() => totalSpawn;
    public float GetSpawnPositionDifference() => m_spawnPositionDiff;

}