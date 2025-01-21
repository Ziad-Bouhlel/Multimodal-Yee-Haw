using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelector : MonoBehaviour
{
    private Renderer cubeRenderer;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        cubeRenderer.material.color = Color.green;
        Debug.Log($"Cube sélectionné : {gameObject.name}");
    }
}
