using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour
{

    public Image playerColor;
    public Text nameText;
    public Text territoriesText;
    public Text tenksText;
    public Text incomingTanksText;


    public void changeValues(string Name, int NumberOfTeritoris, int NumberTanks, int IncomingTanks)
    {

        nameText.text = Name;
        territoriesText.text = NumberOfTeritoris.ToString();
        tenksText.text = NumberTanks.ToString();
        incomingTanksText.text = ( "+ " + IncomingTanks.ToString());


    }

}
