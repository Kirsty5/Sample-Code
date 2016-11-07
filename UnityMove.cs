using UnityEngine;
using System.Collections;

public class UnityMove : MonoBehaviour {

    public float speed = 5.0f;
    public float turnSmoothing = 15f;   // A smoothing value for turning the player.
    public float speedDampTime = 0.1f;  // The damping for the speed parameter
    public float jumpPower = 10.0f;
    public float useCurvesHeight = 0.5f;

    public float airHeight = 0.0f; // Height in air, is 0 when on ground

    public float distanceFromGround = 0.5f;

    public float fallDamageRate = 10.0f; // A value to determine how much damage is taken from falling

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

    private Animator anim;              // Reference to the animator component.
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

    // Use this for initialization
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
	
	// Update is called once per frame
	void FixedUpdate () {
        // We only wanna do stuff here if the player is in fact still alive

        float health = stats.GetHealth();

        isPaused = menu.pauseStatus();

        bool hasWon = stats.GameStatus();

        if (health > 0 && isPaused == false && hasWon == false)
        {
            // First thing we do is check if player is on the ground
            CheckGroundStatus();

            // Right let's check for if we need to calculate fall damage
            // First let's check the current height with the last stored height
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
            // We can do this here since fall damage is only added once we've checked for the right conditions
            if (fallDamage > 0)
                CheckFallDamage();

            SetSpeed();
        }
    }

    private void SetSpeed()
    {
        float h = Input.GetAxis("Horizontal"); // Lefty Righty
        float v = Input.GetAxis("Vertical"); // Uppy Downy

        if (h != 0 || v != 0)
        {
            MovementManagement(h, v);
            anim.SetFloat("Speed", speed);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

        if(Input.GetButton("Jump"))
        {
                anim.SetBool("Jump", true);

                JumpManagement();
        }
        else
        {
            anim.SetBool("Jump", false);
        }

    }

    private void MovementManagement(float horizontal, float vertical)
    {
        Rotating(horizontal, vertical);
        Vector3 Movement = new Vector3(horizontal, 0.0f, vertical);
        transform.position += (Movement * speed * Time.deltaTime);
    }

    private void Rotating(float horizontal, float vertical)
    {
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
        if (isGrounded == true)
        {
            Jump = new Vector3(0.0f, jumpPower * Time.deltaTime * 2.0f, 0.0f);
        }
        transform.position += Jump;
    }
    
    // Taken from the standard assets script
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        // So we're using this raycast to see if the model has touched the ground
        // The distance from ground depends on the transform of the model, for unity chan it seems to be the middle of her
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, distanceFromGround))
        {
            GroundNormal = hitInfo.normal;
            isGrounded = true;
            print("Grounded");
        }
        else
        {
            isGrounded = false;
            GroundNormal = Vector3.up;
            print("Airborn");
            timeInAir += Time.deltaTime;
        }
    }

    private void CheckFallDamage()
    {
        print(fallDamage); // A test to see if the fall damage is being calculated

        stats.Damage(Mathf.Round(fallDamage * fallDamageRate)); // Throw the fall damage into the stats script

        fallDamage = 0; // Be sure we reset fall damage or else this method will call itself again until we do
        
    }
}
