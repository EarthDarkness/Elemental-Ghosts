using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalNetworkInput;

public class PlayerInput : MonoBehaviour {

    public enum Direction
    {
        None = -1, Top, Right, Bottom, Left
    }
    [Header("Player Setting")]
    public bool useKeyboard;
    [ShowIf("UsingController")]
    public int playerNumber;
    [Tooltip("Which rate the control will consider as a input"), ShowIf("UsingController")]
    public float controlRate = 0.8f;

    public CharacterMovement movement;
    public ElementBending elementBending;

#if UNITY_EDITOR
    private bool UsingController(Object nu)
    {
        return !useKeyboard;
    }
#endif

    // Use this for initialization
    void Start () {
        if (movement == null)
            movement = GetComponent<CharacterMovement>();
        if (elementBending == null)
            elementBending = GetComponent<ElementBending>();
	}

    private float tempH, tempV;
    private bool action;
    void Update () {
        if (useKeyboard)
        {
            tempH = Input.GetAxisRaw("Horizontal");
            tempV = Input.GetAxisRaw("Vertical");
            action = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X);
        }
        else
        {
            tempH = UNInput.GetAxis(playerNumber, AxisCode.LSH);
            tempV = UNInput.GetAxis(playerNumber, AxisCode.LSV);
            action = UNInput.GetButtonDown(playerNumber, ButtonCode.A) || UNInput.GetButtonDown(playerNumber, ButtonCode.RightBumper);
        }

        if (Mathf.Abs(tempH) > controlRate)
        {
            if (tempH > 0)
                movement.ChangeDirection(Direction.Right);
            else
                movement.ChangeDirection(Direction.Left);
        }
        else if (Mathf.Abs(tempV) > controlRate)
        {
            if (tempV > 0)
                movement.ChangeDirection(Direction.Top);
            else
                movement.ChangeDirection(Direction.Bottom);
        }

        if(action)
        {
            elementBending.Action();
        }
    }


}
