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
    [Header("Debugger")]
    [SerializeField] float m_segmentSpawnPointSphereSize = 15f;
    [SerializeField] Vector3 m_segmentSpawnPoint;
    [SerializeField] Color m_segmentSpawnPointColor;

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
                //initSpawn.SetActive(false);
                initSpawn.GetComponent<SegmentBehaviour>().SetSpawnParent(this);
                nextSpawnPoint = initSpawn.transform.GetChild(1).transform.position;
                continue;
            }
            SpawnTile(spawnIndex, spawnIndex == totalSpawn);
        }
    }

    public void SpawnTile(int? p_name = null, bool p_isLast = false)
    {
        GameObject temp = Instantiate(segmentPrefab, nextSpawnPoint, Quaternion.identity);
        if (p_isLast)
        {
            m_segmentSpawnPoint = temp.transform.position;
        }


        temp.GetComponent<SegmentBehaviour>().SetSpawnParent(this);
        temp.transform.parent = segmentParent == null ? new GameObject("Segments").transform : segmentParent.transform;
        temp.name = p_name != null ? $"Segment {p_name} " : "Segment";
        nextSpawnPoint = temp.transform.GetChild(1).transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = m_segmentSpawnPointColor;
        m_segmentSpawnPoint = new Vector3(0, 0, GetLastSpawnPosition());
        Gizmos.DrawSphere(m_segmentSpawnPoint, m_segmentSpawnPointSphereSize);
    }

    public float GetLastSpawnPosition() => (totalSpawn) * m_spawnPositionDiff;
    public int GetTotalSpawnCount() => totalSpawn;
    public float GetSpawnPositionDifference() => m_spawnPositionDiff;

}