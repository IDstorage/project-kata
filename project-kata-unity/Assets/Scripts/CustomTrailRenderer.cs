using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class CustomTrailRenderer : CustomBehaviour
{
    [SerializeField] private int xSize = 30, ySize = 1;
    [SerializeField] private float size = 1F;
    [SerializeField] private float gap = 1F;
    [SerializeField] private float duration;

    private GameObject target;
    private Mesh mesh;

    private Vector3 previousPosition;
    private Vector3[] previousPositions;

    public int Length { get; private set; }


    public void InitializeMesh(Transform standard)
    {
        target = new GameObject("Mesh");
        //target.transform.SetParent(transform, false);
        var meshFilter = target.AddComponent<MeshFilter>();
        target.AddComponent<MeshRenderer>();

        mesh = new Mesh();

        var vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = Vector3.zero;
        }
        mesh.vertices = vertices;

        var triangles = new int[xSize * ySize * 6];
        for (int i = 0, vIdx = 0; i < triangles.Length; i += 6, ++vIdx)
        {
            for (int j = 0; j < ySize; ++j, ++vIdx)
            {
                triangles[i] = vIdx;
                triangles[i + 1] = triangles[i + 4] = vIdx + 1;
                triangles[i + 2] = triangles[i + 3] = ySize + vIdx + 1;
                triangles[i + 5] = ySize + vIdx + 2;
            }
        }
        mesh.triangles = triangles;

        Vector3 top = standard.up * size * 0.5f;
        Vector3 bottom = -standard.up * size * 0.5f;
        previousPositions = new Vector3[ySize + 1];
        for (int i = 0; i <= ySize; ++i)
        {
            mesh.vertices[i] = Vector3.Lerp(top, bottom, (float)i / ySize);
        }

        var normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = Vector3.back;
        }
        mesh.normals = normals;

        meshFilter.mesh = mesh;
    }

    public void UpdateVertices(Transform standard)
    {
        if (mesh == null) return;

        Vector3 top = standard.up * size * 0.5f;
        Vector3 bottom = -standard.up * size * 0.5f;

        var verts = mesh.vertices;

        bool updatePosition = false;
        for (int i = 0; i <= ySize; ++i)
        {
            var comparePos = standard.position + Vector3.Lerp(top, bottom, (float)i / ySize);
            var delta = comparePos - previousPositions[i];

            if (delta.magnitude >= gap) updatePosition = true;
        }

        //if (!updatePosition) return;

        Length = Mathf.Min(Length + 1, xSize - 1);

        for (int i = verts.Length - 1; i >= (ySize + 1) * 2; --i)
        {
            var initial = Vector3.Lerp(top, bottom, (float)(i % (ySize + 1)) / ySize);
            verts[i] = verts[i - (ySize + 1)] - (standard.position + initial - previousPositions[i % (ySize + 1)]);
        }

        for (int i = ySize + 1; i < (ySize + 1) * 2; ++i)
        {
            var initial = Vector3.Lerp(top, bottom, (float)i / ySize);
            verts[i] = previousPositions[i - (ySize + 1)] - standard.position;
        }

        // Update
        for (int i = 0; i <= ySize; ++i)
        {
            verts[i] = Vector3.Lerp(top, bottom, (float)i / ySize);
            previousPositions[i] = standard.position + verts[i];
        }

        mesh.vertices = verts;
    }

    public void UpdateNormals(Transform standard)
    {
        if (mesh == null) return;

        mesh.RecalculateNormals();
    }

    public void UpdateTangents()
    {
        if (mesh == null) return;

        mesh.RecalculateTangents();
    }


    private void Start()
    {
        InitializeMesh(transform);

        Anomaly.Utils.SmartCoroutine.Create(CoUpdate());
    }

    private IEnumerator CoUpdate()
    {
        WaitForSeconds ws = new WaitForSeconds(1F);

        while (true)
        {
            target.transform.position = transform.position;
            UpdateVertices(transform);
            UpdateNormals(transform);
            UpdateTangents();
            yield return null;
        }
    }

    private void OnUpdate()
    {

    }


    private void OnDrawGizmos()
    {
        if (mesh == null) return;
        for (int i = 0; i < mesh.vertices.Length; ++i)
        {
            Color prev = Gizmos.color;
            float c = (float)(i - ySize) / (mesh.vertices.Length - ySize);
            if (i <= ySize) Gizmos.color = Color.red;
            else Gizmos.color = new Color(c, c, c, 1F);
            Gizmos.DrawSphere(transform.position + mesh.vertices[i], i <= ySize ? 0.2f : 0.1f);
            Gizmos.color = prev;
        }
    }
}
