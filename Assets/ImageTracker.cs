using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject lineRendererPrefab; // Prefab contenant un LineRenderer pour dessiner l'encadré.

    private Dictionary<string, GameObject> activeFrames = new Dictionary<string, GameObject>(); // Associe les QR codes aux encadrés.
    private GameObject selectedFrame = null;

    void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Ajouter un cadre autour des QR codes détectés
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log("Nom : "+trackedImage.name);
            var frame = Instantiate(lineRendererPrefab, trackedImage.transform);
            frame.transform.localPosition = Vector3.zero; // Centrer autour du QR code
            frame.transform.localScale = new Vector3(trackedImage.size.x, trackedImage.size.y, 1); // Ajuster à la taille du QR code
            frame.name = "Frame_" + trackedImage.referenceImage.name;

            activeFrames[trackedImage.referenceImage.name] = frame;
        }

        // Mettre à jour la position des cadres si les QR codes bougent
        foreach (var trackedImage in eventArgs.updated)
        {
            if (activeFrames.ContainsKey(trackedImage.referenceImage.name))
            {
                var frame = activeFrames[trackedImage.referenceImage.name];
                frame.SetActive(trackedImage.trackingState == TrackingState.Tracking);

                // Réajuster la taille si nécessaire
                frame.transform.localScale = new Vector3(trackedImage.size.x, trackedImage.size.y, 1);
            }
        }
    }

    void Update()
    {
        // Gérer les clics pour changer la couleur de l'encadré
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var clickedFrame = hit.transform.gameObject;

                if (clickedFrame.name.StartsWith("Frame_"))
                {
                    if (selectedFrame != null)
                    {
                        // Réinitialiser la couleur de l'encadré précédent
                        var previousRenderer = selectedFrame.GetComponent<LineRenderer>();
                        previousRenderer.startColor = Color.white;
                        previousRenderer.endColor = Color.white;
                    }

                    // Mettre à jour le nouvel encadré sélectionné
                    selectedFrame = clickedFrame;

                    var lineRenderer = selectedFrame.GetComponent<LineRenderer>();
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;

                    Debug.Log($"QR Code sélectionné : {clickedFrame.name.Replace("Frame_", "")}");
                }
            }
        }
    }
}
