using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public partial class UpdateTreeColors : MonoBehaviour
{
    [UnityEditor.MenuItem("Terrain/Update Tree Lightmap Color")]
    public static void RebuildWithLightmap()
    {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex)
        {
            if ((Terrain.activeTerrain == null) || (Terrain.activeTerrain.terrainData == null))
            {
                EditorUtility.DisplayDialog("No active terrain in the scene", "No active terrain in the scene", "Ok");
                return;
            }
            //Undo.RegisterUndo(Terrain.activeTerrain.terrainData, "Set Tree colors");
            Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Set Tree colors");
        }
        else
        {
            //UnityEditor.TerrainLightmapper.UpdateTreeLightmapColor(tex, Terrain.activeTerrain.terrainData); /**/
            EditorUtility.DisplayDialog("Select a lightmap", "Select a lightmap", "Ok");
        }
    }

    [UnityEditor.MenuItem("Terrain/Update Tree Color")]
    public static void RebuildWithColor()
    {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex)
        {
            if ((Terrain.activeTerrain == null) || (Terrain.activeTerrain.terrainData == null))
            {
                EditorUtility.DisplayDialog("No active terrain in the scene", "No active terrain in the scene", "Ok");
                return;
            }
            //Undo.RegisterUndo(Terrain.activeTerrain.terrainData, "Set Tree colors");
            Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Set Tree colors");
        }
        else
        {
            //UnityEditor.TerrainLightmapper.UpdateTreeColor(tex, Terrain.activeTerrain.terrainData); /**/
            EditorUtility.DisplayDialog("Select a lightmap", "Select a lightmap", "Ok");
        }
    }

}