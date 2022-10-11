using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public static Items instance;

    public Transform theDest;
    private int timeOut;
    bool hasBeenHeld = false;
    bool dropKeyPressed = false;
    Vector3 coordinates;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        coordinates = this.transform.position; // store initial coordinates to respawn
        theDest = GameObject.Find("Destination").transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Hand" && theDest.childCount == 0 && !hasBeenHeld) // if it collides with hand and hand is not carrying anything
        {
            GameManager.instance.previousState = 1;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false; // turn off gravity on item
            GetComponent<Rigidbody>().isKinematic = true;
            hasBeenHeld = true;

            this.transform.SetParent(theDest); // making it a child of empty game object
            this.transform.localPosition = Vector3.zero; // moves block to hand gameobject
            this.transform.localRotation = Quaternion.Euler(Vector3.zero);

            // if item does not match item that is supposed to be picked up
            if ((this.name == Spawner.instance.spawnedObjects[0].name) && (hasBeenHeld))
            {
                
                GameManager.instance.UpdateGameState(GameState.Goal); // move to next state
            } else if ((this.name != Spawner.instance.spawnedObjects[0].name) && (hasBeenHeld))
            {
                GameManager.instance.errorNumber = 4;
            }
        }

        // if collided with bookshelf and is not being held then respawn back to original location
        if (collision.gameObject.layer == 6 && !hasBeenHeld)
        {
            this.transform.position = coordinates;
            Debug.Log("Collided with Shelf!");
            GameManager.instance.errorNumber = 1;
            GameManager.instance.UpdateGameState(GameState.Error); // Display error
        }

        // if collided with floor and is not being held then respawn back to original location
        if (collision.gameObject.layer == 7 && !hasBeenHeld)
        {
            this.transform.position = coordinates;
            Debug.Log("Collided with Floor!");
            GameManager.instance.errorNumber = 2;
            GameManager.instance.UpdateGameState(GameState.Error); // Display Error
        }

        // if wrong item picked up
        if (GameManager.instance.errorNumber == 4 && hasBeenHeld)
        {
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
            hasBeenHeld = false;
            this.transform.position = coordinates;
            GameManager.instance.UpdateGameState(GameState.Error); // Display Error
            this.transform.SetParent(null); // make it a parent again
        }
    }

    void Update()
    {
        
        // Check if space key is pressed down
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    dropKeyPressed = true;
        //}
        if (OVRInput.GetUp(OVRInput.RawButton.X))
        {
            dropKeyPressed = true;
        }
        
    }

    private void FixedUpdate()
    {
        if (dropKeyPressed && this.transform.IsChildOf(theDest))
        {
            Debug.Log("Dropping...");
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;

            hasBeenHeld = false;
            dropKeyPressed = false;
            this.transform.SetParent(null); // make it a parent again
            if (this.transform.position.x >= -7.2 && this.transform.position.x <= -6.8)
            {
                GameManager.instance.previousState = 2; // tell system it is above goal
            }

            GameManager.instance.UpdateGameState(GameState.Idle);

        }
    }

}
