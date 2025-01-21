using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TimeSelector : MonoBehaviour
{
    // Singleton Instance
    public static TimeSelector Instance { get; private set; }

    //public Dropdown startTimeDropdown;
    //public Dropdown endTimeDropdown;
    public Dropdown speDropdown;
    public Dropdown yearDropdown;
    public Dropdown transportDropdown;

    public Text warningMessage;
    public Text listStudents;

    private string roomSelect;

    private int startTime;
    private int endTime;

    private List<Suspect> listOfSuspects = new List<Suspect>();
    private List<ObjectOfInterest> listOfObjects = new List<ObjectOfInterest>();
    private List<string> roomList = new List<string>();
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
        /*
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
        */
        // Load data
        LoadSuspects();
        LoadObjects();
        LoadRooms();
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

    private void LoadRooms()
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

    public List<string> GetPeopleAndObjects(string room, int startTime, int endTime, string selectedSpe, string selectedYear, string selectedTransport)
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

        foreach (var obj in listOfObjects)
        {
            if (obj.oSourceLoc == room &&
                int.TryParse(obj.oSellTime.Replace("h", ""), out int sellTime) &&
                sellTime >= startTime && sellTime < endTime)
            {
                results.Add(obj.oName);
            }
        }

        return results;
    }

    public List<string> GetPeopleAndObjectsv2(string selectedSpe, string selectedYear, string selectedTransport)
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
        string selectedRoom = "ee";
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


}