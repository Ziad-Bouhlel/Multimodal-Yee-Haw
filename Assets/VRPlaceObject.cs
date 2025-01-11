using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class VRPlaceObjects : MonoBehaviour
{
    private Camera mainCamera;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    public Material hoverMaterial;
    private Renderer targetRenderer;

    public string targetTag = "Floor";
    public TMP_Dropdown prefabDropdown;
    private List<GameObject> availablePrefabs = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;

        // Création du matériau de survol si non assigné
        if (hoverMaterial == null)
        {
            hoverMaterial = new Material(Shader.Find("Standard"));
            hoverMaterial.color = Color.white;
        }

        LoadAllPrefabs();
        PopulateDropdown();
    }

    void Update()
    {
        HandleCameraRaycast();
        HandlePlacement();
    }

    private void HandleCameraRaycast()
    {
        // Utilise la position et la direction de la caméra pour le raycast
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && IsFloorTile(hit.collider.gameObject))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null)
            {
                if (renderer != targetRenderer)
                {
                    ResetPreviousObject();

                    targetRenderer = renderer;

                    // Sauvegarde le matériau original si pas encore sauvegardé
                    if (!originalMaterials.ContainsKey(renderer))
                    {
                        originalMaterials[renderer] = renderer.material;
                    }

                    // Applique le matériau de survol
                    renderer.material = hoverMaterial;
                }
            }
        }
        else
        {
            ResetPreviousObject();
        }
    }

    private void HandlePlacement()
    {
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (targetRenderer != null)
            {
                PlacePrefabOnTile(targetRenderer.gameObject);
            }
        }
    }

    private void ResetPreviousObject()
    {
        if (targetRenderer != null)
        {
            // Vérifie si un matériau original a été sauvegardé
            if (originalMaterials.ContainsKey(targetRenderer))
            {
                targetRenderer.material = originalMaterials[targetRenderer];
            }

            // Supprime le renderer du dictionnaire
            originalMaterials.Remove(targetRenderer);

            targetRenderer = null;
        }
    }

    private void PlacePrefabOnTile(GameObject targetObject)
    {
        if (prefabDropdown != null && prefabDropdown.value < availablePrefabs.Count)
        {
            GameObject selectedPrefab = availablePrefabs[prefabDropdown.value];
            if (selectedPrefab != null)
            {
                // Calcule le centre du tile
                Vector3 tileCenter = targetObject.GetComponent<Renderer>().bounds.center;

                // Place le prefab au centre du tile
                Instantiate(selectedPrefab, tileCenter, Quaternion.identity);

                // Optionnel : Jouer un son ou effet pour confirmer le placement
                Debug.Log("Objet placé au centre : " + tileCenter);
            }
        }
        else
        {
            Debug.LogWarning("Aucun prefab sélectionné ou valeur du dropdown hors limites.");
        }
    }

    private void LoadAllPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");
        availablePrefabs.Clear();
        foreach (GameObject prefab in prefabs)
        {
            availablePrefabs.Add(prefab);
        }
    }

    private void PopulateDropdown()
    {
        if (prefabDropdown != null)
        {
            prefabDropdown.ClearOptions();
            List<string> options = new List<string>();

            foreach (GameObject prefab in availablePrefabs)
            {
                options.Add(prefab.name);
            }

            prefabDropdown.AddOptions(options);
        }
        else
        {
            Debug.LogWarning("Le dropdown des prefabs n'est pas assigné dans l'inspecteur.");
        }
    }

    private bool IsFloorTile(GameObject gameObject)
    {
        return gameObject.CompareTag(targetTag);
    }
}
