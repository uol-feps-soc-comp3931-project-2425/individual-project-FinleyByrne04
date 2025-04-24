using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;

public class SceneChanger : MonoBehaviour
{
    public string[] sceneNames;
    private int currentSceneIndex = 0;
    private int taskIndex = 0;
    private int[] interiorScenes = { 1, 3, 5, 7, 8, 9, 10, 11, 13, 15, 17, 19 };
    private float[,] results = new float[20,5];

    public Transform taskObject;
    public Transform xrOrigin;
    public Transform referenceObject;
    public Vector3[] outsidePositions;
    public Vector3[] insidePositions;
    public Vector3[] rotations;

    private float time = 0.0f;
    private float taskInterval = 60.0f;


    void Start()
    {
        // Load start scene and place objects
        moveObjects();
        SceneManager.LoadScene(sceneNames[currentSceneIndex], LoadSceneMode.Additive);
    }

    void Update()
    {
        // Track time for scene changes
        time += Time.deltaTime;
        if (time >= taskInterval)
        {
            taskInterval = 15.0f;
            time = 0.0f;

            // Collect results and iterate task
            MeasureDistance();
            taskIndex++;

            // Move onto next scene after 4 tasks
            if (taskIndex == 4)
            {
                taskIndex = 0;

                // If there are still scenes to test, carry on, if not exit
                currentSceneIndex++;
                if (currentSceneIndex < sceneNames.Length)
                {
                    // Load new scene additively, unload previous scene, collect garbage and clean memory
                    SceneManager.LoadScene(sceneNames[currentSceneIndex], LoadSceneMode.Additive);
                    SceneManager.UnloadSceneAsync(sceneNames[currentSceneIndex - 1]);
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                    moveObjects();
                }
                else
                {
                    // Unloads last scene, exits application or editor window
                    Debug.Log("All scenes completed.");
                    SceneManager.UnloadSceneAsync(sceneNames[currentSceneIndex - 1]);
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();

#if UNITY_EDITOR
                    sceneResults();
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    sceneResults();
                    Application.Quit();
#endif
                }
            }
            else
            {
                moveObjects();
            }
        }
    }


    void moveObjects()
    {
        // Checks for needed positions, interior or exterior
        Vector3[] positions;
        if (interiorScenes.Contains(currentSceneIndex))
        {
            positions = insidePositions;
        }
        else
        {
            positions = outsidePositions;
        }

        // Adds height for heightened avatar testing
        if (currentSceneIndex == 4 || currentSceneIndex == 5)
        {
            xrOrigin.position = new Vector3(positions[taskIndex].x, positions[taskIndex].y + 0.75f, positions[taskIndex].z);
        }
        // Decreases height for shortened avatar testing
        else if (currentSceneIndex == 6 || currentSceneIndex == 7) 
        {
            xrOrigin.position = new Vector3(positions[taskIndex].x, positions[taskIndex].y - 0.75f, positions[taskIndex].z);
        }
        // Otherwise set position of the participant
        else
        {
            xrOrigin.position = positions[taskIndex];
        }
        
        xrOrigin.rotation = Quaternion.Euler(rotations[taskIndex % 4]);

        // Set positions of the cube and red line based on participant position
        Vector3 taskPosition = xrOrigin.position + xrOrigin.forward * 0.4f;
        taskPosition.y = 2.0f;
        taskObject.position = taskPosition;
        taskObject.rotation = xrOrigin.rotation;

        Vector3 referencePosition = xrOrigin.position + xrOrigin.forward * Random.Range(1.5f, 2.5f);
        referencePosition.y = 2.0f;
        referenceObject.position = referencePosition;
        referenceObject.rotation = xrOrigin.rotation;
    }


    void MeasureDistance()
    {
        // Store position halfway between participant and red line
        Vector3 halfwayPoint = (xrOrigin.position + referenceObject.position) / 2;

        Vector3 taskPosition = taskObject.position;

        string intOrExt;
        if (interiorScenes.Contains(currentSceneIndex))
        {
            intOrExt = "Interior";
        }
        else
        {
            intOrExt = "Exterior";
        }

        // Calculate users depth perception error, negative values mean too far, positive values mean too close
        float error = 0f;
        int modTask = taskIndex % 4;
        switch (modTask)
        {
            case 0:
                error = halfwayPoint.z - taskPosition.z;
                break;
            case 1:
                error = halfwayPoint.x - taskPosition.x;
                break;
            case 2:
                error = taskPosition.z - halfwayPoint.z;
                break;
            case 3:
                error = taskPosition.x - halfwayPoint.x;
                break;
        }
        // Round to 3 s.f. and output to console
        error = float.Parse(error.ToString("G3"));
        results[currentSceneIndex, taskIndex] = error;
        Debug.Log("Task " + (taskIndex + 1) + ", Scene: " + sceneNames[currentSceneIndex] + " " + intOrExt + ", Accuracy error = " + error);
    }


    void sceneResults()
    {
        string filePath = Path.Combine(Application.dataPath, "SceneManagement/DepthTaskResults.csv");

        // Format results and store in a csv file
        for (int i = 0; i < results.GetLength(0); i++)
        {
            string sceneName = sceneNames[i];
            string environmentType = "";
            if (interiorScenes.Contains(i)) 
            {
                environmentType = "Interior";
            }
            else
            {
                environmentType = "Exterior";
            }

            for (int j = 0; j < 4; j++)
            {
                float error = results[i, j];
                string line = $"{sceneName},{environmentType},{j + 1},{error:F3}";
                File.AppendAllText(filePath, line + "\n");
            }
        }
        Debug.Log("Results written to CSV file");
    }
}

