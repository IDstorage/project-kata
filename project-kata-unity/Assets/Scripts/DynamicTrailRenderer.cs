using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class DynamicTrailRenderer : CustomBehaviour
{
    public class VertexLine
    {
        //public List<Vector3> vertices = new List<Vector3>();
        public Vector3[] vertices;
        //public List<int> indices = new List<int>();
        public (int line, int size) indices;

        public Transform header;
        public Vector3 direction;
        public float distance;

        public Vector3 offset;

        public int Length => indices.size;

        public bool freeze = false;

        public void InitializePoints(Vector3[] vertices, int lineNumber, int size)
        {
            this.vertices = vertices;

            indices.line = lineNumber;
            indices.size = size;

            for (int i = 0; i < indices.size; ++i)
            {
                vertices[indices.size * indices.line + i] = header != null ? header.position + direction * distance + offset : Vector3.zero;
            }
        }

        public void Update()
        {
            if (vertices == null || vertices.Length == 0) return;

            if (!header.hasChanged) return;

            for (int i = indices.size - 1; i > 0; --i)
            {
                vertices[indices.size * indices.line + i] = vertices[indices.size * indices.line + i - 1];
            }

            vertices[indices.size * indices.line] = header.position + direction * distance + offset;
        }

        public void Clear()
        {
            for (int i = 1; i < indices.size; ++i)
            {
                vertices[indices.size * indices.line + i] = vertices[indices.size * indices.line];
            }
        }
    }

    [SerializeField, Min(0F)] private float width = 1F;
    [SerializeField, Range(10, 50)] private int xCount = 10;
    [SerializeField, Range(2, 7)] private int yCount = 2;

    [SerializeField] private int anchorLine = 0;

    [SerializeField] private Material[] materials;

    [SerializeField] private float interpolateThreshold = 1F;

    private List<VertexLine> vertexLines = new List<VertexLine>();

    private GameObject meshAttach;
    private Mesh mesh;

    private void CreateMeshIfNull()
    {
        if (meshAttach != null && mesh != null) return;

        meshAttach = new GameObject("DynamicTrailRenderer");

        var meshFilter = meshAttach.AddComponent<MeshFilter>();
        var meshRenderer = meshAttach.AddComponent<MeshRenderer>();

        mesh = new Mesh();

        meshFilter.mesh = mesh;
        meshRenderer.materials = materials;
    }

    private void InitializeMeshElements()
    {
        var vertices = new Vector3[xCount * yCount];
        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = Vector3.zero;
        }
        mesh.vertices = vertices;

        var triangles = new int[(xCount - 1) * (yCount - 1) * 6];
        for (int i = 0, tIdx = 0, vIdx = 0; tIdx < triangles.Length; ++i, ++vIdx)
        {
            for (int j = 0; j < xCount - 1; ++j, ++vIdx, tIdx += 6)
            {
                triangles[tIdx] = triangles[tIdx + 4] = vIdx;
                triangles[tIdx + 2] = triangles[tIdx + 5] = vIdx + xCount + 1;
                triangles[tIdx + 1] = vIdx + xCount;
                triangles[tIdx + 3] = vIdx + 1;
            }
        }
        mesh.triangles = triangles;

        var normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = Vector3.back;
        }
        mesh.normals = normals;

        var uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; ++i)
        {
            uvs[i] = new Vector2((float)(i % xCount) / (xCount - 1), (float)(i / xCount) / (yCount - 1));
        }
        mesh.uv = uvs;


        vertexLines.Clear();

        float deltaDistance = (float)width / (yCount - 1);

        for (int i = 0; i < yCount; ++i)
        {
            var vl = new VertexLine();
            vertexLines.Add(vl);

            vl.header = transform;
            vl.direction = transform.up;
            vl.distance = deltaDistance * i;

            vl.InitializePoints(vertices, i, xCount);
        }
    }


    public void UpdateNormals()
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
        CreateMeshIfNull();
        InitializeMeshElements();

        Anomaly.Utils.SmartCoroutine.Create(CoUpdate());
    }

    private IEnumerator CoUpdate()
    {
        WaitForEndOfFrame wf = new WaitForEndOfFrame();
        (Vector3 pos, Quaternion quat) previous = (transform.position, transform.rotation);

        while (true)
        {
            yield return wf;

            if (!transform.hasChanged) continue;

            bool needInterpolate = (transform.position - previous.pos).sqrMagnitude >= interpolateThreshold * interpolateThreshold;
            if (needInterpolate)
            {
                (Vector3 pos, Quaternion quat) temp = (transform.position, transform.rotation);

                transform.position = Vector3.Slerp(previous.pos, transform.position, 0.5f);
                transform.rotation = Quaternion.Slerp(previous.quat, transform.rotation, 0.5f);

                UpdateVertex();

                transform.position = temp.pos;
                transform.rotation = temp.quat;
            }

            UpdateVertex();

            previous.pos = transform.position;
            previous.quat = transform.rotation;
        }


        void UpdateVertex()
        {
            anchorLine = Mathf.Clamp(anchorLine, 0, yCount);

            float deltaDistance = (float)width / (yCount - 1);

            for (int i = 0; i < vertexLines.Count; ++i)
            {
                vertexLines[i].direction = transform.up;
                vertexLines[i].distance = deltaDistance * (i - anchorLine);
                vertexLines[i].Update();
            }

            mesh.vertices = vertexLines[0].vertices;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < vertexLines.Count; ++i)
        {
            vertexLines[i].Clear();
        }
    }


#if UNITY_EDITOR
    [System.NonSerialized] public bool showGizmos;

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        if (mesh == null) return;

        for (int i = 0; i < mesh.vertices.Length; ++i)
        {
            Color prev = Gizmos.color;
            float c = (float)(i % xCount) / (xCount - 1);
            if (i % xCount == 0) Gizmos.color = Color.red;
            else Gizmos.color = new Color(c, c, c, 1F);
            Gizmos.DrawSphere(mesh.vertices[i], i % xCount == 0 ? 0.2f : 0.1f);
            Gizmos.color = prev;
        }
    }
#endif
}
