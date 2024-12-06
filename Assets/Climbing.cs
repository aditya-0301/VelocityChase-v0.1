using System.Collections;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    private int climbLayer;
    public Camera cam;
    private float playerHeight = 3f;
    private float playerRadius = 0.5f;
    private bool isClimbing = false;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Only check the ClimbLayer, no need for bitwise negation
        climbLayer = LayerMask.GetMask("ClimbLayer");
        climbLayer = ~climbLayer;  
    }

    void Update()
    {
        Climb();
        animator.SetBool("IsClimbing", isClimbing);
    }

    private void Climb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isClimbing) // Prevent starting another climb while climbing
        {
            // Raycast for climbable surface
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var firstHit, 0.5f, climbLayer))
            {
                Debug.Log("Climbable in front");

                // Raycast downward to find the point to move to
                Vector3 rayStart = firstHit.point + (cam.transform.forward * 1f * playerRadius) + (Vector3.up * playerHeight);
                if (Physics.Raycast(rayStart, Vector3.down, out var secondHit, playerHeight))
                {
                    Debug.Log("Found place to land");
                    StartCoroutine(LerpClimb(secondHit.point, 0.5f));
                }
                else
                {
                    Debug.Log("No valid landing spot found");
                }
            }
            else
            {
                Debug.Log("No climbable surface detected");
            }
        }
    }

    IEnumerator LerpClimb(Vector3 targetPosition, float duration)
    {
        isClimbing = true; // Set climbing state as true at the start of the coroutine
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isClimbing = false; // Set climbing state as false when done
    }
}
