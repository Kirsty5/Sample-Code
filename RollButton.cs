using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RollButton : MonoBehaviour {

    public TextMesh sign;   // The sign which displays what the user has rolled

    float newRoll;          // For storing the new value of the roll

    // The Roll Button has been clicked with a computer mouse
    void OnMouseDown()
    {
        // If the dog is not currently moving...
        if (DogMovement.Moves == 0)
        {
            // Make a new roll
            NewRoll();
        }
    }

    void Update()
    {
        // Check for Mobile input 
        if (Input.touchCount > 0)
        {
            // Store the touch
            Touch touch = Input.GetTouch(0);
            // Check that the touch is on the roll button
            if (touch.position == new Vector2(transform.position.x, transform.position.y))
            {
                // Finally check to see if the dog has no moves
                if(DogMovement.Moves == 0)
                {
                    // Make a new roll if so
                    NewRoll();
                }
            }
                
        }

        // Update the sign in case score or roll has been updated
        sign.text = "Score : " + DogMovement.Score + "\n" + "Roll : " + newRoll;
    }

    void NewRoll()
    {
        // Get the value of the roll
        newRoll = Mathf.Round(Random.Range(1.0f, 6.0f));
        // Set the new number of moves the penquin has to make
        DogMovement.Moves = newRoll;
    }
}
