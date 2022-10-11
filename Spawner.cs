using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    public List<GameObject> objectsToSpawn = new List<GameObject>();
    public List<GameObject> spawnedObjects = new List<GameObject>();

    public float xRandom;
    public int xIndex;
    public float yRandom;
    public int yIndex;
    private float[] xValuesArray;
    private float[] yValuesArray;
    public Vector3 randomLocation;

    public bool ischanged = false;

    public bool isRandomized = true;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        yValuesArray = new float[] { 6.5f, 9.5f, 12.5f, 15.5f };
        xValuesArray = new float[] { -6.0f, -3.0f, 0f, 3.0f, 6.0f };
        // spawn 5 objects initially
        for (int i = 0; i < 5; i++)
        {
            SpawnGameObject();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnGameObject()
    {
        int index = isRandomized ? Random.Range(0, objectsToSpawn.Count) : 0;
        xIndex = isRandomized ? Random.Range(0, 5) : 0; // random value from 0 to 4 (5 is exclusive)
        yIndex = isRandomized ? Random.Range(0, 4) : 0; // random value from 0 to 3 (4 is exclusive)
        xRandom = xValuesArray[xIndex];
        yRandom = yValuesArray[yIndex];


        // make sure item not spawned in close proximity of another object
        // repeat process 15 times
        for (int i = 0; i <= 15; i++)
        {
            for (int i2 = 0; i2 < spawnedObjects.Count; i2++)
            {
                // if there is a match then rerandomize
                if (xRandom == spawnedObjects[i2].transform.position.x)
                {
                    if (yRandom <= spawnedObjects[i2].transform.position.y + 1.5f && yRandom >= spawnedObjects[i2].transform.position.y - 1.5f)
                    {
                        xIndex = isRandomized ? Random.Range(0, 5) : 0; // random value from 0 to 4 (5 is exclusive)
                        yIndex = isRandomized ? Random.Range(0, 4) : 0; // random value from 0 to 3 (4 is exclusive)
                        xRandom = xValuesArray[xIndex];
                        yRandom = yValuesArray[yIndex];
                        Debug.Log("Changed" + objectsToSpawn[index]);
                        ischanged = true;
                        break;
                    }
                }
            }
        }

        // set random location of empty game object spawner
        randomLocation = new Vector3(xRandom, yRandom, -1.5f);
        this.transform.position = randomLocation;

        if (objectsToSpawn.Count > 0)
        {
            // make sure no 2 objects spawn
            for (int i = 0; i < 5; i++)
            {
                for (int i2 = 0; i2 < spawnedObjects.Count; i2++)
                {
                    if (objectsToSpawn[index].name == spawnedObjects[i2].name)
                    {
                        if (index == objectsToSpawn.Count - 1)
                        {
                            index = 0;
                        } else
                        {
                            index++;
                        }
                        break;
                    }
                }
            }
            GameObject foodEmptyGameObject = Instantiate(objectsToSpawn[index], this.transform.position, this.transform.rotation);
            spawnedObjects.Add(foodEmptyGameObject);
            foodEmptyGameObject.transform.SetParent(GameObject.Find("Food").transform);
        } else
        {
            GameManager.instance.errorNumber = 3;
            GameManager.instance.UpdateGameState(GameState.Error);
        }
    }
}
