using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteAlways]
public class ProceduralBeveledTile : MonoBehaviour
{
    [Header("Tile Dimensions")]
    public Vector3 size = new Vector3(1f, 0.2f, 1f);

    [Header("Bevel Settings")]
    [Range(0.001f, 0.5f)]
    public float cornerRadius = 0.05f;

    [Range(1, 16)]
    [Tooltip("Subdivisions for curved corners. 4 to 8 works great.")]
    public int cornerSegments = 6;

    private void OnValidate()
    {
        GenerateTile();
    }

    private void Start()
    {
        GenerateTile();
    }

    [ContextMenu("Generate Tile Mesh")]
    public void GenerateTile()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "SoftBeveledTile";

        // Clamp radius to half of smallest dimension
        float maxR = Mathf.Min(size.x, Mathf.Min(size.y, size.z)) * 0.499f;
        float r = Mathf.Min(cornerRadius, maxR);

        // Half inner dimensions
        float hx = (size.x * 0.5f) - r;
        float hy = (size.y * 0.5f) - r;
        float hz = (size.z * 0.5f) - r;

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // 1. FLAT FACES (6 Quads)
        AddQuadFace(verts, normals, uvs, tris, new Vector3(0, hy + r, 0), Vector3.up, Vector3.right * hx, Vector3.forward * hz);
        AddQuadFace(verts, normals, uvs, tris, new Vector3(0, -hy - r, 0), Vector3.down, Vector3.right * hx, Vector3.back * hz);
        AddQuadFace(verts, normals, uvs, tris, new Vector3(hx + r, 0, 0), Vector3.right, Vector3.forward * hz, Vector3.up * hy);
        AddQuadFace(verts, normals, uvs, tris, new Vector3(-hx - r, 0, 0), Vector3.left, Vector3.back * hz, Vector3.up * hy);
        AddQuadFace(verts, normals, uvs, tris, new Vector3(0, 0, hz + r), Vector3.forward, Vector3.left * hx, Vector3.up * hy);
        AddQuadFace(verts, normals, uvs, tris, new Vector3(0, 0, -hz - r), Vector3.back, Vector3.right * hx, Vector3.up * hy);

        // 2. EDGES (12 Cylindrical Seams)
        // Vertical Y Edges
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(hx, 0, hz), Vector3.up, hy, Vector3.right, Vector3.forward, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(-hx, 0, hz), Vector3.up, hy, Vector3.forward, Vector3.left, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(-hx, 0, -hz), Vector3.up, hy, Vector3.left, Vector3.back, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(hx, 0, -hz), Vector3.up, hy, Vector3.back, Vector3.right, r, cornerSegments);

        // Horizontal X Edges
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(0, hy, hz), Vector3.right, hx, Vector3.forward, Vector3.up, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(0, -hy, hz), Vector3.right, hx, Vector3.down, Vector3.forward, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(0, hy, -hz), Vector3.right, hx, Vector3.up, Vector3.back, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(0, -hy, -hz), Vector3.right, hx, Vector3.back, Vector3.down, r, cornerSegments);

        // Horizontal Z Edges
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(hx, hy, 0), Vector3.forward, hz, Vector3.up, Vector3.right, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(-hx, hy, 0), Vector3.forward, hz, Vector3.left, Vector3.up, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(hx, -hy, 0), Vector3.forward, hz, Vector3.right, Vector3.down, r, cornerSegments);
        AddEdgeCylinder(verts, normals, uvs, tris, new Vector3(-hx, -hy, 0), Vector3.forward, hz, Vector3.down, Vector3.left, r, cornerSegments);

        // 3. CORNERS (8 Spherical Caps)
        AddCornerCap(verts, normals, uvs, tris, new Vector3(hx, hy, hz), Vector3.right, Vector3.up, Vector3.forward, r, cornerSegments);
        AddCornerCap(verts, normals, uvs, tris, new Vector3(-hx, hy, hz), Vector3.up, Vector3.left, Vector3.forward, r, cornerSegments);
        AddCornerCap(verts, normals, uvs, tris, new Vector3(-hx, -hy, hz), Vector3.left, Vector3.down, Vector3.forward, r, cornerSegments);
        AddCornerCap(verts, normals, uvs, tris, new Vector3(hx, -hy, hz), Vector3.down, Vector3.right, Vector3.forward, r, cornerSegments);

        AddCornerCap(verts, normals, uvs, tris, new Vector3(hx, hy, -hz), Vector3.up, Vector3.right, Vector3.back, r, cornerSegments);
        AddCornerCap(verts, normals, uvs, tris, new Vector3(-hx, hy, -hz), Vector3.left, Vector3.up, Vector3.back, r, cornerSegments);
        AddCornerCap(verts, normals, uvs, tris, new Vector3(-hx, -hy, -hz), Vector3.down, Vector3.left, Vector3.back, r, cornerSegments);
        AddCornerCap(verts, normals, uvs, tris, new Vector3(hx, -hy, -hz), Vector3.right, Vector3.down, Vector3.back, r, cornerSegments);

        mesh.vertices = verts.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();

        // RECALCULATE BOUNDS AND TANGENTS FOR TOON SHADER SUPPORT
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.sharedMesh = mesh;
    }

    private void AddQuadFace(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<int> t,
        Vector3 center, Vector3 normal, Vector3 rightExtent, Vector3 upExtent)
    {
        int baseIdx = v.Count;
        v.Add(center - rightExtent - upExtent);
        v.Add(center + rightExtent - upExtent);
        v.Add(center + rightExtent + upExtent);
        v.Add(center - rightExtent + upExtent);

        for (int i = 0; i < 4; i++)
        {
            n.Add(normal);
        }

        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0, 1));

        t.Add(baseIdx);
        t.Add(baseIdx + 2);
        t.Add(baseIdx + 1);
        t.Add(baseIdx);
        t.Add(baseIdx + 3);
        t.Add(baseIdx + 2);
    }

    private void AddEdgeCylinder(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<int> t,
        Vector3 center, Vector3 axis, float halfLength, Vector3 startDir, Vector3 endDir, float r, int segs)
    {
        int baseIdx = v.Count;

        for (int i = 0; i <= segs; i++)
        {
            float angle = (i / (float)segs) * Mathf.PI * 0.5f;
            Vector3 dir = (startDir * Mathf.Cos(angle) + endDir * Mathf.Sin(angle)).normalized;

            Vector3 p1 = center - axis * halfLength + dir * r;
            Vector3 p2 = center + axis * halfLength + dir * r;

            v.Add(p1);
            v.Add(p2);

            n.Add(dir);
            n.Add(dir);

            uv.Add(new Vector2(i / (float)segs, 0f));
            uv.Add(new Vector2(i / (float)segs, 1f));

            if (i < segs)
            {
                int curr = baseIdx + i * 2;
                t.Add(curr);
                t.Add(curr + 1);
                t.Add(curr + 3);

                t.Add(curr);
                t.Add(curr + 3);
                t.Add(curr + 2);
            }
        }
    }

    private void AddCornerCap(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<int> t,
        Vector3 center, Vector3 dirX, Vector3 dirY, Vector3 dirZ, float r, int segs)
    {
        int baseIdx = v.Count;

        for (int y = 0; y <= segs; y++)
        {
            float pitch = (y / (float)segs) * Mathf.PI * 0.5f;
            for (int x = 0; x <= segs; x++)
            {
                float yaw = (x / (float)segs) * Mathf.PI * 0.5f;

                Vector3 localNormal = (dirX * Mathf.Cos(pitch) * Mathf.Cos(yaw) +
                                       dirY * Mathf.Sin(pitch) +
                                       dirZ * Mathf.Cos(pitch) * Mathf.Sin(yaw)).normalized;

                v.Add(center + localNormal * r);
                n.Add(localNormal);
                uv.Add(new Vector2(x / (float)segs, y / (float)segs));

                if (x < segs && y < segs)
                {
                    int row1 = baseIdx + y * (segs + 1) + x;
                    int row2 = baseIdx + (y + 1) * (segs + 1) + x;

                    t.Add(row1);
                    t.Add(row2);
                    t.Add(row1 + 1);

                    t.Add(row1 + 1);
                    t.Add(row2);
                    t.Add(row2 + 1);
                }
            }
        }
    }
}