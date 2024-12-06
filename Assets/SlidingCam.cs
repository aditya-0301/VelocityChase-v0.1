using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCam : MonoBehaviour
{
    public Transform player;              // Reference to the player object (or player movement script)
    public Vector3 cameraOffset;          // Offset of the camera from the player
    public float slideCameraHeight = 0.5f; // Height the camera should move to when sliding
    public float cameraLowerSpeed = 5f;   // Speed of the camera lowering when sliding

    private float originalCameraHeight;   // Original camera height when not sliding
    private bool isSliding = false;       // Track if the player is sliding
    private Vector3 targetCameraPosition; // Target position for the camera

    private PlayerMovement playerMovement; // Reference to the player's movement script

    void Start()
    {
        // Store the original camera height
        originalCameraHeight = transform.localPosition.y;
        // Store the target position initially
        targetCameraPosition = transform.localPosition;

        // Get reference to the PlayerMovement script from the player object
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Check if the player is currently sliding
        isSliding = playerMovement.IsSliding();

        // Adjust camera position based on sliding state
        if (isSliding)
        {
            // Target position is lowered for sliding
            targetCameraPosition = new Vector3(transform.localPosition.x, slideCameraHeight, transform.localPosition.z);
        }
        else
        {
            // Target position is the original camera height when not sliding
            targetCameraPosition = new Vector3(transform.localPosition.x, originalCameraHeight, transform.localPosition.z);
        }

        // Smoothly move the camera towards the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetCameraPosition, cameraLowerSpeed * Time.deltaTime);
    }
}
