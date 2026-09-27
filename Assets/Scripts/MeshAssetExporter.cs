#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class MeshAssetExporter
{
    [MenuItem("CONTEXT/MeshFilter/Save Mesh As Asset")]
    private static void SaveMeshFromContextMenu(MenuCommand menuCommand)
    {
        MeshFilter meshFilter = (MeshFilter)menuCommand.context;
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning("No valid Mesh found on this MeshFilter!");
            return;
        }

        SaveMeshAsset(meshFilter.sharedMesh, meshFilter.gameObject.name);
    }

    [MenuItem("GameObject/Save Selected Mesh As Asset", false, 30)]
    private static void SaveSelectedMesh()
    {
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            Debug.LogWarning("No GameObject selected!");
            return;
        }

        MeshFilter meshFilter = selectedObj.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning("Selected GameObject does not have a MeshFilter with a valid mesh!");
            return;
        }

        SaveMeshAsset(meshFilter.sharedMesh, selectedObj.name);
    }

    public static void SaveMeshAsset(Mesh originalMesh, string defaultName)
    {
        // 1. Choose directory path
        string folderPath = "Assets/SavedMeshes";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string defaultPath = $"{folderPath}/{defaultName}_Mesh.asset";
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Mesh Asset",
            $"{defaultName}_Mesh",
            "asset",
            "Please select a destination to save the Mesh asset.",
            folderPath
        );

        if (string.IsNullOrEmpty(path)) return; // User canceled save dialog

        // 2. Instantiate a copy of the mesh so it isn't tied to runtime procedural data
        Mesh meshToSave = Object.Instantiate(originalMesh);

        // 3. Save as asset database item
        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 4. Highlight the newly created asset in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = meshToSave;

        Debug.Log($"<color=green>Mesh successfully saved as asset at:</color> {path}");
    }
}
#endif