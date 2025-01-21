using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Suspect
{
    string sName, sGender, sHairColor, sTransport, sClothing, sYear, sSpe;
    string[] locations = new string[5];
    int sHeight;

    // Start is called before the first frame update
    public Suspect(
        string name,
        string gender,
        string onePM,
        string twoPM,
        string threePM,
        string fourPM,
        string fivePM,
        string hairColor,
        int height,
        string transport,
        string clothing,
        string year,
        string spe)
    {
        sName = name;
        sGender = gender;
        locations[0] = onePM;
        locations[1] = twoPM;
        locations[2] = threePM;
        locations[3] = fourPM;
        locations[4] = fivePM;
        sHairColor = hairColor;
        sHeight = height;
        sTransport = transport;
        sClothing = clothing;
        sYear = year;
        sSpe = spe;
    }

    public string getName()
    {
        return sName;
    }
    public string getTransport()
    {
        return sTransport;
    }

    public string getClothing()
    {
        return sClothing;
    }

    public string getYear()
    {
        return sYear;
    }

    public string getSpe()
    {
        return sSpe;
    }

    public string getHair()
    {
        return sHairColor;
    }
    public int getHeight()
    {
        return sHeight;
    }

    public string getGender()
    {
        return sGender;
    }

    // function to return the location of the person at a specific time
    public string queryTime(int time)
    {
        if (time <= 17 && time >= 13)
        {
            return locations[time - 13];
        }
        else return "time not existant";
    }

}