using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRManipulateObecjts : MonoBehaviour
{
    private Camera mainCamera;
    public Material hoverMaterial;
    public Material selectedMaterial;
    private Material originalMaterial;
    private Renderer currentHoverRenderer;
    private Renderer selectedRenderer;

    private bool isMovingObject = false; // Indique si un objet est en cours de déplacement
    private VRTranslate vrTranslateScript; // Référence au script VRTranslate

    private Vector3 originalPosition; // Position d'origine de l'objet
    private Quaternion originalRotation; // Rotation d'origine de l'objet
    private Vector3 originalScale; // Échelle d'origine de l'objet

    private void Start()
    {
        mainCamera = Camera.main;

        // Vérifie que les matériaux sont bien assignés
        if (hoverMaterial == null || selectedMaterial == null)
        {
            Debug.LogError("HoverMaterial ou SelectedMaterial n'est pas assigné dans l'inspecteur.");
        }

        // Trouve le script VRTranslate dans la scène
        vrTranslateScript = FindObjectOfType<VRTranslate>();
        if (vrTranslateScript == null)
        {
            Debug.LogError("Le script VRTranslate n'a pas été trouvé dans la scène.");
        }
    }

    private void Update()
    {
        if (!isMovingObject)
        {
            HandleHover();
        }
        HandleSelect();

        if (isMovingObject)
        {
            MoveSelectedObject();
            RotateSelectedObject();
            ScaleSelectedObject();
        }
    }

    private void HandleHover()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null)
            {
                if (renderer != currentHoverRenderer)
                {
                    ResetHover(); // Réinitialise l'ancien hover

                    currentHoverRenderer = renderer;
                    originalMaterial = renderer.material;

                    // Applique le matériau de survol
                    renderer.material = hoverMaterial;
                }
            }
        }
        else
        {
            ResetHover();
        }
    }

    private void HandleSelect()
    {
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (isMovingObject)
            {
                // Arrête de déplacer l'objet
                isMovingObject = false;

                if (vrTranslateScript != null)
                {
                    vrTranslateScript.enabled = true; // Réactive le script VRTranslate
                }
            }
            else if (currentHoverRenderer != null)
            {
                if (selectedRenderer != null)
                {
                    // Réinitialise le matériau de l'objet précédemment sélectionné
                    selectedRenderer.material = originalMaterial;
                }

                // Applique le matériau sélectionné
                selectedRenderer = currentHoverRenderer;
                selectedRenderer.material = selectedMaterial;

                // Enregistre la position, la rotation et la taille d'origine
                originalPosition = selectedRenderer.transform.position;
                originalRotation = selectedRenderer.transform.rotation;
                originalScale = selectedRenderer.transform.localScale;

                // Commence à déplacer l'objet
                isMovingObject = true;

                if (vrTranslateScript != null)
                {
                    vrTranslateScript.enabled = false; // Désactive le script VRTranslate
                }
            }
        }
    }

private void MoveSelectedObject()
{
    if (selectedRenderer != null && Gamepad.current != null)
    {
        Vector2 joystickInput = Gamepad.current.leftStick.ReadValue();

        // Obtient la direction de la caméra en ignorant la composante verticale
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0; // Ignore la composante Y (verticale)
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0; // Ignore la composante Y (verticale)
        right.Normalize();

        // Calcule le mouvement dans le référentiel de la caméra
        Vector3 movement = (forward * joystickInput.y + right * joystickInput.x) * Time.deltaTime * 2.0f; // Facteur de vitesse

        // Applique le mouvement à l'objet sélectionné
        selectedRenderer.transform.position += movement;
    }
}

    private void RotateSelectedObject()
    {
        if (selectedRenderer != null && Gamepad.current != null)
        {
            Vector2 rotationInput = Gamepad.current.rightStick.ReadValue();

            // Tourne l'objet autour de l'axe Y (gauche-droite) et de l'axe X (haut-bas)
            Vector3 rotation = new Vector3(-rotationInput.y, rotationInput.x, 0) * Time.deltaTime * 100.0f; // Facteur de vitesse
            selectedRenderer.transform.Rotate(rotation, Space.World);
        }
    }

      private void ScaleSelectedObject()
{
    if (selectedRenderer != null && Gamepad.current != null)
    {
        if (Gamepad.current.buttonWest.wasPressedThisFrame) // Carré
        {
            selectedRenderer.transform.localScale *= 0.9f; // Diminue la taille
        }
        else if (Gamepad.current.buttonNorth.wasPressedThisFrame) // Triangle
        {
            selectedRenderer.transform.localScale *= 1.1f; // Augmente la taille
        }
        else if (Gamepad.current.buttonEast.wasPressedThisFrame) // Rond
        {
            // Réinitialise la taille, la position et la rotation
            selectedRenderer.transform.position = originalPosition;
            selectedRenderer.transform.rotation = originalRotation;
            selectedRenderer.transform.localScale = originalScale;

            // Désélectionne l'objet lorsque le bouton Rond est appuyé
            selectedRenderer.material = originalMaterial;
            selectedRenderer = null;
            isMovingObject = false;

            // Réactive le script VRTranslate
            if (vrTranslateScript != null)
            {
                vrTranslateScript.enabled = true;
            }
        }
    }
}

    public void ResetHover()
    {
        if (currentHoverRenderer != null)
        {
            currentHoverRenderer.material = originalMaterial;
            currentHoverRenderer = null;
        }
    }
}


