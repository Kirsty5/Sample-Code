using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpMenu : MonoBehaviour {
    public GameObject panel;
    public Text timeText;

	void Update () {
        if (GlobalVar.isInteracting == false)
        {
            // Find if Escape has been pressed
            bool pressed = Input.GetKeyDown(KeyCode.Escape);
            bool click = Input.GetMouseButtonDown(1);
            // If esc has been pressed...
            if (pressed)
            {
                // If the menu is already up close it and it's sub panels
                if (panel.gameObject.active)
                {
                    panel.SetActive(false);
                    InventoryMenu.activateInven = false;
                    RelationshipMenu.activateRship = false;
                    OptionsMenu.activateOptions = false;
                    GlobalVar.isPaused = false;
                }
                else // Otherwise show the menu
                {
                    panel.SetActive(true);
                    GlobalVar.isPaused = true;
                }
            }
		
		// Close the menu if left click has been pressed and menu is active
            if(click && panel.active == true)
            {
                panel.SetActive(false);
                InventoryMenu.activateInven = false;
                RelationshipMenu.activateRship = false;
                OptionsMenu.activateOptions = false;
                GlobalVar.isPaused = false;
            }

		// Change the text of the time here
            timeText.text = "Month : " + GlobalVar.month + "\n" +
                            "Day : " + GlobalVar.day + ", " + TimeManagement.getDay() + "\n" + 
                            "Time : " + GlobalVar.hour.ToString("00") + ":" + Mathf.Round(GlobalVar.minute).ToString("00");
        }
	}
}
