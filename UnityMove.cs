using UnityEngine;
using System.Collections;

public class UnityMove : MonoBehaviour {

    public float speed = 5.0f;
    public float turnSmoothing = 15f;   
    public float speedDampTime = 0.1f;  
    public float jumpPower = 10.0f;
    public float useCurvesHeight = 0.5f;

    public float airHeight = 0.0f; 

    public float distanceFromGround = 0.5f;

    public float fallDamageRate = 10.0f; 

    private float orgColHeight;
    private Vector3 orgVectColCenter;

    private Vector3 GroundNormal;

    private Vector3 Jump;

    private bool isGrounded;
    private float fallDamage = 0.0f;
    private float health;

    private float timeInAir = 0.0f;
    private float currentHeight;

    private CapsuleCollider col;

    private Animator anim;              
    private AnimatorStateInfo currentBaseState;
    private Rigidbody rgdbody;

    public GameObject gameMaster;

    private UnityStats stats;
    private UnityMenu menu;

    private bool isPaused;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int restState = Animator.StringToHash("Base Layer.Rest");

    void Start () {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rgdbody = GetComponent<Rigidbody>();

        orgColHeight = col.height;
        orgVectColCenter = col.center;

        currentHeight = transform.position.y;

        stats = GetComponent<UnityStats>();
        menu = gameMaster.GetComponent<UnityMenu>();
    }
	
	void FixedUpdate () {
	// Get Player health
        float health = stats.GetHealth();
	// Get if the Player has paused the game
        isPaused = menu.pauseStatus();
	// Get if the Player has won or lost the game
        bool hasWon = stats.GameStatus();
	// While the player is above 0 health, has not paused and has not won/lost the game
        if (health > 0 && isPaused == false && hasWon == false)
        {
            // Check if player is on the ground
            CheckGroundStatus();
		
            // Check if the player is currently falling by comparing current height to the one last stored
            if (transform.position.y < currentHeight)
            {
                // We only want to check for fall damage if there has been a significant time spent in the air
                if (timeInAir > 1.5f && isGrounded == true)
                {
                    fallDamage = timeInAir;
                    timeInAir = 0.0f;
                    currentHeight = transform.position.y;
                }
            }
            else if (isGrounded == true) // Else if the player is currently grounded make sure we reset timer and height
            {
                timeInAir = 0.0f;
                currentHeight = transform.position.y;
            }
            
		// If we have fall damage then calculate it
            if (fallDamage > 0)
                CheckFallDamage();
		// Finally we add the speed to the player
            SetSpeed();
        }
    }

    private void SetSpeed()
    {
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical"); 

	    // Calculate movement if there is input
        if (h != 0 || v != 0)
        {
            MovementManagement(h, v);
            anim.SetFloat("Speed", speed);
        }
        else // Otherwise make sure we reset the animation to idle
        {
            anim.SetFloat("Speed", 0);
        }

	    // Check if we're jumping
        if(Input.GetButton("Jump"))
        {
                anim.SetBool("Jump", true);

                JumpManagement();
        }
        else // Otherwise reset the jump animation
        {
            anim.SetBool("Jump", false);
        }

    }

    private void MovementManagement(float horizontal, float vertical)
    {
	    // If this is called we simply calculate the new position the player will move to and add to transform
        Rotating(horizontal, vertical);
        Vector3 Movement = new Vector3(horizontal, 0.0f, vertical);
        transform.position += (Movement * speed * Time.deltaTime);
    }

    private void Rotating(float horizontal, float vertical)
    {
	    // This exact function was taking from Unity's stealth tutorials // 
	    
        // Create a new vector of the horizontal and vertical inputs.
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

        // Create a rotation based on this new vector assuming that up is the global y axis.
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Create a rotation that is an increment closer to the target rotation from the player's rotation.
        Quaternion newRotation = Quaternion.Lerp(rgdbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

        // Change the players rotation to this new rotation.
        rgdbody.MoveRotation(newRotation);
    }

    private void JumpManagement()
    {
	    // If the player is grounded then that's when we want to calculate the jump height
        if (isGrounded == true)
        {
            Jump = new Vector3(0.0f, jumpPower * Time.deltaTime * 2.0f, 0.0f);
        }
        transform.position += Jump; // Simply add jump to current transform 
    }
    
    // Taken from the standard assets script
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        // See if the Raycast starting from the model has touched with the ground
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, distanceFromGround))
        {
            GroundNormal = hitInfo.normal;
            isGrounded = true;
            print("Grounded");
        }
        else // Otherwise we say we're in the air and add onto the time player has been airborn
        {
            isGrounded = false;
            GroundNormal = Vector3.up;
            print("Airborn");
            timeInAir += Time.deltaTime;
        }
    }

    private void CheckFallDamage()
    {
	    // This function is called when the fall damage variable is not 0
        print(fallDamage);
	// Calculate the damage the player takes in the player stats script
        stats.Damage(Mathf.Round(fallDamage * fallDamageRate)); 
	// Reset fall damage
        fallDamage = 0; 
        
    }
}
