using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{

    [SerializeField] Text controlsInstruction;
    bool minimised = false;

    [SerializeField] AudioSource backgroundAudio;
    bool muted = false;

    private float horizontalInput;
    private float verticalInput;
    private float zInput;
    private float savedXPOS;
    private float savedYPOS;
    private float savedYPOS_offSet;
    private float savedZPOS;
    private float savedZPOS_offSet;

    private Rigidbody rigidBodyComponent;

    // Start is called before the first frame update
    void Start()
    {
        rigidBodyComponent = GetComponent<Rigidbody>();
        controlsInstruction.text = "Press Trigger to drop item.\nPress A to mute/unmute music.\nPress B to minimize/show this instruction";

    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.idleCountdownTimer -= Time.deltaTime;

        if (GameManager.instance.idleCountdownTimer < 0.3)
        {
            GameManager.instance.idleCountdownTimer = 0;
        }

        if (GameManager.instance.idleCountdownTimer == 0 && GameManager.instance.isCounting)
        {
            GameManager.instance.UpdateGameState(GameState.Food);
            GameManager.instance.isCounting = false;
        }
        /* Keybard test 
         * 
         * // axis will return a float
        horizontalInput = Input.GetAxis("Horizontal") * 5; // increase speed
        verticalInput = Input.GetAxis("Vertical") * 5;
        zInput = Input.GetAxis("Zaxis") * 5;
        */

        // get coordinates from MyPAM controller 

        /* myPAM coords:
         * x (-150 to 150)
         * y (-105 to 105)
         * z (0 to 120)
         * 
         * game coords:
         * x (-8 to 8)
         * y (5.91 to 15.91) [z in myPAM]
         * z (-12.2 to -1) [y in myPAM]
         * 
         * To optimize rehab, x and y have to be scaled down equally, i.e SF of 18.75
         */

        savedXPOS = (float)(UDP_Handling.X2pos / 18.75f);
        savedYPOS = (float)(UDP_Handling.Y2pos / 18.75f);
        savedYPOS_offSet = savedYPOS - 6.6f; // offset y since its range is -12.2 to -1
        savedZPOS = (float)(UDP_Handling.Z2pos / 12.0f);
        savedZPOS_offSet = savedZPOS + 5.91f; // offset z since its range is 5.91 to 15.91


        // control instructions
        if (OVRInput.GetUp(OVRInput.RawButton.B) && !minimised)
        {
            controlsInstruction.text = "";
            minimised = true;
        } else if (OVRInput.GetUp(OVRInput.RawButton.B) && minimised)
        {
            controlsInstruction.text = "Press Trigger to drop item.\nPress A to mute/unmute music.\nPress B to minimize/show this instruction";
            minimised = false;
        }

        if (OVRInput.GetUp(OVRInput.RawButton.A) && !muted)
        {
            backgroundAudio.volume = 0;
            muted = true;
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.A) && muted)
        {
            backgroundAudio.volume = 0.5f;
            muted = false;
        }

    }

    // Update is called once per physics frame
    private void FixedUpdate()
    {
        rigidBodyComponent.transform.position = new Vector3(savedXPOS, savedZPOS_offSet, savedYPOS_offSet);

        // rigidBodyComponent.velocity = new Vector3(horizontalInput, verticalInput, zInput);
    }
}
