using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TimeSelector : MonoBehaviour
{
    public Dropdown startTimeDropdown;
    public Dropdown endTimeDropdown;
    public Dropdown roomDropdown;
    public Text warningMessage;
    public Text listStudents;

    private int startTime;
    private int endTime;

    private List<Suspect> listOfSuspects = new List<Suspect>();
    private List<ObjectOfInterest> listOfObjects = new List<ObjectOfInterest>();
    private List<string> roomList = new List<string>();

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
                    new Suspect(tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], tmp[6], tmp[7], int.Parse(tmp[8]), tmp[9], tmp[10])
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
        roomDropdown.ClearOptions();
        roomDropdown.AddOptions(roomList);
    }

    public List<string> GetPeopleAndObjectsInRoom(string room, int startTime, int endTime)
    {
        List<string> results = new List<string>();

        foreach (var suspect in listOfSuspects)
        {
            for (int time = startTime; time < endTime; time++)
            {
                string location = suspect.queryTime(time);
                if (location == room && !results.Contains(suspect.getName()))
                {
                    results.Add(suspect.getName());
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
        string selectedRoom = roomDropdown.options[roomDropdown.value].text;
        List<string> results = GetPeopleAndObjectsInRoom(selectedRoom, startTime + 13, endTime + 13);

        if (results.Count > 0)
        {
            listStudents.text = $"Résultats pour la salle {selectedRoom} entre {startTime + 13}h et {endTime + 13}h :\n";
            listStudents.text += string.Join(", ", results);
        }
        else
        {
            listStudents.text = $"Aucun suspect ou objet trouvé dans la salle {selectedRoom} pendant cette plage horaire.";
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
