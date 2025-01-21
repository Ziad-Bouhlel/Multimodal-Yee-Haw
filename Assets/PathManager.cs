using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;

    private List<Transform> trackedPositions = new List<Transform>();

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

    // Ajouter une position suivie (par exemple, un cube QR détecté)
    public void AddPosition(Transform newPosition)
    {
        if (!trackedPositions.Contains(newPosition))
        {
            trackedPositions.Add(newPosition);
            Debug.Log($"Position ajoutée : {newPosition.name}");
        }
    }

    // Supprimer une position si le QR code n'est plus visible
    public void RemovePosition(Transform positionToRemove)
    {
        if (trackedPositions.Contains(positionToRemove))
        {
            trackedPositions.Remove(positionToRemove);
            Debug.Log($"Position supprimée : {positionToRemove.name}");
        }
    }

    // Mettre à jour une position existante (si un QR code bouge)
    public void UpdatePathPosition()
    {
        foreach (var position in trackedPositions)
        {
            Debug.Log($"Position actuelle : {position.name} à {position.position}");
        }
    }

    // Exemple pour calculer un chemin entre deux positions (ou distance)
    public float CalculateDistance(Transform start, Transform end)
    {
        if (start != null && end != null)
        {
            float distance = Vector3.Distance(start.position, end.position);
            Debug.Log($"Distance entre {start.name} et {end.name} : {distance}m");
            return distance;
        }

        Debug.LogWarning("Une des positions est nulle !");
        return -1;
    }
}
