using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalNetworkInput;

public class CharacterMovement : MonoBehaviour
{

    [Header("Initial Setup"), Tooltip("Only applicable before the start of the game")]
    public PlayerInput.Direction initialDirection = PlayerInput.Direction.Right;

    [Header("Controller Setup"), Tooltip("Velocity of the player")]
    public float velocity = 5.0f;
    [Tooltip("Size of the buffer used for changing direction")]
    public int sizeBufferDirectionChange = 3;

    [InspectorReadOnly, SerializeField]
    private Vector3 direction;

    [InspectorReadOnly, SerializeField]
    private PlayerInput.Direction currentDirection;
    private PlayerInput.Direction nextDirection;
    private int bufferDirectionChange;
    private new Rigidbody rigidbody;
    private Vector3 tempVector;

    [InspectorReadOnly]
    public bool aligned = true;
    // Use this for initialization
    void Start()
    {
        bufferDirectionChange = 0;
        currentDirection = PlayerInput.Direction.None;
        nextDirection = PlayerInput.Direction.None;

        ChangeDirection(initialDirection);
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void ChangeDirection(PlayerInput.Direction newDirection)
    {
        ChangeDirection(newDirection, false);
    }
    private void ChangeDirection(PlayerInput.Direction newDirection, bool bufferInput)
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
        nextDirection = PlayerInput.Direction.None;
        currentDirection = newDirection;
        switch (newDirection)
        {
            case PlayerInput.Direction.Top:
                direction.x = 0;
                direction.z = 1;
                break;
            case PlayerInput.Direction.Right:
                direction.x = 1;
                direction.z = 0;
                break;
            case PlayerInput.Direction.Bottom:
                direction.x = 0;
                direction.z = -1;
                break;
            case PlayerInput.Direction.Left:
                direction.x = -1;
                direction.z = 0;
                break;
        }
    }

    private bool CheckCanChange()
    {
        return aligned;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // invert direction
        //ChangeDirection((PlayerInput.Direction)((((int)currentDirection) + 2) % 4));
    }


   
}
