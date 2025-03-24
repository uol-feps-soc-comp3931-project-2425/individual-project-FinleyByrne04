using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string[] sceneNames;
    private int currentSceneIndex = 0;
    private int taskIndex = 0;
    private int tasksPerScene = 4;

    public Transform taskObject;
    public Transform xrOrigin;
    public Transform referenceObject;
    public Vector3[] positions;
    public Vector3[] rotations;

    private float time = 0.0f;
    private float taskInterval = 15.0f;

    void Start()
    {
        moveObjects();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= taskInterval) // Press Enter to switch
        {
            time = 0.0f;
            MeasureDistance();
            taskIndex++;

            if (taskIndex == tasksPerScene)
            {
                currentSceneIndex++;
                if (currentSceneIndex < sceneNames.Length)
                {
                    SceneManager.LoadScene(sceneNames[currentSceneIndex]);
                }
            }
            else
            {
                moveObjects();
                Debug.Log("Task " + taskIndex + " completed");
            }
        }
    }

    void moveObjects()
    {
        if (taskIndex < positions.Length)
        {
            xrOrigin.position = positions[taskIndex];
            xrOrigin.rotation = Quaternion.Euler(rotations[taskIndex]);

            Vector3 taskPosition = xrOrigin.position + xrOrigin.forward * 0.782f;
            taskObject.position = taskPosition;

            Vector3 referencePosition = xrOrigin.position + xrOrigin.forward * 2.0f;
            referenceObject.position = referencePosition;
            referenceObject.rotation = Quaternion.Euler(rotations[taskIndex]);
        }
    }

    void MeasureDistance()
    {
        Vector3 halfwayPoint = (xrOrigin.position + referenceObject.position) / 2;

        Vector3 taskPosition = taskObject.position;

        float error = 0f;
        if (taskIndex % 4 == 1 || taskIndex % 4 == 3)
        {
            error = Mathf.Abs(taskPosition.x - halfwayPoint.x);
            Debug.Log("Task " + (taskIndex + 1) + ", Scene: " + sceneNames[currentSceneIndex+1] + ", Accuracy error = " + error);
        }
        else 
        {
            error = Mathf.Abs(taskPosition.z - halfwayPoint.z);
            Debug.Log("Task " + (taskIndex + 1) + ", Scene: " + sceneNames[currentSceneIndex] + ", Accuracy error = " + error);
        }
    }
}
