using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    
   public static SelectionManager Instance;

    [SerializeField] private TextMeshProUGUI feedbackText; // Texte pour afficher le temps/distance
    private List<Transform> selectedRooms = new List<Transform>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Sélectionne une salle
    public bool SelectRoom(Transform room)
    {
        if (selectedRooms.Count<2  && !selectedRooms.Contains(room)) // Limiter à 2 sélections
        {
            selectedRooms.Add(room);
            Debug.Log($"Salle ajoutée : {room.name}");

            if (selectedRooms.Count == 2)
            {
                CalculateTimeOrDistance();
            }

            return true; // Sélection réussie
        }
        else
        {
            Debug.Log("Vous ne pouvez sélectionner que 2 salles à la fois.");
            return false; // Sélection échouée
        }
    }

    // Désélectionne une salle
    public void DeselectRoom(Transform room)
    {
        if (selectedRooms.Contains(room))
        {
            selectedRooms.Remove(room);
            Debug.Log($"Salle retirée : {room.name}");
            
            if (selectedRooms.Count < 2)
            {
                feedbackText.text = "";
            }
        }
    }

    // Calcule et affiche le temps ou la distance entre les deux salles
    private void CalculateTimeOrDistance()
    {
        if (selectedRooms.Count == 2)
        {
            Transform room1 = selectedRooms[0];
            Transform room2 = selectedRooms[1];

            // Calculer la distance entre les deux salles
            float distance = Vector3.Distance(room1.position, room2.position);
            float estimatedTime = distance * 10; // Exemple : 2 secondes par unité de distance

            // Afficher le résultat
            feedbackText.text = $"Temps estimé : {estimatedTime:F1} minutes";
            Debug.Log($"Distance entre {room1.name} et {room2.name} : {distance:F1}");
        }
    }
}
