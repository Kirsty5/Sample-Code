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

        if (GlobalVar.isPaused == false)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (GlobalVar.isInteracting == false)
            {
                if (h > 0) { direction = Vector2.right; anim.SetInteger("Facing", 0); }
                else if (h < 0) { direction = Vector2.left; anim.SetInteger("Facing", 1); }
                else if (v > 0) { direction = Vector2.up; anim.SetInteger("Facing", 2); }
                else if (v < 0) { direction = Vector2.down; anim.SetInteger("Facing", 3); }

                newPos = new Vector3(h, v, 0) * Time.deltaTime;

                if (run)
                    transform.position += newPos * runspeed;
                else
                    transform.position += newPos * walkspeed;
            }

            if ((h == 0 && v == 0) || GlobalVar.isInteracting == true)
                anim.SetBool("isMoving", false);
            else
                anim.SetBool("isMoving", true);

            FindInteractable();

            FindPortal();
        }
        else
            anim.SetBool("isMoving", false);


        GlobalVar.playerFacing = direction;

    }

    void FindInteractable()
    {
        RaycastHit2D hitNPC = Physics2D.Raycast(transform.position, direction, 0.25f, LayerMask.GetMask("NPC"));
        RaycastHit2D hitInteract = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Interactables"));

        bool space = Input.GetKeyDown(KeyCode.Space);
        bool click = Input.GetMouseButtonDown(0);

        if (space || click)
        {
            // If we've hit an NPC
            if (hitNPC && GlobalVar.isInteracting == false)
            {
                GlobalVar.isInteracting = true;
                GlobalVar.interactingType = "NPC";
                GlobalVar.interactingWith = hitNPC.collider.gameObject.transform.name.ToString();
            }
            // If we've hit an object
            else if(hitInteract && !hitNPC && GlobalVar.isInteracting == false)
            {
                string interact = hitInteract.transform.name;
                GlobalVar.interactingType = "Object";
                GlobalVar.isInteracting = true;
                GlobalVar.interactingWith = interact;
            }
            else if (GlobalVar.doneInteracting == true)
            {
                GlobalVar.isInteracting = false;
                GlobalVar.doneInteracting = false;
            }

        }
        
    }

    void FindPortal()
    {
        RaycastHit2D hitPortal = Physics2D.Raycast(transform.position, direction, 0.3f, LayerMask.GetMask("Portal"));
        if(hitPortal)
        {
            string door = hitPortal.transform.name;

            print("Collided with " + door + ". Moving to new Location.");

            if(door == "Players House Enter")
            {
                transform.position = new Vector3(0,13.0f,0);
            }

            if (door == "Players House Exit")
            {
                transform.position = new Vector3(0, 0.5f, 0);
            }

            if (door == "Serenas House Enter")
            {
                transform.position = new Vector3(15.4f, 13.4f, 0);
            }

            if (door == "Serenas House Exit")
            {
                transform.position = new Vector3(5.0f, 0.5f, 0);
            }

            if (door == "Maisies Shop Enter")
            {
                transform.position = new Vector3(-13.2f, 13.2f, 0);
            }

            if (door == "Maisies Shop Exit")
            {
                transform.position = new Vector3(-5.0f, 0.5f, 0);
            }

            if (door == "Library Enter")
            {
                transform.position = new Vector3(-27.5f, -1.8f, 0);
            }

            if (door == "Library Exit")
            {
                transform.position = new Vector3(0.0f, -5.0f, 0);
            }
        }
    }
}
