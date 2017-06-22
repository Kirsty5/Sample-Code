using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class TextBox : MonoBehaviour {

    public Text name;		            // Text Object where the name will be displayed
    public Text texts;		            // Text Object where the dialogue will be displayed

    public GameObject namePanel;        // Name Input Panel
    public InputField textfield;        // Where the player inputs their name
    public Button confirm;              // Confirm name selection

    public GameObject selectionPanel;   // Selection Panel
    public Button selection1;           // 1st Selectable option
    public Button selection2;           // 2nd Selectable option
    public Button selection3;           // 3rd Selectable option

    public GameObject character1;       // 1st (Main) character you see on screen
    public GameObject character2;       // 2nd character

    private static string[] lines;	    // Array to store the lines of text
    private bool selecting;             // Checks if player is selecting text or name
    private static bool loaded = false; // Determines if we have lines loaded

    void Start()
    {
		// By default set the selections to false
        namePanel.SetActive(false);
        selectionPanel.SetActive(false);
		// Set the character sprites to false
        character1.SetActive(false);
        character2.SetActive(false);
		// Add a listener to check if player has selected a name
        confirm.onClick.AddListener(SetName);

        selection1.onClick.AddListener(Selected1);
        selection2.onClick.AddListener(Selected2);
        selection3.onClick.AddListener(Selected3);
		
		// Change state of "selecting"
        selecting = false;
    }

    void Update()
    {
		// Get Inputs
        bool space = Input.GetKeyDown(KeyCode.Space);
        bool ctrl = Input.GetKey(KeyCode.LeftControl);

		// Check if we can progress with game
        if (selecting == false && GlobalVar.fileEnd == false && loaded == true && GlobalVar.isPaused == false)
        {
            // Add 1 to the line if we detect input
            if (space || ctrl)
            {
                GlobalVar.line += 1;
            }

        }

        // Cheat to make sure the selection screen goes off after each selection
        if (GlobalVar.line == 0)
        {
            selecting = false;
            selectionPanel.SetActive(false);
        }

		// If the line is not null display
        if (lines[GlobalVar.line] != null)
            DisplayText();
        else
        {
			// Else set an end of demo display
            name.text = "";
            texts.text = "End of demo.";
        }
    }

    public void OnClick()
    {
		// If a click has been found and we are not doing anything else go to next line
        if(selecting == false && GlobalVar.fileEnd == false && loaded == true && GlobalVar.isPaused == false)
            GlobalVar.line += 1;
    }

    void DisplayText()
    {
		// Make temp strings for the text
        string charname;
        string chartext;

		// Check if the current line contains "End"
        if (lines[GlobalVar.line].Contains("{End}"))
        {
			// Say we are not loading anymore lines
            GlobalVar.fileEnd = true;
            loaded = false;
			// If we find "RouteEnd" in the line then end the game
            if(lines[GlobalVar.line].Contains("{RouteEnd}"))
            {
                GlobalVar.gameEnd = true;
            }
        } 
		// Else if the current line asks to select a name
        else if (lines[GlobalVar.line].Contains("{NameTextField}"))
        {
            // Change state of "selecting" to true
            selecting = true;
            // Activate the select name panel
            namePanel.SetActive(true);
        } 
		// Else if the current line asks to select pronouns
        else if (lines[GlobalVar.line].Contains("{PronounSelection}"))
        {
            // Change state of "selecting" to true
            selecting = true;
            // Activate the selection panel
            selectionPanel.SetActive(true);

            // Make cut off points
            int cuttoff1;
            int cuttoff2;
            int cuttoff3;

            cuttoff1 = lines[GlobalVar.line].IndexOf('1');
            cuttoff2 = lines[GlobalVar.line].IndexOf('2');
            cuttoff3 = lines[GlobalVar.line].IndexOf('3');
			
			// Set selections to be the strings we get from the cut offs
            selection1.GetComponentInChildren<Text>().text = lines[GlobalVar.line].Substring(cuttoff1 + 1, (cuttoff2 - cuttoff1 -1 ));
            selection2.GetComponentInChildren<Text>().text = lines[GlobalVar.line].Substring(cuttoff2 + 1, (cuttoff3 - cuttoff2 - 1 ));
            selection3.GetComponentInChildren<Text>().text = lines[GlobalVar.line].Substring(cuttoff3 + 1);
        }
		// Else if the line asks us to make an affection related selection
        else if (lines[GlobalVar.line].Contains("{AffectionSelection}"))
        {
            // Say we are selecting something so we don't skip this
            selecting = true;
            // Activate the selection panel
            selectionPanel.SetActive(true);

            // Make cutt off points
            int cuttoff1;
            int cuttoff2;
            int cuttoff3;

            cuttoff1 = lines[GlobalVar.line].IndexOf('1');
            cuttoff2 = lines[GlobalVar.line].IndexOf('2');
            cuttoff3 = lines[GlobalVar.line].IndexOf('3');
			
			// Set selections to be the strings we get from the cut offs
            selection1.GetComponentInChildren<Text>().text = lines[GlobalVar.line].Substring(cuttoff1 + 1, (cuttoff2 - cuttoff1 - 1));
            selection2.GetComponentInChildren<Text>().text = lines[GlobalVar.line].Substring(cuttoff2 + 1, (cuttoff3 - cuttoff2 - 1));
            selection3.GetComponentInChildren<Text>().text = lines[GlobalVar.line].Substring(cuttoff3 + 1);
        }
        else // Else we have a regular line of dialogue
        {
            // Make sure selecting is reset here just in case
            selecting = false;

            // Change the line if it mentions variables such as pronouns or player name
            if(lines[GlobalVar.line].Contains("{PlayerName}"))
            {
                lines[GlobalVar.line]
                     = lines[GlobalVar.line].Replace("{PlayerName}", GlobalVar.playerName);
            }

            if (lines[GlobalVar.line].Contains("{SubjectPronoun}"))
            {
                lines[GlobalVar.line]
                     = lines[GlobalVar.line].Replace("{SubjectPronoun}", GlobalVar.subjectPronoun);
            }

            if (lines[GlobalVar.line].Contains("{ObjectPronoun}"))
            {
                lines[GlobalVar.line]
                     = lines[GlobalVar.line].Replace("{ObjectPronoun}", GlobalVar.objectPronoun);
            }

            if (lines[GlobalVar.line].Contains("{PossPronoun}"))
            {
                lines[GlobalVar.line]
                     = lines[GlobalVar.line].Replace("{PossPronoun}", GlobalVar.possPronoun);
            }

            // Set up int to store where the line cuts off
            int cuttoff1;
            int cuttoff2;

            // Find the point in line where it goes from name to dialogue
            cuttoff1 = lines[GlobalVar.line].IndexOf('[');
            cuttoff2 = lines[GlobalVar.line].IndexOf(']');

            // Set up our strings with the text
            charname = lines[GlobalVar.line].Substring(0, cuttoff1);
            chartext = lines[GlobalVar.line].Substring(cuttoff2 + 2);

            // Set the text of our text objects to the strings
            if (charname == "Blank")
                name.text = " ";
            else
                name.text = charname;

			// Change the text box test to be the text we've gotten from the line
            texts.text = chartext;
        }
    }

    public static void Load(string filename)
    {
        try
        {
            string line; // This will store the content of our text

			// Set up Stream Reader
            StreamReader textReader = new StreamReader("Assets/Text/" + filename + ".txt", Encoding.Default);

            using (textReader)
            {
				// Store all the text in our line string
                line = textReader.ReadToEnd();
				// Split up the text and store it in our line array
                lines = line.Split('\n');
				// Print to console that we have read the lines
                print("We have read the lines.");
            }
			// Close our file reader
            textReader.Close();
			// Print to console again that we have acquired text
            print("We have got the text");
            loaded = true;
        }
        catch
        {
			// If reading the file fails just print to console that we've failed
            print("No can do my man");
        }
    }

    void SetName()
    {
		// Remove the listener since we no longer need it
        confirm.onClick.RemoveListener(SetName);

		// Set the player's name to the one in the text field
        GlobalVar.playerName = textfield.text;
		// Move onto the next line
        GlobalVar.line += 1;
		// Say we are no longer selecting and de-activate the name panel
        selecting = false;
        namePanel.SetActive(false);
    }

	// Player has selected the first option in a selection screen
    void Selected1()
    {	
		// If the selection is for pronouns
        if (lines[GlobalVar.line].Contains("{PronounSelection}"))
        {
			// Change the player's pronouns to the ones selected
            GlobalVar.subjectPronoun = "He";
            GlobalVar.objectPronoun = "Him";
            GlobalVar.possPronoun = "Himself";
			// Move into the next line
            GlobalVar.line += 1;
			// Say we are no longer selecting and de-activate the selection panel
            selecting = false;
            selectionPanel.SetActive(false);
        }
		// Else the selection is for affection
        else
        {
			// Say we have reached the end of the file
            GlobalVar.fileEnd = true;
			// Move to the next game event based on selection
            GlobalVar.gameEvent += "A";
			// Say we are no longer selecting and de-activate the selection panel
            selecting = false;
            selectionPanel.SetActive(false);
        }
    }

	// Player has selected the second option in a selection screen
    void Selected2()
    {
		// If the selection is for pronouns
        if (lines[GlobalVar.line].Contains("{PronounSelection}"))
        {
			// Change the player's pronouns to the ones selected
            GlobalVar.subjectPronoun = "She";
            GlobalVar.objectPronoun = "Her";
            GlobalVar.possPronoun = "Herself";
			// Move into the next line
            GlobalVar.line += 1;
			// Say we are no longer selecting and de-activate the selection panel
            selecting = false;
            selectionPanel.SetActive(false);
        }
		// Else the selection is for affection
        else
        {
			// Say we have reached the end of the file
            GlobalVar.fileEnd = true;
			// Move to the next game event based on selection
            GlobalVar.gameEvent += "B";
			// Say we are no longer selecting and de-activate the selection panel
            selecting = false;
            selectionPanel.SetActive(false);
        }
    }

	// Player has selected the third option in a selection screen
    void Selected3()
    {
		// If the selection is for pronouns
        if (lines[GlobalVar.line].Contains("{PronounSelection}"))
        {
			// Change the player's pronouns to the ones selected
            GlobalVar.subjectPronoun = "They";
            GlobalVar.objectPronoun = "Them";
            GlobalVar.possPronoun = "Themself";
			// Move into the next line
            GlobalVar.line += 1;
			// Say we are no longer selecting and de-activate the selection panel
            selecting = false;
            selectionPanel.SetActive(false);
        }
		// Else the selection is for affection
        else
        {
			// Say we have reached the end of the file
            GlobalVar.fileEnd = true;
			// Move to the next game event based on selection
            GlobalVar.gameEvent += "C";
			// Say we are no longer selecting and de-activate the selection panel
            selecting = false;
            selectionPanel.SetActive(false);
        }
    }
}
