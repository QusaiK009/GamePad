using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float xTargetScaled;
    private float yTargetScaled;
    private float zTargetScaled;

    /* 1: dropped item on shelf
     * 2: dropped item on floor
     * 3: no objects in spawn array
     * 4: wrong item picked up
     */
    public int errorNumber;

    /* 0: Start
     * 1: Food
     * 2: Goal
     */
    public int previousState;

    public float idleCountdownTimer;
    public bool isCounting = false;

    [SerializeField] Transform goal;
    [SerializeField] Transform food;
    [SerializeField] Text gameInstruction;

    public GameState state;

    public static event Action<GameState> onGameStateChanged;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        previousState = 0;
        gameInstruction.text = "Welcome! Get Ready!";
        gameInstruction.fontSize = 60;
        gameInstruction.color = Color.green;
        UpdateGameState(GameState.Idle);
        // start from original position, wait 5 seconds
        xTargetScaled = 3.0f * 18.75f;
        yTargetScaled = (-12.2f + 6.6f) * 18.75f;
        zTargetScaled = (12.0f - 5.91f) * 12.0f;
        UDP_Handling.Xtarget = xTargetScaled;
        UDP_Handling.Ytarget = yTargetScaled;
        UDP_Handling.Ztarget = zTargetScaled;
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Food:
                PickUp();
                break;
            case GameState.Goal:
                Drop();
                break;
            case GameState.Idle:
                IdleState();
                break;
            case GameState.Error:
                ErrorMessage();
                break;
            case GameState.Complete:
                GameOver();
                break;
            default:
                break;
        }

        onGameStateChanged?.Invoke(newState);
    }

    private void ErrorMessage()
    {
        if (errorNumber == 1)
        {
            gameInstruction.text = "Dropped item on shelf!";
            gameInstruction.color = Color.red;
            errorNumber = 0;
        }
        else if (errorNumber == 2)
        {
            gameInstruction.text = "Dropped item on floor!";
            gameInstruction.color = Color.red;
            errorNumber = 0;
        }
        else if (errorNumber == 3)
        {
            gameInstruction.text = "No objects in spawn array!";
            gameInstruction.color = Color.red;
            errorNumber = 0;
        }
        else if (errorNumber == 4)
        {
            gameInstruction.text = "Wrong item picked up!";
            gameInstruction.color = Color.red;
            UpdateGameState(GameState.Idle);
        } 
        else
        {
            Debug.Log("Invalid error number");
        }
    }

    private void IdleState()
    {
        isCounting = true;
        Debug.Log("In Idle");
        if (previousState < 2) // if previous state was food or start
        {
            // 5 second cooldown time
            idleCountdownTimer = 5.0f;
            if (previousState == 1)
            {
                if (errorNumber == 4)
                {
                    // go back to original position
                    xTargetScaled = 3.0f * 18.75f;
                    yTargetScaled = (-12.2f + 6.6f) * 18.75f;
                    zTargetScaled = (12.0f - 5.91f) * 12.0f;
                    UDP_Handling.Xtarget = xTargetScaled;
                    UDP_Handling.Ytarget = yTargetScaled;
                    UDP_Handling.Ztarget = zTargetScaled;
                    errorNumber = 0;
                }
                else
                {
                    // freeze MyPAM
                    UDP_Handling.Xtarget = UDP_Handling.X2pos;
                    UDP_Handling.Ytarget = UDP_Handling.Y2pos;
                    UDP_Handling.Ztarget = UDP_Handling.Z2pos;
                }
            }
        } else if (previousState == 2)
        {
            // 5 second cooldown time
            idleCountdownTimer = 6.0f;
            // go back to original position after dropping food
            xTargetScaled = 3.0f * 18.75f;
            yTargetScaled = (-12.2f + 6.6f) * 18.75f;
            zTargetScaled = (12.0f - 5.91f) * 12.0f;
            UDP_Handling.Xtarget = xTargetScaled;
            UDP_Handling.Ytarget = yTargetScaled;
            UDP_Handling.Ztarget = zTargetScaled;
        }
    }

    private void GameOver()
    {
        gameInstruction.text = "CONGRATS!";
        gameInstruction.fontSize = 120;
        gameInstruction.color = Color.cyan;
        // return hand to original position
        xTargetScaled = 3.0f * 18.75f;
        yTargetScaled = (-12.2f + 6.6f) * 18.75f;
        zTargetScaled = (12.0f - 5.91f) * 12.0f;
        UDP_Handling.Xtarget = xTargetScaled;
        UDP_Handling.Ytarget = yTargetScaled;
        UDP_Handling.Ztarget = zTargetScaled;
    }

    private void Drop()
    {
         //Debug.Log("Going to goal");
         gameInstruction.text = "Please Drop on Yellow Plate.";
         gameInstruction.color = Color.yellow;
         xTargetScaled = goal.transform.position.x * 18.75f;
         yTargetScaled = (goal.transform.position.z + 5.1f) * 18.75f; // increase by 1.5 to allow item to be on top of goal
         zTargetScaled = (goal.transform.position.y - 4.41f) * 12.0f; // raise above goal by 1.5
         UDP_Handling.Xtarget = xTargetScaled;
         UDP_Handling.Ytarget = yTargetScaled;
         UDP_Handling.Ztarget = zTargetScaled;
    }

    private void PickUp()
    {
        Debug.Log("Going to food");

        // go to first member of spawned items list
        DisplayText();
        xTargetScaled = Spawner.instance.spawnedObjects[0].transform.position.x * 18.75f;
        yTargetScaled = (Spawner.instance.spawnedObjects[0].transform.position.z + 6.6f) * 18.75f;
        zTargetScaled = (Spawner.instance.spawnedObjects[0].transform.position.y - 5.91f) * 12.0f;
       // Debug.Log(xTargetScaled);
        //Debug.Log(yTargetScaled);
        //Debug.Log(zTargetScaled);
        UDP_Handling.Xtarget = xTargetScaled;
        UDP_Handling.Ytarget = yTargetScaled;
        UDP_Handling.Ztarget = zTargetScaled;

        // only ends when timer is done
        // UpdateGameState(GameState.Complete);
    }

    private void DisplayText()
    {
        if (Spawner.instance.spawnedObjects[0].name == "Avocado(Clone)")
        {
            gameInstruction.text = "Please Pick Up Avocado";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.green;
        } 
        else if (Spawner.instance.spawnedObjects[0].name == "Banana(Clone)")
        {
            gameInstruction.text = "Please Pick Up Banana";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.yellow;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Burger(Clone)")
        {
            gameInstruction.text = "Please Pick Up Burger";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.yellow;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Cheese_02(Clone)")
        {
            gameInstruction.text = "Please Pick Up Cheese";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.yellow;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Chips_Bag(Clone)")
        {
            gameInstruction.text = "Please Pick Up Chips Bag";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.red;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Croissant(Clone)")
        {
            gameInstruction.text = "Please Pick Up Croissant";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.yellow;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Donut(Clone)")
        {
            gameInstruction.text = "Please Pick Up Donut";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.yellow;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Drink_01(Clone)")
        {
            gameInstruction.text = "Please Pick Up Drink";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.green;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Fries(Clone)")
        {
            gameInstruction.text = "Please Pick Up Fries";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.red;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Muffin(Clone)")
        {
            gameInstruction.text = "Please Pick Up Muffin";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.magenta;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Onion(Clone)")
        {
            gameInstruction.text = "Please Pick Up Onion";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.magenta;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Pineapple(Clone)")
        {
            gameInstruction.text = "Please Pick Up Pineapple";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.yellow;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Rice_Bowl(Clone)")
        {
            gameInstruction.text = "Please Pick Up Rice Bowl";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.cyan;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Tomato(Clone)")
        {
            gameInstruction.text = "Please Pick Up Tomato";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.red;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "SweetPepper(Clone)")
        {
            gameInstruction.text = "Please Pick Up Sweet Pepper";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.red;
        }
        else if (Spawner.instance.spawnedObjects[0].name == "Watermelon(Clone)")
        {
            gameInstruction.text = "Please Pick Up Watermelon";
            gameInstruction.fontSize = 30;
            gameInstruction.color = Color.green;
        }


    }
}
public enum GameState
{
    Food, // state for instruction to pick up food
    Goal, // state for instruction to drop food in goal
    Idle, // freezes robot during transitions and errors
    Error, // displays error messages
    Complete // state to display complete message when all items placed
}