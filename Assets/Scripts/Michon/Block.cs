using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Block : MonoBehaviour
{
    [SerializeField] BoxCollider m_collider;

    [Header("Spawner")]
    [SerializeField] GameObject m_coinPrefab;
    [SerializeField] GameObject m_ObstaclePrefab;
    [SerializeField] Transform originTransform;
    [SerializeField] int obstacleSpawnLimit = 1;
    [Space]

    [Header("Grid")]
    //xPoints is supposed to be based off the movement points of the player  | l | m = 0 | r | 
    [SerializeField] float[] xPoints = new float[3];
    [SerializeField] int column = 3;//DEFAULT = 3  => FOR NOW KEEP THE SAME VAL
    [SerializeField] int row;
    [Space]
    [SerializeField] float zOffset;
    Vector3[,] spawnPoints;
    [Space]

    [Header("Item Spawn Visualizer")]
    [SerializeField] float d_sphereSize = 0.3f;
    [SerializeField] Color d_color = Color.white;
    [Space]

    [Header("Block Visualizer")]
    [SerializeField] bool ShowDebugTools = true;
    [SerializeField] bool showNextBlockPreview = true;
    [SerializeField] int previewLimit = 2;
    [SerializeField] Color previewColor = Color.white;

    public float maxXBounds;
    public float maxYBounds;
    public static float zSize = 0;
    private void OnValidate()
    {
        if (m_collider == null)
            m_collider = GetComponent<BoxCollider>();

        InitGridLayout();
    }

    private void Start()
    {
        m_collider = this.gameObject.GetComponent<BoxCollider>();

        InitGridLayout();
        SpawnItems();
    }

    private void Update()
    {
        InitGridLayout();
    }

    [ContextMenu("Block/Check Collider Length")]
    public void GetColliderLength()
    {
        zSize = GetComponent<BoxCollider>().bounds.size.z;
    }

    void InitGridLayout()
    {
        //GetXpoints();
        spawnPoints = new Vector3[row, column];

        //var pointDiffX = m_collider.bounds.max.x / row;

        if (zSize <= 0)
        {
            zSize = m_collider.size.z;
            Debug.Log($"zSize has been set to {zSize}");
        }
        //maxXBounds = m_collider.bounds.max.x;
        zSize = m_collider.bounds.size.z;
        var pointDiffY = zSize / column;

        Debug.LogWarning($"{gameObject.transform.parent.transform.parent}  using Max Z Bounds: {zSize} \n" +
            $"calculated spawn distance = {pointDiffY}");

        for (int pointXIndex = 0; pointXIndex < row; pointXIndex++)
        {
            for (int pointYIndex = 0; pointYIndex < column; pointYIndex++)
            {
                //var xVal = offset - pointXIndex * pointDiffX;
                var yVal = (pointYIndex * pointDiffY) + zOffset;
                //spawnPoints[pointXIndex, pointYIndex] = new Vector3(xPoints[pointXIndex], transform.position.y, yVal);
                spawnPoints[pointXIndex, pointYIndex] = new Vector3(xPoints[pointXIndex], 0, yVal);
            }
        }
        //Debug.Log($"total Values loaded: {points.Length}");
    }

    void GetXpoints()
    {
        var playerMovementPointDelta = GameManager.instance.GetMovementDistance;
        for (int xPointIndex = 0; xPointIndex < row; xPointIndex++)
        {
            xPoints[xPointIndex] = xPointIndex * playerMovementPointDelta;
        }
    }

    [ContextMenu("Spawnables/Spawn Collections")]
    void SpawnItems()
    {
        foreach (var spawnPosition in spawnPoints)
        {
            var obj = Instantiate(m_coinPrefab, transform);
            obj.transform.localPosition = spawnPosition;
        }
    }

    #region Debugger

    private void OnDrawGizmos()
    {
        if (!ShowDebugTools)
            return;
        ShowSpawnPointVisualizer();
        ShowNextBlockVisualizer();
    }

    void ShowSpawnPointVisualizer()
    {
        Gizmos.color = d_color;
        foreach (var item in spawnPoints)
        {

            Gizmos.DrawSphere(transform.position + item, d_sphereSize);
        }
    }

    void ShowNextBlockVisualizer()
    {
        if (showNextBlockPreview)
        {
            Gizmos.color = previewColor;
            for (int i = 0; i < previewLimit; i++)
            {
                //Gizmos.DrawCube(transform.position , m_collider.size);
                Gizmos.DrawCube(transform.position + new Vector3(transform.position.x, transform.position.y, m_collider.size.z * i), m_collider.size);
            }
        }
    }

    [ContextMenu("Show Values")]
    public void ShowValues()
    {
        string info = "";
        for (int xIndex = 0; xIndex < row; xIndex++)
        {
            for (int yIndex = 0; yIndex < column; yIndex++)
            {

                info += $" {spawnPoints[xIndex, yIndex]}";
            }
            info += "\n";
        }
        Debug.Log(info);
    }
    #endregion

}