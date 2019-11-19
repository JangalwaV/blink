using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    [SerializeField] bool _invertVertical = false;

    public event Action<Vector3> MoveInput = delegate { };      //Uses a Vector3 to find which direction to move
    public event Action<Vector3> RotateInput = delegate { };    //uses a Vector3 to find which direction to rotate
    public event Action JumpInput = delegate { };
    public event Action BlinkInput = delegate { };


    void Update()
    {
        DetectMoveInput();
        DetectRotateInput();
        DetectJumpInput();
        DetectBlinkInput();
    }

    void DetectJumpInput()
    {
        //check for Spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpInput?.Invoke();
        }
    }

    void DetectBlinkInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            BlinkInput?.Invoke();
        }
    }

    void DetectRotateInput()
    {
        //get input from controller as a 0 or 1
        float xInput = Input.GetAxisRaw("Mouse X");
        float yInput = Input.GetAxisRaw("Mouse Y");

        //Parse input
        if (xInput != 0 || yInput != 0)
        {
            //check for inverted camera
            if (_invertVertical)
            {
                yInput = -yInput;
            }

            //combine into a single vector
            Vector3 rotation = new Vector3(yInput, xInput, 0);

            //notify rotate
            RotateInput?.Invoke(rotation);
        }
    }

    void DetectMoveInput()
    {
        //get input from controller as a 0 or 1
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        //Parse input
        if (xInput != 0 || yInput != 0)
        {
            //convert to local direction
            Vector3 _horizontalMovement = transform.right * xInput;
            Vector3 _forwardMovement = transform.forward * yInput;

            //combine into a single vector
            Vector3 movement = (_horizontalMovement + _forwardMovement).normalized;

            //notify move
            MoveInput?.Invoke(movement);
        }
    }
}
