using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomNumber : MonoBehaviour {

    public Texture[] numberTextures = new Texture[11];
    public InputField inputNumber;
    public Text scoreText;
    public Text gameStatus;
    public Text randomText;
    public GameObject button;
    public GameObject number;

    float score;
    float randomNumber;
    float guessedNumber;

    // Use this for initialization
    void Start () {
        // Set score to 0 on start up
        score = 0;
        // Set up score text
        scoreText.text = "Score : " + score;
        // Get our random number to guess
        NewNumberToGuess();
    }

    public void ButtonClick()
    {
        // Get our inputted value when the button has been pressed
        float.TryParse(inputNumber.text, out guessedNumber);
        // If the inputted number is the same as our random number...
        if(guessedNumber == randomNumber)
        {
            // Add 1 to the score
            score += 1.0f;
            // Display that the user has guessed correctly
            gameStatus.text = "Good Job!";
            // Get a new number to guess
            NewNumberToGuess();
            // Update the score text
            scoreText.text = "Score : " + score;
        }
        else
        {
            // Output try again if the user has guessed incorrectly
            gameStatus.text = "Try Again!";
        }
    }

    void NewNumberToGuess()
    {
        // Get a random number
        randomNumber = Mathf.Round(Random.Range(0.0f, 10.0f));
        // Set the number text
        randomText.text = randomNumber.ToString();

        // Get the renderer for the number cube
        Renderer rend = number.GetComponent<Renderer>();
        int num = (int) randomNumber;
        // Set the cubes texture
        rend.material.SetTexture(0, numberTextures[num]);
    }
}
