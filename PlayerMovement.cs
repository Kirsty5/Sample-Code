using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public Animator anim;
    public float walkspeed = 1.0f;
    public float runspeed = 2.0f;

    Vector3 newPos;
    Vector2 direction;

	void Update ()
    {
        bool run = Input.GetKeyDown(KeyCode.E);

	    // If the game is not paused...
        if (GlobalVar.isPaused == false)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
	
		// And the player is not interacting with an object...
            if (GlobalVar.isInteracting == false)
            {
		    // Determine which direction the player is moving in
                if (h > 0) { direction = Vector2.right; anim.SetInteger("Facing", 0); }
                else if (h < 0) { direction = Vector2.left; anim.SetInteger("Facing", 1); }
                else if (v > 0) { direction = Vector2.up; anim.SetInteger("Facing", 2); }
                else if (v < 0) { direction = Vector2.down; anim.SetInteger("Facing", 3); }

		    // Set up the new position the player is moving in
                newPos = new Vector3(h, v, 0) * Time.deltaTime;

		    // Check if the player is running and determine new position based on that
                if (run)
                    transform.position += newPos * runspeed;
                else
                    transform.position += newPos * walkspeed;
            }

		// If the player is moving set the right animation
            if ((h == 0 && v == 0) || GlobalVar.isInteracting == true)
                anim.SetBool("isMoving", false);
            else
                anim.SetBool("isMoving", true);

		// Check for interactables
            FindInteractable();
        }
        else // If the game is paused make sure the animation is not moving
            anim.SetBool("isMoving", false);

	// Store the direction player is facing in
        GlobalVar.playerFacing = direction;

    }

    void FindInteractable()
    {
	    // Set up raycasts for NPCs and other Interactable objects
        RaycastHit2D hitNPC = Physics2D.Raycast(transform.position, direction, 0.25f, LayerMask.GetMask("NPC"));
        RaycastHit2D hitInteract = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Interactables"));

        bool space = Input.GetKeyDown(KeyCode.Space);
        bool click = Input.GetMouseButtonDown(0);

	    // Check for player input to react to objects
        if (space || click)
        {
            // If we've hit an NPC...
            if (hitNPC && GlobalVar.isInteracting == false)
            {
		    // Store who we're interacting with 
                GlobalVar.isInteracting = true;
                GlobalVar.interactingType = "NPC";
                GlobalVar.interactingWith = hitNPC.collider.gameObject.transform.name.ToString();
            }
            // If we've hit an object...
            else if(hitInteract && !hitNPC && GlobalVar.isInteracting == false)
            {
		    // Store which object we've interacted with
                string interact = hitInteract.transform.name;
                GlobalVar.interactingType = "Object";
                GlobalVar.isInteracting = true;
                GlobalVar.interactingWith = interact;
            }
		// If we've finished interacting with an object
            else if (GlobalVar.doneInteracting == true)
            {
		    // Clear the data for who or what we've interacted with
                GlobalVar.isInteracting = false;
                GlobalVar.doneInteracting = false;
            }

        }
        
    }
}
