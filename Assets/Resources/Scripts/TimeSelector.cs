using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TimeSelector : MonoBehaviour
{
    public Dropdown startTimeDropdown;
    public Dropdown endTimeDropdown;
    public Dropdown speDropdown;
    public Dropdown yearDropdown;
    public Dropdown transportDropdown;

    public Text warningMessage;
    public Text listStudents;
    public Text roomSelected;

    private int startTime;
    private int endTime;

    private List<Suspect> listOfSuspects = new List<Suspect>();
    private List<ObjectOfInterest> listOfObjects = new List<ObjectOfInterest>();
    private List<string> roomList = new List<string>();
    private List<string> speList = new List<string>();
    private List<string> yearList = new List<string>();
    private List<string> transportList = new List<string>();


    void Start()
    {
        List<string> times = new List<string>();
        for (int i = 13; i < 18; i++)
        {
            times.Add(i + "h");
        }

        startTimeDropdown.ClearOptions();
        startTimeDropdown.AddOptions(times);

        endTimeDropdown.ClearOptions();
        endTimeDropdown.AddOptions(times);

        LoadSuspects();
        LoadObjects();
        LoadRooms();
        LoadSpe();
        LoadYear();
        LoadTransport();
    }

    void LoadSuspects()
    {
        TextAsset suspectsFile = Resources.Load<TextAsset>("students");
        string[] lines = suspectsFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] tmp = lines[i].Split(',');
            if (tmp.Length >= 11)
            {
                listOfSuspects.Add(
                    new Suspect(tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], tmp[6], tmp[7], int.Parse(tmp[8]), tmp[9], tmp[10], tmp[11], tmp[12])
                );
            }
        }
    }

    void LoadObjects()
    {
        TextAsset objectsFile = Resources.Load<TextAsset>("objects");
        string[] lines = objectsFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] tmp = lines[i].Split(',');
            if (tmp.Length >= 3)
            {
                listOfObjects.Add(new ObjectOfInterest(tmp[0], tmp[1], tmp[2]));
            }
        }
    }

    void LoadRooms()
    {
        TextAsset suspectsFile = Resources.Load<TextAsset>("students");
        string[] lines = suspectsFile.text.Split('\n');

        HashSet<string> uniqueRooms = new HashSet<string>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length >= 6)
            {
                for (int j = 2; j <= 6; j++)
                {
                    string room = cols[j].Trim();
                    if (!string.IsNullOrEmpty(room))
                    {
                        uniqueRooms.Add(room);
                    }
                }
            }
        }

        roomList = new List<string>(uniqueRooms);
    }

    void LoadSpe()
    {
        TextAsset suspectsFile = Resources.Load<TextAsset>("students");
        string[] lines = suspectsFile.text.Split('\n');
        HashSet<string> uniqueSpe = new HashSet<string>();

        for (int i = 1; i < lines.Length; i++) 
        {
            string[] suspect = lines[i].Split(',');
            if (suspect.Length > 12)
            {
                string spe = suspect[12].Trim();
                if (!string.IsNullOrEmpty(spe))
                {
                    uniqueSpe.Add(spe);
                }
            }
        }
        print("Contenu de uniqueSpe : " + string.Join(", ", uniqueSpe));

        speList = new List<string>(uniqueSpe);
        speDropdown.ClearOptions();
        speDropdown.AddOptions(speList);
    }

    void LoadYear()
    {
        TextAsset suspectsFile = Resources.Load<TextAsset>("students");
        string[] lines = suspectsFile.text.Split('\n');
        HashSet<string> uniqueYear = new HashSet<string>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] suspect = lines[i].Split(',');
            if (suspect.Length > 12)
            {
                string year = suspect[11].Trim();
                if (!string.IsNullOrEmpty(year))
                {
                    uniqueYear.Add(year);
                }
            }
        }
        print("Contenu de uniqueSpe : " + string.Join(", ", uniqueYear));

        yearList = new List<string>(uniqueYear);
        yearDropdown.ClearOptions();
        yearDropdown.AddOptions(yearList);
    }

    void LoadTransport()
    {
        TextAsset suspectsFile = Resources.Load<TextAsset>("students");
        string[] lines = suspectsFile.text.Split('\n');
        HashSet<string> uniqueTransport = new HashSet<string>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] suspect = lines[i].Split(',');
            if (suspect.Length > 12)
            {
                string transport = suspect[9].Trim();
                if (!string.IsNullOrEmpty(transport))
                {
                    uniqueTransport.Add(transport);
                }
            }
        }
        print("Contenu de uniqueSpe : " + string.Join(", ", uniqueTransport));

        transportList = new List<string>(uniqueTransport);
        transportDropdown.ClearOptions();
        transportDropdown.AddOptions(transportList);
    }

    public List<string> GetPeopleAndObjects(string room, int startTime, int endTime , string selectedSpe,string selectedYear, string selectedTransport)
    {
        List<string> results = new List<string>();

        foreach (var suspect in listOfSuspects)
        {
            for (int time = startTime; time < endTime; time++)
            {
                string location = suspect.queryTime(time);
                print($"given={selectedSpe} {room}\n"+
                    $"got = {location} {suspect.getSpe()}\n"+
                    $"bool = {selectedSpe==suspect.getSpe()} / {location==room}");
                if (location.Trim() == room.Trim() && !results.Contains(suspect.getName()) && suspect.getSpe().Trim() == selectedSpe.Trim() && suspect.getYear().Trim()==selectedYear.Trim() && suspect.getTransport().Trim() == selectedTransport.Trim())
                {
                    results.Add(suspect.getName() + "/" + suspect.getClothing());
                    break;
                }
            }
        }

        foreach (var obj in listOfObjects)
        {
            if (obj.oSourceLoc == room && int.TryParse(obj.oSellTime.Replace("h", ""), out int sellTime) && sellTime >= startTime && sellTime < endTime)
            {
                results.Add(obj.oName);
            }
        }

        return results;
    }

    public void DisplayResults()
    {
        string selectedRoom = roomSelected.text;
        string selectedSpe = speDropdown.options[speDropdown.value].text;
        string selectedYear = yearDropdown.options[yearDropdown.value].text;
        string selectedTransport = transportDropdown.options[transportDropdown.value].text;

        List<string> results = GetPeopleAndObjects(selectedRoom, startTime + 13, endTime + 13 , selectedSpe , selectedYear , selectedTransport);

        if (results.Count > 0)
        {
            listStudents.text = $"Résultats pour la salle {selectedRoom} entre {startTime + 13}h et {endTime + 13}h :\n";
            listStudents.text += string.Join(", ", results);
        }
        else
        {
            listStudents.text = $"aucun pour la salle {selectedRoom} entre {startTime + 13}h et {endTime + 13}h + {selectedSpe}\n";
        }
    }

    public void OnSubmit()
    {
        startTime = startTimeDropdown.value;
        endTime = endTimeDropdown.value;

        if (startTime >= endTime)
        {
            warningMessage.text = "Erreur : L'heure de début doit être inférieure à l'heure de fin.";
            warningMessage.color = Color.red;
        }
        else
        {
            warningMessage.text = "";
            DisplayResults();
        }
    }
}
