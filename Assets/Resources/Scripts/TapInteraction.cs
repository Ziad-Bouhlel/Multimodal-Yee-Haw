using UnityEngine;

public class TapInteraction : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color originalColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
    }

    void Update()
    {
        // D�tection pour les clics de souris (PC)
        if (Input.GetMouseButtonDown(0))
        {
            DetectTapOrClick(Input.mousePosition);
        }

        // D�tection pour les tapes (mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DetectTapOrClick(Input.GetTouch(0).position);
        }
    }

    void DetectTapOrClick(Vector3 inputPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            // Changer la couleur
            objectRenderer.material.color = Random.ColorHSV();

            RoomSelectionManager roomSelectionManager = FindObjectOfType<RoomSelectionManager>();
            if (roomSelectionManager != null)
            {
                roomSelectionManager.SelectRoom(gameObject);
            }

            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif

            AudioSource audio = GetComponent<AudioSource>();
            if (audio != null) audio.Play();
        }
    }

}