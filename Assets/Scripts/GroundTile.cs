using UnityEngine;

public class GroundTile : MonoBehaviour
{
    GroundSpawner groundSpawner;
    Collider m_collider;

    [SerializeField] GameObject coinPrefab;
    [SerializeField] GameObject obstaclePrefab;

    [Header("Values")]
    [SerializeField] int coinsToSpawn = 10;
    [SerializeField] int m_totalSpawnPoints = 3;
    [SerializeField] float m_offset = 0.5f;
    [Header("Debug")]
    [SerializeField] Vector3 calculatedPoint;
    [SerializeField] float indicatorSize = 0.5f;

    #region lifecycles

    private void OnValidate()
    {
        if (m_collider == null)
        {
            m_collider = GetComponent<Collider>();
        }
    }
    private void OnEnable()
    {

    }

    private void Awake()
    {
        m_collider = GetComponent<Collider>();

    }

    private void Start()
    {
        groundSpawner = GameObject.FindObjectOfType<GroundSpawner>();
    }
    #endregion

    public void SpawnObstacle()
    {
        // Choose a random point to spawn the obstacle
        int obstacleSpawnIndex = Random.Range(2, 5);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex).transform;

        // Spawn the obstace at the position
        Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity, transform);
    }

    public void SpawnCoins()
    {
        for (int i = 0; i < coinsToSpawn; i++)
        {
            GameObject coinTemp = Instantiate(coinPrefab, transform);
            coinTemp.transform.position = GetRandomPointInCollider(GetComponent<Collider>());
        }
    }

    Vector3 GetRandomPointInCollider(Collider collider)
    {
        //Vector3 point = new Vector3(
        //    GetXPosition(Random.Range(0, 3)),
        //    1,
        //    Random.Range(collider.bounds.min.z, collider.bounds.max.z)
        //    );

        Vector3 point = new Vector3(
           GetXPosition(Random.Range(0, 3)),
           1,
           GetZPosition(Random.Range(0, 3))
           );

        if (point != collider.ClosestPoint(point))
        {
            point = GetRandomPointInCollider(collider);
        }
        return point;
    }

    public float GetXPosition(int p_posValue)
    {
        var pointDistance = (m_collider.bounds.max.x - m_collider.bounds.min.x) / m_totalSpawnPoints;
        var calculatedXPos = (m_collider.bounds.min.x + (p_posValue * pointDistance)) + m_offset;
        return calculatedXPos;
    }

    public float GetZPosition(int p_posValue)
    {
        var pointDistance = (m_collider.bounds.max.z - m_collider.bounds.min.z) / 15;
        var calculatedZPos = (m_collider.bounds.min.z + (p_posValue * pointDistance)) + m_offset;
        return calculatedZPos;
    }

    private void OnTriggerExit(Collider other)
    {
        //groundSpawner.SpawnTile(true);
        //Destroy(gameObject, 2);
    }

    #region DEBUG

    [ContextMenu("Get Bounds Info")]
    public void GetBoundsInfo()
    {
        var collider = GetComponent<Collider>();

        Debug.LogWarning($"{gameObject.name} " +
        $"x min:{collider.bounds.min.x } max: {collider.bounds.max.x} \n" +
        $"y min:{collider.bounds.min.y } max: {collider.bounds.max.y} \n" +
        $"z min:{collider.bounds.min.z } max: {collider.bounds.max.z} ");

        GameManager.instance.DistanceBounds((m_collider.bounds.max.x - m_collider.bounds.min.x) / m_totalSpawnPoints);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (m_collider != null)
        {
            var pointDistance = (m_collider.bounds.max.x - m_collider.bounds.min.x) / m_totalSpawnPoints;
            var position = m_offset + m_collider.bounds.min.x + (1 * pointDistance);

            Gizmos.DrawSphere(new Vector3(position, transform.position.y + 1), indicatorSize);
        }
    }

    #endregion
}