using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalNetworkInput;

public class CharacterMovement : MonoBehaviour
{
    public enum Direction
    {
        None, Top, Right, Bottom, Left
    }
    [Header("Player Setting")]
    public bool useKeyboard;
    [ShowIf("UsingController")]
    public int playerNumber;

    [Header("Initial Setup"), Tooltip("Only applicable before the start of the game")]
    public Direction initialDirection;

    [Header("Controller Setup"), Tooltip("Velocity of the player")]
    public float velocity = 5.0f;
    [Tooltip("Which rate the control will consider as a input")]
    public float controlRate = 0.8f;
    [Tooltip("Size of the buffer used for changing direction")]
    public int sizeBufferDirectionChange = 3;

    [InspectorReadOnly, SerializeField]
    private Vector3 direction;

    [InspectorReadOnly, SerializeField]
    private Direction currentDirection;
    private Direction nextDirection;
    private int bufferDirectionChange;
    private new Rigidbody rigidbody;
    private Vector3 tempVector;

    // Use this for initialization
    void Start()
    {
        bufferDirectionChange = 0;
        currentDirection = Direction.None;
        nextDirection = Direction.None;

        ChangeDirection(initialDirection);
        rigidbody = GetComponent<Rigidbody>();
    }
#if UNITY_EDITOR
    private bool UsingController(Object nu)
    {
        return !useKeyboard;
    }
#endif
    // Update is called once per frame
    void Update()
    {
        UpdateInput();
        UpdateBuffer();
        CalculateVelocity();
    }

    private void CalculateVelocity()
    {
        tempVector = rigidbody.velocity;
        tempVector.x = direction.x * velocity;
        tempVector.z = direction.z * velocity;
        rigidbody.velocity = tempVector;
    }

    private float tempH, tempV;
    private void UpdateInput()
    {
        if (useKeyboard)
        {
            tempH = Input.GetAxis("Horizontal");
            tempV = Input.GetAxis("Vertical");
        }
        else
        {
            tempH = UNInput.GetAxis(playerNumber, AxisCode.LSH);
            tempV = UNInput.GetAxis(playerNumber, AxisCode.LSV);
        }

        if (Mathf.Abs(tempH) > controlRate)
        {
            if (tempH > 0)
                ChangeDirection(Direction.Right);
            else
                ChangeDirection(Direction.Left);
        }
        else if (Mathf.Abs(tempV) > controlRate)
        {
            if (tempV > 0)
                ChangeDirection(Direction.Top);
            else
                ChangeDirection(Direction.Bottom);
        }

    }


    public void UpdateBuffer()
    {
        if (bufferDirectionChange > 0)
        {
            bufferDirectionChange--;
            if (bufferDirectionChange == 0)
            {
                ChangeDirection(nextDirection, true);
            }
        }

    }

    private void ChangeDirection(Direction newDirection, bool bufferInput = false)
    {
        if (newDirection == currentDirection)
            return;

        if(!CheckCanChange())
        {
            if (bufferInput) // desconsider if this command was called from buffer
                return;

            nextDirection = newDirection;
            bufferDirectionChange = sizeBufferDirectionChange;

            return;
        }

        bufferDirectionChange = 0;
        nextDirection = Direction.None;
        currentDirection = newDirection;
        switch (newDirection)
        {
            case Direction.Top:
                direction.x = 0;
                direction.z = 1;
                break;
            case Direction.Right:
                direction.x = 1;
                direction.z = 0;
                break;
            case Direction.Bottom:
                direction.x = 0;
                direction.z = -1;
                break;
            case Direction.Left:
                direction.x = -1;
                direction.z = 0;
                break;
        }
    }

    private bool CheckCanChange()
    {
        return true;
    }

   
}
