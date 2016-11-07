using UnityEngine;
using System.Collections;

public class NPCRelationships : MonoBehaviour {

    // Relationships
    public static float[] relationships;
    public static float[] talkedToday;
    public static float[] convosHad;

    void Start () {
        relationships = new float[GlobalVar.npcTotal];
        talkedToday = new float[GlobalVar.npcTotal];
        convosHad = new float[GlobalVar.npcTotal];

        // We're setting up relationship values here
        // First int is NPC ID and Second int is exact relationship value (Higher = better relation)
        // 0 = Maisie
        relationships[0] = PlayerPrefs.GetFloat("MaisieAffection");
        // 1 = Serena
        relationships[1] = PlayerPrefs.GetFloat("SerenaAffection");
        // 2 = Lucille
        relationships[2] = PlayerPrefs.GetFloat("LucilleAffection");

        // Then we want to set the status for whether an NPC has been spoken to today
        // 0 means no 1 means yes
        talkedToday[0] = 0.0f;
        talkedToday[1] = 0.0f;
        talkedToday[2] = 0.0f;

        // Next we want to set the number of affection convos we've had
        convosHad[0] = PlayerPrefs.GetFloat("MaisieConvo");
        convosHad[1] = PlayerPrefs.GetFloat("SerenaConvo");
        convosHad[2] = PlayerPrefs.GetFloat("LucilleConvo");
    }

    void Update()
    {
        // Just use this to check we don't hit introduction to character again
        for(int i = 0; i < GlobalVar.npcTotal; i++)
        {
            if (relationships[i] <= 0.0f && convosHad[i] > 0.0f)
            {
                relationships[i] = 1.0f;
            }
        }
    }

    // Return NPC name
    public static string returnName(int i)
    {
        if (i == 0)
            return "Maisie";
        else if (i == 1)
            return "Serena";
        else if (i == 2)
            return "Lucille";
        else
            return "Not found";
    }

    // This raises Maisie's affection depending on how many times you make her deliveries
    public static void deliveryAffection()
    {
        if (GlobalVar.deliveriesMade % 5 == 0)
        {
            relationships[0] += 15.0f;
        }
    }
	
}
