using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GenerateFlatTextures : EditorWindow
{
    // Establish scene to change, save folder and dictionary of materials
    private const string sceneName = "OutdoorsScene";
    private const string saveFolder = "Assets/TexturelessMaterials";
    private static Dictionary<string, Material> createdMaterials = new Dictionary<string, Material>();

    // Create accessible button in editor
    [MenuItem("Tools/Convert to Textureless Materials")]
    public static void ConvertMaterials()
    {
        // Get all renderers in the specified scene and iterate 
        Scene scene = SceneManager.GetSceneByName(sceneName);
        Renderer[] renderers = GetRenderersInScene(scene);

        foreach (Renderer renderer in renderers)
        {
            // Create a new material for each renderer
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

            // Iterate through each material in the renderer
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                // Ensure the material is copiable
                Material originalMat = renderer.sharedMaterials[i];
                if (originalMat == null || AssetDatabase.IsSubAsset(originalMat))
                    continue;

                string newMaterialPath = Path.Combine(saveFolder, originalMat.name + "_Flat.mat");

                // Check if the flat material was already created
                if (!createdMaterials.TryGetValue(originalMat.name, out Material newMat))
                {
                    // Assign HDRP lit shader and base colour to be the average colour of the original material
                    newMat = new Material(Shader.Find("HDRP/Lit"));
                    newMat.color = GetAverageColor(originalMat);

                    // Create the new material asset and store for repetition checks and reuse
                    AssetDatabase.CreateAsset(newMat, newMaterialPath);
                    createdMaterials[originalMat.name] = newMat;
                }

                // Array of the flat materials
                newMaterials[i] = newMat;
            }

            // Assign new materials to object
            renderer.sharedMaterials = newMaterials;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Color GetAverageColor(Material mat)
    {
        // If no texture, just make the material grey
        if (!mat.HasProperty("_MainTex"))
            return Color.gray;

        // Store texture and return base color if null
        Texture2D tex = mat.mainTexture as Texture2D;
        if (tex == null)
            return mat.color;

        // Get pixel colours and find and return the average
        Color[] pixels = tex.GetPixels();
        Color avgColor = Color.black;

        foreach (Color c in pixels)
            avgColor += c;

        avgColor /= pixels.Length;
        return avgColor;
    }

    private static Renderer[] GetRenderersInScene(Scene scene)
    {
        List<Renderer> renderers = new List<Renderer>();

        // Loop through all objects in the scene
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            // Find all renderers in the object and any children
            Renderer[] foundRenderers = rootObject.GetComponentsInChildren<Renderer>();
            renderers.AddRange(foundRenderers);
        }

        return renderers.ToArray();
    }
}
