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
        if (time >= taskInterval)
        {
            time = 0.0f;
            MeasureDistance();
            taskIndex++;

            if (taskIndex % tasksPerScene == 0)
            {
                currentSceneIndex++;
                if (currentSceneIndex < sceneNames.Length)
                {
                    SceneManager.LoadScene(sceneNames[currentSceneIndex], LoadSceneMode.Additive);

                    if (currentSceneIndex > 0)
                    {
                        SceneManager.UnloadSceneAsync(sceneNames[currentSceneIndex - 1]);
                        moveObjects();
                    }
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
        if (taskIndex < positions.Length)
        {
            xrOrigin.position = positions[taskIndex];
            xrOrigin.rotation = Quaternion.Euler(rotations[taskIndex%4]);

            Vector3 taskPosition = xrOrigin.position + xrOrigin.forward * 0.782f;
            taskObject.position = taskPosition;
            taskObject.rotation = xrOrigin.rotation;

            Vector3 referencePosition = xrOrigin.position + xrOrigin.forward * Random.Range(1.5f, 2.5f);
            referenceObject.position = referencePosition;
            referenceObject.rotation = xrOrigin.rotation;
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
            Debug.Log("Task " + (taskIndex + 1) + ", Scene: " + sceneNames[currentSceneIndex] + ", Accuracy error = " + error);
        }
        else 
        {
            error = Mathf.Abs(taskPosition.z - halfwayPoint.z);
            Debug.Log("Task " + (taskIndex + 1) + ", Scene: " + sceneNames[currentSceneIndex] + ", Accuracy error = " + error);
        }
    }
}
