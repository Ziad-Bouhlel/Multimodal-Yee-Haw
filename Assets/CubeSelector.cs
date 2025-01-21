using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelector : MonoBehaviour
{
    private Renderer cubeRenderer;
    private bool isSelected = false;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        cubeRenderer.material.color = Color.red;

    }

    private void OnMouseDown()
    {

        

       // Si le cube est sélectionné, on le désélectionne
        if (isSelected==true)
        {
            isSelected = false;
            cubeRenderer.material.color = Color.red;
            SelectionManager.Instance.DeselectRoom(this.transform);
            Debug.Log($"Cube désélectionné : {gameObject.name}");
        }
        // Sinon, on essaye de le sélectionner
        else
        {
            if (SelectionManager.Instance.SelectRoom(this.transform))
            {
                isSelected = true;
                cubeRenderer.material.color = Color.yellow;
                Debug.Log($"Cube sélectionné : {gameObject.name}");
                TimeSelector.OnSelect(gameObject.name);
                Debug.Log($"Cube sélectionné : {gameObject.name}");
                
            }
            else
            {
                Debug.Log("Impossible de sélectionner plus de 2 salles.");
            }
        }
    }
}
