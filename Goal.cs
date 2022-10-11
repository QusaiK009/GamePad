using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // if collides with food then destroy food object
        if (collision.gameObject.layer == 3)
        {
            Destroy(collision.gameObject);
            Spawner.instance.spawnedObjects.Remove(collision.gameObject);
            Spawner.instance.SpawnGameObject();
            GameManager.instance.previousState = 2; // tell system goal state is done
            GameManager.instance.UpdateGameState(GameState.Idle); // move to next state
        }
    }
}
