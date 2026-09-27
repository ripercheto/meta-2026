using UnityEngine;

[ExecuteAlways]
public class HollowBoxCollider : MonoBehaviour
{
    [Header("Outer Dimensions")]
    [Tooltip("Overall inner boundary size (width X, height Y, depth Z) of the box.")]
    public Vector3 outerSize = new Vector3(5f, 5f, 5f);

    [Header("Wall Thickness")]
    [Tooltip("Thickness extending outwards for each of the 6 walls.")]
    [Min(0.01f)]
    public float wallThickness = 0.2f;

    // References to the 6 individual colliders
    [HideInInspector] public BoxCollider top;
    [HideInInspector] public BoxCollider bottom;
    [HideInInspector] public BoxCollider left;
    [HideInInspector] public BoxCollider right;
    [HideInInspector] public BoxCollider front;
    [HideInInspector] public BoxCollider back;

    private void OnValidate()
    {
        UpdateColliders();
    }

    private void Start()
    {
        UpdateColliders();
    }

    /// <summary>
    /// Updates all 6 box colliders to sit on the outside of outerSize and overlap along all edges and corners.
    /// </summary>
    public void UpdateColliders()
    {
        // Ensure all 6 box colliders exist on dedicated child objects
        top = GetOrAddCollider("Wall_Top");
        bottom = GetOrAddCollider("Wall_Bottom");
        left = GetOrAddCollider("Wall_Left");
        right = GetOrAddCollider("Wall_Right");
        front = GetOrAddCollider("Wall_Front");
        back = GetOrAddCollider("Wall_Back");

        // Half-extents for positioning centers
        float halfWall = wallThickness / 2f;
        float halfX = outerSize.x / 2f;
        float halfY = outerSize.y / 2f;
        float halfZ = outerSize.z / 2f;

        // Expanded lengths so each wall overlaps adjacent walls by 1 wallThickness on all sides
        float fullX = outerSize.x + (2f * wallThickness);
        float fullY = outerSize.y + (2f * wallThickness);
        float fullZ = outerSize.z + (2f * wallThickness);

        // 1. Top & Bottom Colliders
        // Sits above/below outerSize; extends full X and Z so corners overlap side walls
        top.size = new Vector3(fullX, wallThickness, fullZ);
        top.center = new Vector3(0f, halfY + halfWall, 0f);

        bottom.size = new Vector3(fullX, wallThickness, fullZ);
        bottom.center = new Vector3(0f, -halfY - halfWall, 0f);

        // 2. Left & Right Colliders
        // Sits left/right of outerSize; extends full Y and Z so corners overlap top/bottom/front/back
        left.size = new Vector3(wallThickness, fullY, fullZ);
        left.center = new Vector3(-halfX - halfWall, 0f, 0f);

        right.size = new Vector3(wallThickness, fullY, fullZ);
        right.center = new Vector3(halfX + halfWall, 0f, 0f);

        // 3. Front & Back Colliders
        // Sits front/back of outerSize; extends full X and Y so corners overlap side/top/bottom
        front.size = new Vector3(fullX, fullY, wallThickness);
        front.center = new Vector3(0f, 0f, halfZ + halfWall);

        back.size = new Vector3(fullX, fullY, wallThickness);
        back.center = new Vector3(0f, 0f, -halfZ - halfWall);
    }

    private BoxCollider GetOrAddCollider(string childName)
    {
        Transform child = transform.Find(childName);
        if (child == null)
        {
            child = new GameObject(childName).transform;
            child.SetParent(transform, false);
        }

        BoxCollider col = child.GetComponent<BoxCollider>();
        if (col == null)
        {
            col = child.gameObject.AddComponent<BoxCollider>();
        }

        return col;
    }
}