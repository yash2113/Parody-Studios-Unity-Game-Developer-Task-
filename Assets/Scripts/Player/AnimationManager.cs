using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ChildAnimatorController : MonoBehaviour
{
    private Animator animator;

    // These will be fetched from the parent
    private Rigidbody parentRb;
    private Transform groundCheck;
    private LayerMask groundLayer;

    [Header("Ground Check Settings")]
    public float groundCheckRadius = 0.2f;

    [Header("Movement Settings")]
    public float runThreshold = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Get required components from the parent
        Transform parent = transform.parent;
        if (parent != null)
        {
            parentRb = parent.GetComponent<Rigidbody>();
            groundCheck = parent.Find("GroundCheck"); // Make sure this exists
            PlayerMovement playerMovement = parent.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                groundLayer = playerMovement.groundLayer;
            }
        }

        if (animator == null || parentRb == null || groundCheck == null)
        {
            Debug.LogError("ChildAnimatorController setup is incomplete!");
        }
    }

    void Update()
    {
        if (parentRb == null || groundCheck == null)
            return;

        HandleAnimations();
    }

    void HandleAnimations()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        float horizontalSpeed = new Vector3(parentRb.velocity.x, 0, parentRb.velocity.z).magnitude;
        float verticalVelocity = parentRb.velocity.y;

        bool isRunning = isGrounded && horizontalSpeed > runThreshold;
        bool isFalling = !isGrounded && verticalVelocity < -0.1f;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isFalling", isFalling);
    }
}
