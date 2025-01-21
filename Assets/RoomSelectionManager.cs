using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomSelectionManager : MonoBehaviour
{
    public TextMeshProUGUI roomSelectionText; // Affichage UI pour les salles sélectionnées
    public TextMeshProUGUI travelTimeText;   // Affichage du temps de trajet

    private GameObject selectedRoom1;
    private GameObject selectedRoom2;

    // Méthode appelée lorsqu'on clique sur une salle
    public void SelectRoom(GameObject room)
    {
        if (selectedRoom1 == null)
        {
            selectedRoom1 = room;
            UpdateUI();
            Debug.Log($"First room selected: {room.name}");
        }
        else if (selectedRoom2 == null && room != selectedRoom1)
        {
            selectedRoom2 = room;
            UpdateUI();
            CalculateTravelTime();
            Debug.Log($"Second room selected: {room.name}");
        }
        else
        {
            // Réinitialiser si on clique une 3e fois
            selectedRoom1 = room;
            selectedRoom2 = null;
            UpdateUI();
            Debug.Log($"Reset selection. New first room: {room.name}");
        }
    }

    // Calculer la distance et le temps de trajet entre les deux salles
    private void CalculateTravelTime()
    {
        if (selectedRoom1 != null && selectedRoom2 != null)
        {
            float distance = Vector3.Distance(selectedRoom1.transform.position, selectedRoom2.transform.position);
            float travelTime = distance * 10; // Exemple de conversion distance -> temps

            travelTimeText.text = $"Travel time: {travelTime:F2} seconds";
            Debug.Log($"Distance: {distance}, Travel time: {travelTime}");
        }
    }

    // Mettre à jour l'UI
    private void UpdateUI()
    {
        if (selectedRoom1 != null && selectedRoom2 == null)
        {
            roomSelectionText.text = $"Selected: {selectedRoom1.name}";
        }
        else if (selectedRoom1 != null && selectedRoom2 != null)
        {
            roomSelectionText.text = $"Selected: {selectedRoom1.name} -> {selectedRoom2.name}";
        }
        else
        {
            roomSelectionText.text = "Select a room...";
        }
    }
}
