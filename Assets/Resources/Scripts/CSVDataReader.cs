using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSVDataReader : MonoBehaviour
{
    TextAsset suspectsFile;
    TextAsset objectsFile;

    List<Suspect> listOfSuspects = new List<Suspect>();
    List<ObjectOfInterest> listOfObjects = new List<ObjectOfInterest>();
    public Canvas myCanvas;

    // Start is called before the first frame update
    void Start()
    {
        suspectsFile = Resources.Load<TextAsset>("students");
        objectsFile = Resources.Load<TextAsset>("objects");

        string[] objects = objectsFile.ToString().Split('\n');
        string[] suspects = suspectsFile.ToString().Split('\n');

        for (int i = 1; i < objects.Length - 1; i++)
        {
            string[] tmp = objects[i].Split(',');
            print(tmp.Length);
            listOfObjects.Add(new ObjectOfInterest(tmp[0], tmp[1], tmp[2]));
        }

        for (int i = 1; i < suspects.Length - 1; i++)
        {
            string[] tmp = suspects[i].Split(',');
            listOfSuspects.Add(
                new Suspect(tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], tmp[6], tmp[7], int.Parse(tmp[8]), tmp[9], tmp[10], tmp[11], tmp[12])
            );
        }

        myCanvas.GetComponentInChildren<Text>().text = listOfSuspects[0].getName() + " was at " + listOfSuspects[0].queryTime(14) + " at 14h.";

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<string> GetPeopleAndObjectsInRange(int startTime, int endTime)
    {
        List<string> results = new List<string>();

        // Filtrer les étudiants en fonction de l'heure
        foreach (var suspect in listOfSuspects)
        {
            Debug.Log("Checking " + suspect.getName());
            for (int time = startTime; time <= endTime; time++)
            {
                string location = suspect.queryTime(time);  // Récupère la localisation pour chaque heure
                if (location != "time not existant" && !results.Contains(suspect.getName()))
                {
                    results.Add(suspect.getName());  // Ajoute le nom de l'étudiant si une présence est trouvée
                    break;  // Une fois trouvé, on arrête de vérifier pour ce suspect
                }
            }
        }

        // Filtrer les objets en fonction de l'heure
        foreach (var obj in listOfObjects)
        {
            // Vérifiez si l'objet est dans la plage horaire et ajout du nom
            if (int.TryParse(obj.oSellTime, out int sellTime) && sellTime >= startTime && sellTime <= endTime)
            {
                results.Add(obj.oName);
            }
        }

        return results;
    }


    public void DisplayResults(int startTime, int endTime)
    {
        List<string> results = GetPeopleAndObjectsInRange(startTime, endTime);

        // Trouve le texte pour afficher les résultats
        Text resultText = myCanvas.GetComponentInChildren<Text>();
        resultText.text = string.Join("\n", results);
        Debug.Log("Results displayed: \n" + string.Join("\n", results));
    }


}