using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class RestartScene : MonoBehaviour
{
    public GameObject fsrCanvas;
    public GameObject parent;
    public GameObject gloveOnText;
    public GameObject gloveOffText;

    public List<GameObject> objects = new();
    private readonly List<Vector3> objectPositions = new();
    public List<GameObject> foodPrefabs = new();
    public List<Vector3> foodPrefabPos = new();
    private Quaternion objectRotations = new(0.0f, 0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        //Store all cooking tools position
        for (int i = 0; i < objects.Count; i++)
        {
            objectPositions.Add(new Vector3(objects[i].transform.position.x, objects[i].transform.position.y, objects[i].transform.position.z));
        }
    }

    public void Restart()
    {   
        //Set cooking tools position to their original position
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].transform.SetPositionAndRotation(objectPositions[i], objectRotations);
        }
        // Create new food objects
        for (int i = 0; i < foodPrefabs.Count; i++)
        {
            Instantiate(foodPrefabs[i], foodPrefabPos[i], objectRotations);
        }

        // Get all the gameobjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Loop through each gameobject
        foreach (GameObject obj in allObjects)
        {
            // Check if the gameobject's name matches the name to destroy
            if (obj.name == "Upper_Hull" || obj.name == "Lower_Hull")
            {
                // Destroy the gameobject
                Destroy(obj);
            }
        }
    }

    public void ToggleMode(bool mode)
    {
        // Get all the gameobjects in the scene
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("GB");

        // Loop through each gameobject
        foreach (GameObject obj in allObjects)
        {
            obj.GetComponent<Grabbable>().includeInExperiment = mode;
        }
        gloveOnText.SetActive(mode);
        gloveOffText.SetActive(!mode);
    }

    public void ChangeGloveMode()
    {
        if (gloveOnText.activeInHierarchy == true)
        {
            ToggleMode(false);
        }
        else
        {
            ToggleMode(true);
        }
    }
}
