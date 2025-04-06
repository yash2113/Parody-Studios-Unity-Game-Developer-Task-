using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    public Transform hologram;         // Assign hologram model in Inspector
    public Transform playerParent;     // Parent of the player (e.g., the visual model and collider)
    public Rigidbody playerRb;         // Rigidbody on the player object
    public float gravityStrength = 9.81f;
    public float rotationSpeed = 5f;
    public Transform pivotPoint;       // Child empty GameObject used as the rotation pivot

    private Vector3 newGravityDirection = Vector3.down; // Default gravity direction
    private bool isSelectingGravity = false;

    private void Start()
    {
        hologram.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleGravitySelection();
        ApplyGravity();
    }

    void HandleGravitySelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            hologram.gameObject.SetActive(true);
            hologram.localRotation *= Quaternion.Euler(-90f, 0f, 0f);
            newGravityDirection = -hologram.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hologram.gameObject.SetActive(true);
            hologram.localRotation *= Quaternion.Euler(90f, 0f, 0f);
            newGravityDirection = -hologram.up;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hologram.gameObject.SetActive(true);
            hologram.localRotation *= Quaternion.Euler(0f, 0f, 90f);
            newGravityDirection = -hologram.up;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hologram.gameObject.SetActive(true);
            hologram.localRotation *= Quaternion.Euler(0f, 0f, -90f);
            newGravityDirection = -hologram.up;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeGravity();
        }
    }

    void ChangeGravity()
    {
        // Set Unity's global gravity vector
        Physics.gravity = newGravityDirection * gravityStrength;
        isSelectingGravity = false;

        // Hide hologram and reset its rotation
        hologram.localRotation = Quaternion.identity;
        hologram.gameObject.SetActive(false);

        // Reset current velocity and apply immediate push in the new gravity direction
        playerRb.velocity = -newGravityDirection * gravityStrength * 2.5f;
        playerRb.angularVelocity = Vector3.zero;

        // Rotate the player to align with the new gravity direction
        Quaternion targetRotation = Quaternion.LookRotation(playerParent.forward, -newGravityDirection);
        StartCoroutine(RotatePlayerParent(targetRotation));
    }

    void ApplyGravity()
    {
        playerRb.AddForce(Physics.gravity, ForceMode.Acceleration);
    }

    IEnumerator RotatePlayerParent(Quaternion targetRotation)
    {
        Quaternion startRotation = playerParent.rotation;
        float elapsedTime = 0f;
        Vector3 pivotPosition = pivotPoint.position;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rotationSpeed;
            Quaternion currentRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);

            // Rotate around the pivot point
            Vector3 offset = playerParent.position - pivotPosition;
            offset = currentRotation * Quaternion.Inverse(startRotation) * offset;
            playerParent.position = pivotPosition + offset;

            playerParent.rotation = currentRotation;
            yield return null;
        }

        playerParent.rotation = targetRotation;
    }
}
