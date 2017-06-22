using UnityEngine;
using System.Collections;

public class DogMovement : MonoBehaviour {

    public static float Moves;  // How many spaces the dog has to move
    public static float Score;  // The player's score

    public GameObject dog;      // The Dog game object
    public Animator anim;       // Animator object
    public float speed = 1.0f;  // How fast the dog moves

    Vector3 currentPosition;    // Where the dog currently is
    Vector3 newPosition;        // Where the dog is heading towards
    Vector3 lastPosition;       // The last position the dog was at
    Vector3 facing;             // The direction the dog is facing

    ParticleSystem part;        // Stores the particle system for the bones

    float partimer = 2.0f;      // Timer for the particle system to stay on
    float movespeed;            // Speed at which the dog is moving

    void Start()
    {
        // Give the dog 0 moves to start of with
        Moves = 0;
        // Give the player 0 score to start of with
        Score = 0;
        // Find & Set the current position of the dog
        currentPosition = dog.transform.position;
        newPosition = dog.transform.position;

        // Face the dog counter clockwise
        anim.SetInteger("Facing", 2);
    }

    void Update()
    {
        // If the dog still has moves to make...
        if (Moves > 0)
        {
            // Check if there is a particle system playing
            ParticleCheck();

            // Change animator status to moving
            anim.SetBool("Moving", true);

            // If there are still moves and the dog hasn't been given a new position to move to
            if (currentPosition == newPosition)
            {
                // Store our last position as the position the dog is currently at
                lastPosition = currentPosition;
                // Determine where the dog will move next
                MovementManagement();
            }
            // Dog has reached the new position
            else if (dog.transform.position == newPosition)
            {
                // Set current to new position
                currentPosition = newPosition;
                // Take 1 off the moves the dog needs to make
                Moves -= 1;
                // If this is the final move...
                if (Moves == 0)
                {
                    // See if the dog has landed on a fish
                    GetScore();
                }
                movespeed = 0.0f;
            }
            else // Else the dog is still moving
            {
                // Determine the speed which the dog is moving at
                movespeed += speed * Time.deltaTime;
                // Move dog towards the new position
                dog.transform.position = Vector3.Lerp(currentPosition, newPosition, movespeed);
            }

        }
        else // Else if the dog is not moving...
        {
            // Change animator status to not moving
            anim.SetBool("Moving", false);
        }

        // Update Particle System
        ParticleCheck();
    }

    void MovementManagement()
    {
        // Get the direction the dog is currently facing
        GetDirection();

        RaycastHit hit;

        // Use Raycast to find where the next board piece in front of dog
        if (Physics.Raycast(lastPosition, facing, out hit, 10.0f, LayerMask.GetMask("Board")))
        {
            // Make new position into where the next ice cube is
            newPosition = hit.transform.position;
        }
        else // Board piece is not in front of dog
        {
            // Check which direction the dog is currently facing
            if (anim.GetInteger("Facing") == 1)
            {
                anim.SetInteger("Facing", 4); // Facing Up -> Facing Left
            }
            else if (anim.GetInteger("Facing") == 2)
            {
                anim.SetInteger("Facing", 1); // Facing Right -> Facing Up
            }
            else if (anim.GetInteger("Facing") == 3)
            {
                anim.SetInteger("Facing", 2); // Facing Down -> Facing Right
            }
            else if (anim.GetInteger("Facing") == 4)
            {
                anim.SetInteger("Facing", 3); // Facing Left -> Facing Down
            }
        }
    }

    void GetDirection()
    {
        // Find the value Facing is currently at
        if (anim.GetInteger("Facing") == 1)
        {
            // 1 = Facing up (away from the screen)
            facing = Vector3.up;
        }
        if (anim.GetInteger("Facing") == 2)
        {
            // 2 = Facing right
            facing = Vector3.right;
        }
        if (anim.GetInteger("Facing") == 3)
        {
            // 3 = Facing down (towards the screen)
            facing = Vector3.down;
        }
        if (anim.GetInteger("Facing") == 4)
        {
            // 4 = Facing left
            facing = Vector3.left;
        }
    }

    void GetScore()
    {
        // Find if the dog has picked up a bone
        RaycastHit hit;

        // Use the last position to cast the raycast as Unity does not pick up objects from the origin point
        if (Physics.Raycast(lastPosition, facing, out hit, 5.0f, LayerMask.GetMask("Bones")))
        {
            // Find what fish the penquin has picked up
            if (hit.transform.tag == "Black Bone")
            {
                Score -= 10; // Lose points for finding black bones
            }
            if (hit.transform.tag == "White Bone")
            {
                Score += 10; // Gain points for finding regular bones
            }
            if (hit.transform.tag == "Gold Bone")
            {
                Score += 20; // Gain extra points for finding golden bones
            }

            // Set the particle system to be the one the bone uses
            part = hit.transform.GetComponentInChildren<ParticleSystem>();

            // Play the particle system
            part.Play();
        }
    }

    void ParticleCheck()
    {
        // Check if there is a particle system in place
        if (part != null)
        {
            // Check if the particle system is still playing
            if (part.isPlaying == true)
            {
                // If so update the timer
                partimer -= Time.deltaTime;

                // If the timer has reaced 0 or the dog is moving
                if (partimer <= 0 || Moves > 0)
                {
                    // Stop the particle system from playing
                    part.Stop();

                    // Reset the timer
                    partimer = 2.0f;
                }
            }
        }
    }
}

