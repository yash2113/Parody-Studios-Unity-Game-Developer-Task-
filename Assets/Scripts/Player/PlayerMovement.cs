using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (IsGrounded())
        {
            HandleMovement();
        }

        HandleRotation();
        HandleJump();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        rb.angularVelocity = Vector3.zero;
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * moveZ + camRight * moveX).normalized * moveSpeed;
    }

    void HandleRotation()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleAnimations()
    {
        bool grounded = IsGrounded();
        float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float verticalVelocity = rb.velocity.y;

        bool isRunning = grounded && horizontalSpeed > 0.1f;
        bool isFalling = !grounded && verticalVelocity < -0.1f;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isFalling", isFalling);
    }
}
