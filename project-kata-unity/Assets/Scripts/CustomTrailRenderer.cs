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
    [SerializeField] private Material[] materials;

    [SerializeField] private List<Vector3> offsets = new List<Vector3>();

    private GameObject target;
    private Mesh mesh;

    private Vector3[] previousPositions;

    public int Length { get; private set; }


    public void InitializeMesh(Transform standard)
    {
        target = new GameObject("Mesh");
        //target.transform.SetParent(transform, false);
        var meshFilter = target.AddComponent<MeshFilter>();
        var meshRenderer = target.AddComponent<MeshRenderer>();

        mesh = new Mesh();

        InitMeshProperties();

        meshFilter.mesh = mesh;
        meshRenderer.materials = materials;
    }

    public void InitMeshProperties()
    {
        var vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = Vector3.zero;
        }
        mesh.vertices = vertices;

        var triangles = new int[xSize * ySize * 6];
        for (int i = 0, tIdx = 0, vIdx = 0; tIdx < triangles.Length; ++i, ++vIdx)
        {
            for (int j = 0; j < ySize; ++j, ++vIdx, tIdx += 6)
            {
                triangles[tIdx] = vIdx;
                triangles[tIdx + 1] = triangles[tIdx + 4] = vIdx + 1;
                triangles[tIdx + 2] = triangles[tIdx + 3] = ySize + vIdx + 1;
                triangles[tIdx + 5] = ySize + vIdx + 2;
            }
        }
        mesh.triangles = triangles;

        previousPositions = new Vector3[ySize + 1];

        var normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = Vector3.back;
        }
        mesh.normals = normals;

        var uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; ++i)
        {
            uvs[i] = new Vector2((i / (ySize + 1)) / (float)xSize, (i % (ySize + 1)) / (float)ySize);
        }
        mesh.uv = uvs;

        for (int i = 0; i <= ySize; ++i)
        {
            offsets.Add(Vector3.zero);
        }
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
            var comparePos = standard.position + Vector3.Lerp(top, bottom, (float)i / ySize);// + offsets[i];
            var delta = comparePos - previousPositions[i];

            if (delta.magnitude >= gap) updatePosition = true;
        }

        //if (!updatePosition) return;

        Length = Mathf.Min(Length + 1, xSize - 1);

        for (int i = verts.Length - 1; i >= (ySize + 1) * 2; --i)
        {
            var initial = Vector3.Lerp(top, bottom, (float)(i % (ySize + 1)) / ySize);
            verts[i] = verts[i - (ySize + 1)] - (standard.position + initial - previousPositions[i % (ySize + 1)]);// + offsets[i % (ySize + 1)];
        }

        for (int i = ySize + 1; i < (ySize + 1) * 2; ++i)
        {
            var initial = Vector3.Lerp(top, bottom, (float)i / ySize);
            verts[i] = previousPositions[i - (ySize + 1)] - standard.position;// + offsets[i - (ySize + 1)];
        }

        // Update
        for (int i = 0; i <= ySize; ++i)
        {
            verts[i] = Vector3.Lerp(top, bottom, (float)i / ySize);
            previousPositions[i] = standard.position + verts[i];// + offsets[i];
            verts[i] += offsets[i];
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
