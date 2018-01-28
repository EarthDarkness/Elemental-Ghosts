using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalNetworkInput;

public class CharacterCreated : ASignal<CharacterMovement> { }
public class CharacterMovement : MonoBehaviour
{

    [Header("Initial Setup"), Tooltip("Only applicable before the start of the game")]
    public PlayerInput.Direction initialDirection = PlayerInput.Direction.Right;

	[Header("Controller Setup"), Tooltip("Velocity of the player")]
	public static float baseVelocity = 5.0f;
    public float velocity = baseVelocity;
    [Tooltip("Size of the buffer used for changing direction")]
    public int sizeBufferDirectionChange = 3;
    public float timeCollisionThreshold = 0.5f;
    public bool ignoreAlign;

    [Header("Debug"), InspectorReadOnly, SerializeField]
    private Vector3 direction;

    [InspectorReadOnly, SerializeField]
    private PlayerInput.Direction _currentDirection;
    public PlayerInput.Direction currentDirection { get { return _currentDirection; } }

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
        _currentDirection = PlayerInput.Direction.None;
        nextDirection = PlayerInput.Direction.None;

        ChangeDirection(initialDirection);
        rigidbody = GetComponent<Rigidbody>();

        Signals.Get<CharacterCreated>().Dispatch(this);

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

    public void ChangeDirection(Vector3 newDirection)
    {
        newDirection.Normalize();
        _currentDirection = PlayerInput.Direction.None;
        direction = newDirection;
        transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);

    }
    public void ChangeDirection(PlayerInput.Direction newDirection)
    {
        ChangeDirection(newDirection, false);
    }
    private void ChangeDirection(PlayerInput.Direction newDirection, bool bufferInput)
    {
        if (newDirection == _currentDirection)
            return;

        if (!CheckCanChange())
        {
            if (bufferInput) // desconsider if this command was called from buffer
                return;

            nextDirection = newDirection;
            bufferDirectionChange = sizeBufferDirectionChange;

            return;
        }

        bufferDirectionChange = 0;
        nextDirection = PlayerInput.Direction.None;
        _currentDirection = newDirection;
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
        transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private bool CheckCanChange()
    {
        return aligned || ignoreAlign;
    }

    private float lastTimeCollision;
    public void OnCollisionEnter(Collision collision)
    {
        if (Time.time >= lastTimeCollision)
        {
            lastTimeCollision = Time.time + timeCollisionThreshold;


            if (Vector3.Dot(direction, collision.contacts[0].normal) < -0.5f)
                ChangeDirection((PlayerInput.Direction)((((int)_currentDirection) + 2) % 4));

        }

    }



}
