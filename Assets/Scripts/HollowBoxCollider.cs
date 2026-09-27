using UnityEngine;

[ExecuteAlways]
public class HollowBoxCollider : MonoBehaviour
{
    [Header("Outer Dimensions")]
    [Tooltip("Overall size (width X, height Y, depth Z) of the hollow box.")]
    public Vector3 outerSize = new Vector3(5f, 5f, 5f);

    [Header("Wall Thickness")]
    [Tooltip("Thickness of each of the 6 walls.")]
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
        wallThickness = Mathf.Min(wallThickness, Mathf.Min(outerSize.x, Mathf.Min(outerSize.y, outerSize.z)) / 2f);
        UpdateColliders();
    }

    private void Start()
    {
        UpdateColliders();
    }

    /// <summary>
    /// Creates missing box colliders or updates existing ones with full outer dimensions so corners overlap.
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
        float halfX = outerSize.x / 2f;
        float halfY = outerSize.y / 2f;
        float halfZ = outerSize.z / 2f;
        float halfWall = wallThickness / 2f;

        // 1. Top & Bottom Colliders (Full X and Z span)
        top.size = new Vector3(outerSize.x, wallThickness, outerSize.z);
        top.center = new Vector3(0f, halfY - halfWall, 0f);

        bottom.size = new Vector3(outerSize.x, wallThickness, outerSize.z);
        bottom.center = new Vector3(0f, -halfY + halfWall, 0f);

        // 2. Left & Right Colliders (Full Y and Z span - overlaps top/bottom corners)
        left.size = new Vector3(wallThickness, outerSize.y, outerSize.z);
        left.center = new Vector3(-halfX + halfWall, 0f, 0f);

        right.size = new Vector3(wallThickness, outerSize.y, outerSize.z);
        right.center = new Vector3(halfX - halfWall, 0f, 0f);

        // 3. Front & Back Colliders (Full X and Y span - overlaps side and top/bottom corners)
        front.size = new Vector3(outerSize.x, outerSize.y, wallThickness);
        front.center = new Vector3(0f, 0f, halfZ - halfWall);

        back.size = new Vector3(outerSize.x, outerSize.y, wallThickness);
        back.center = new Vector3(0f, 0f, -halfZ + halfWall);
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