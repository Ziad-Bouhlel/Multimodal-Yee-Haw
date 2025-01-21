using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class TimeSelector : MonoBehaviour
{
    // Singleton Instance
    public static TimeSelector Instance { get; private set; }

    public Dropdown startTimeDropdown;
    public Dropdown endTimeDropdown;
    public Dropdown speDropdown;
    public Dropdown yearDropdown;
    public Dropdown transportDropdown;

    public Text warningMessage;
    public Text listStudents;
    public Text menu;


    private static string selectedRoom;
    public Text gotRoom;

    private static int startTime;
    private static int endTime;

    private static List<Suspect> listOfSuspects = new List<Suspect>();
    private static List<ObjectOfInterest> listOfObjects = new List<ObjectOfInterest>();
    private List<string> speList = new List<string>();
    private List<string> yearList = new List<string>();
    private List<string> transportList = new List<string>();

    private void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this GameObject across scenes if necessary
    }

    private void Start()
    {
        
        // Populate dropdowns with times
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
        LoadSpe();
        LoadYear();
        LoadTransport();
    }

    private void LoadSuspects()
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

    private void LoadObjects()
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

    private void LoadSpe()
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

        speList = new List<string>(uniqueSpe);
        speDropdown.ClearOptions();
        speDropdown.AddOptions(speList);
    }

    private void LoadYear()
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

        yearList = new List<string>(uniqueYear);
        yearDropdown.ClearOptions();
        yearDropdown.AddOptions(yearList);
    }

    private void LoadTransport()
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

        transportList = new List<string>(uniqueTransport);
        transportDropdown.ClearOptions();
        transportDropdown.AddOptions(transportList);
    }

    public static List<string> GetPeopleAndObjectsInRoom(string room, int startTime, int endTime)
    {
        List<string> results = new List<string>();
        Debug.Log("times " + startTime + " / " + endTime);
        foreach (var suspect in listOfSuspects)
        {
            for (int time = startTime; time < endTime; time++)
            {
                string location = suspect.queryTime(time);
                Debug.Log($"given={room.Trim().ToLower()}\n" +
                 $"got = {location.Trim()} {suspect.getSpe()}\n" +
                 $"bool = {location.Trim().ToLower() == room.Trim().ToLower()}");
                if (location.Trim().ToLower() == room.Trim() && !results.Contains(suspect.getName()))
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

    public static List<string> GetPeopleAndObjectsv2(string selectedSpe, string selectedYear, string selectedTransport)
    {
        List<string> results = new List<string>();

        foreach (var suspect in listOfSuspects)
        {
            print(suspect.getName());

            if (!results.Contains(suspect.getName()) &&
                    suspect.getSpe().Trim() == selectedSpe.Trim() &&
                    suspect.getYear().Trim() == selectedYear.Trim() &&
                    suspect.getTransport().Trim() == selectedTransport.Trim())
            {
                results.Add(suspect.getName() + " / " + suspect.getGender() + " / " + suspect.getHair() + " / " + suspect.getHeight() + " cm / " + suspect.getClothing());
            }
        }

        return results;
    }

    public void DisplayResults()
    {
        string selectedSpe = speDropdown.options[speDropdown.value].text;
        string selectedYear = yearDropdown.options[yearDropdown.value].text;
        string selectedTransport = transportDropdown.options[transportDropdown.value].text;

        List<string> results = GetPeopleAndObjectsv2(selectedSpe, selectedYear, selectedTransport);

        if (results.Count > 0)
        {
            listStudents.text = $"Résultats pour les étudiants de la spécialité {selectedSpe} de l'année {selectedYear} utilisant le mode de transport : {selectedTransport} :\n";
            listStudents.text += string.Join("\n", results);
        }
        else
        {
            listStudents.text = $"Aucun résultat pour les étudiants de la spécialité {selectedSpe} de l'année {selectedYear} utilisant le mode de transport : {selectedTransport}";
        }
    }

    public void DisplayResultsMenu()
    {
        List<string> results = GetPeopleAndObjectsInRoom(selectedRoom, startTime + 13, endTime + 13);
        if (results.Count > 0)
        {
            menu.text = $"Résultats pour la salle {selectedRoom} entre {startTime + 13}h et {endTime + 13}h :\n";
            menu.text += string.Join(", ", results);
        }
        else
        {
            menu.text = $"Aucun suspect ou objet trouvé dans la salle {selectedRoom} pendant cette plage horaire.";
        }
    }
    public void OnSubmit()
    {
        warningMessage.text = "";
        DisplayResults();

        //startTime = startTimeDropdown.value;
        //endTime = endTimeDropdown.value;
        /*

        if (startTime >= endTime)
        {
            warningMessage.text = "Erreur : L'heure de début doit être inférieure à l'heure de fin.";
            warningMessage.color = Color.red;
        }
        else
        {
            warningMessage.text = "";
            DisplayResults();
        }*/
    }

    public void setRoom(string roomName)
    {
        gotRoom.text = roomName;
    }
    public static void OnSelect(string room)
    {
        selectedRoom = room.Split("_")[1];
        Debug.Log("Room :" + selectedRoom);
            if (Instance != null)
            {
                Instance.setRoom(selectedRoom);
                startTime = Instance.startTimeDropdown.value;
                endTime = Instance.endTimeDropdown.value;
                if (startTime >= endTime)
                {
                    Instance.warningMessage.text = "Erreur : L'heure de début doit être inférieure à l'heure de fin.";
                    Instance.warningMessage.color = Color.red;
                }
                else
                {
                    Instance.warningMessage.text = "";
                    Instance.DisplayResultsMenu();
                }
        }
            else
            {
                Debug.LogError("TimeSelector instance is null. Ensure the script is properly initialized.");
            }
    }


}