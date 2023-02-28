using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    MeshRenderer m_renderer;
    BoxCollider m_collider;
    [Space]
    [SerializeField] float offset;
    [SerializeField] Vector3 origin;
    [Space]
    [Header("Grid")]
    [SerializeField] int row;
    [SerializeField] int column;
    Vector3[,] points;
    [Space]
    [Header("Debug")]
    [SerializeField] float d_sphereSize = 0.3f;
    [SerializeField] Color d_color = Color.white;
    [Space]
    [Header("Block Visualizer")]
    [SerializeField] bool showNextBlockPreview = true;
    [SerializeField] int previewLimit = 2;
    [SerializeField] Vector3 previewOffset;
    [SerializeField] Color previewColor = Color.white;

    public float maxXBounds;
    public float maxYBounds;

    private void OnValidate()
    {
        if (m_renderer == null)
            m_renderer = GetComponent<MeshRenderer>();
        if (m_collider == null)
            m_collider = GetComponent<BoxCollider>();

        points = new Vector3[row, column];
        var pointDiffX = m_collider.bounds.max.x / row;
        var pointDiffY = m_collider.bounds.max.z / row;

        maxXBounds = m_renderer.bounds.max.x;
        maxYBounds = m_renderer.bounds.max.z;

        for (int pointXIndex = 0; pointXIndex < row; pointXIndex++)
        {
            for (int pointYIndex = 0; pointYIndex < column; pointYIndex++)
            {
                var xVal = offset - pointXIndex * pointDiffX;
                var yVal = pointYIndex * pointDiffY;
                points[pointXIndex, pointYIndex] = new Vector3(xVal, yVal);
            }
        }
        Debug.Log($"total Values loaded: {points.Length}");
    }

    [ContextMenu("Show Values")]
    public void ShowValues()
    {
        string info = "";
        for (int xIndex = 0; xIndex < row; xIndex++)
        {
            for (int yIndex = 0; yIndex < column; yIndex++)
            {

                info += $" {points[xIndex, yIndex]}";
            }
            info += "\n";
        }
        Debug.Log(info);
    }

    //Get Spawn Points
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    #region Debugger

    private void OnDrawGizmos()
    {
        ShowSpawnPointVisualizer();
        ShowNextBlockVisualizer();
    }

    void ShowSpawnPointVisualizer()
    {
        Gizmos.color = d_color;
        foreach (var item in points)
        {
            Gizmos.DrawSphere(new Vector3(item.x, 0, item.y), d_sphereSize);
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
                Gizmos.DrawCube(transform.position + new Vector3(transform.position.x, transform.position.y, m_collider.size.z * i) , m_collider.size);
            }
        }
    }
    #endregion

}