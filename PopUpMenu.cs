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
            // If it has been pressed...
            if (pressed)
            {
                // Switch to show menu depending on current active state
                if (panel.gameObject.active)
                {
                    panel.SetActive(false);
                    InventoryMenu.activateInven = false;
                    RelationshipMenu.activateRship = false;
                    OptionsMenu.activateOptions = false;
                    GlobalVar.isPaused = false;
                }
                else
                {
                    panel.SetActive(true);
                    GlobalVar.isPaused = true;
                }
            }

            if(click && panel.active == true)
            {
                panel.SetActive(false);
                InventoryMenu.activateInven = false;
                RelationshipMenu.activateRship = false;
                OptionsMenu.activateOptions = false;
                GlobalVar.isPaused = false;
            }

            timeText.text = "Month : " + GlobalVar.month + "\n" +
                            "Day : " + GlobalVar.day + ", " + TimeManagement.getDay() + "\n" + 
                            "Time : " + GlobalVar.hour.ToString("00") + ":" + Mathf.Round(GlobalVar.minute).ToString("00");
        }
	}
}
