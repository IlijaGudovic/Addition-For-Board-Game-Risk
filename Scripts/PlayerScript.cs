using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    public int playerInt;
    public string Name;
    public int teritoris, tanks, tanksKilled , incomingTanks , kontinetnGrow;
    public int colorInt;

    public GameObject thisPlayerOnList;

    public void changeStats()
    {

        if (thisPlayerOnList != null)
        {
            thisPlayerOnList.GetComponent<PlayerList>().changeValues(Name, teritoris, tanks, teritoris / 3 + kontinetnGrow);
        }

    }

}
