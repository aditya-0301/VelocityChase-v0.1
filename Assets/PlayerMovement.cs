using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;


    public float walkSpeed = 12f;        // Normal walking speed
    public float sprintSpeed = 18f;      // Speed when sprinting
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float slideSpeed = 20f;       // Initial sliding speed
    public float slideDuration = 1f;     // Duration of the slide in seconds
    public float slideFriction = 8f;     // Friction applied to slow down the slide

    public float crouchHeight = 0.5f;    // Height when crouching
    public float normalHeight = 2.0f;     // Normal height of the player
    public float crouchSpeed = 6f;       // Speed while crouching

    private Vector3 velocity;
    private bool isGrounded;
    private bool isSliding = false;      // Is the player currently sliding?
    private Vector3 slideVelocity;       // Velocity during the slide
    private float currentSlideTime = 0f; // Track time spent sliding

    private bool isJumping = false;
    private bool isSprinting = false;    // Toggle for sprinting
    private bool isCrouching = false;    // Is the player currently crouching?

    private Animator animator;




    public bool IsSliding()
    {
        return isSliding;
    }

    void Start()
    {
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        
        
        // Ground detection using CharacterController's isGrounded property
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Apply a small downward force to keep the player grounded
        }

        // Capture movement input for horizontal and vertical axes
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movementDirection = transform.right * x + transform.forward * z; //Creates a Vector3 for movementDirection

        if (movementDirection != Vector3.zero) //Check whether the character is moving or not and setting "IsMoving" based on that
        {
            animator.SetBool("IsMoving", true);
        }

        else
        {
            animator.SetBool("IsMoving", false);
        }

        
        

        // Toggle sprint on Shift press (and cancel slide if already sliding)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isSliding)
            {
                // Cancel slide if Shift is pressed while sliding
                isSliding = false;
            }
            else
            {
                // Toggle sprinting state
                isSprinting = !isSprinting;
            }
        }

        if (movementDirection == Vector3.zero)
        {
            isSprinting = false; // Stop sprinting if no movement input
        }

        // Handle crouching mechanics
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isSprinting && !isSliding && isGrounded)
        {
            isCrouching = !isCrouching; // Toggle crouching state

            if (isCrouching)
            {
                // Reduce the height for crouching and adjust the center so the player doesn't sink into the ground
                controller.height = crouchHeight;
                controller.center = new Vector3(controller.center.x, crouchHeight / 2, controller.center.z);
            }
            else
            {
                // Reset the height and center when standing back up
                controller.height = normalHeight;
                controller.center = new Vector3(controller.center.x, normalHeight / 2, controller.center.z);
            }
        }

        // Handle sliding mechanics
        if (isSliding)
        {
            // Slide logic: reduce slide velocity over time to simulate friction
            currentSlideTime += Time.deltaTime;
            slideVelocity = Vector3.Lerp(slideVelocity, Vector3.zero, slideFriction * Time.deltaTime);

            // Move the player using the slide velocity
            controller.Move(slideVelocity * Time.deltaTime);

            // End the slide after the duration or when speed is near zero
            if (currentSlideTime >= slideDuration || slideVelocity.magnitude < 0.1f)
            {
                isSliding = false;
            }

            // Cancel the slide and jump if the player presses Space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSliding = false;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Apply jump force
            }
        }
        else
        {
            // Set movement speed based on whether the player is sprinting or crouching
            float speed = isSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : walkSpeed);
            Vector3 move = transform.right * x + transform.forward * z;

            
            

            // Move the player using the calculated speed
            controller.Move(move * speed * Time.deltaTime);

            // Start sliding when the Control key is pressed, if not crouching
            if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isCrouching)
            {
                isSliding = true;
                currentSlideTime = 0f; // Reset slide timer
                slideVelocity = move.normalized * slideSpeed; // Calculate the initial sliding velocity
            }

            // Jump logic: apply jump velocity if the player is grounded and presses the Space key
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            }
        }
        

        // Apply gravity to the player
        velocity.y += gravity * Time.deltaTime;

        // Apply the vertical velocity (gravity) to the CharacterController
        controller.Move(velocity * Time.deltaTime);

        // To change IsSprinting to True or False
        if (isSprinting && movementDirection != Vector3.zero)
        {
            animator.SetBool("IsSprinting", true);
        }
        else
        {
            animator.SetBool("IsSprinting", false);
        }

        //To change IsSliding to True or False
        if (isSliding && movementDirection != Vector3.zero)
        {
            animator.SetBool("IsSliding",true);
        }
        else
        {
            animator.SetBool("IsSliding",false);
        }

        //To Change IsJumping to True or False
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsJumping",true) ;
        }
        else
        {
            animator.SetBool("IsJumping",false) ;
        }
    }
}
