using UnityEngine;

public class GazeTrigger : MonoBehaviour
{
    public Camera mainCamera; // Caméra publique sélectionnable
    public GameObject targetObject; // Objet à faire apparaître
    public float gazeDuration = 2f; // Temps nécessaire de fixation
    public float distanceFromCube = 1f; // Distance à laquelle l'objet apparaît devant le cube
    private float gazeTimer = 0f;

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Aucune caméra assignée au script !");
        }
        targetObject.SetActive(false); // Désactiver l'objet au démarrage
    }

    void Update()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Rayon au centre de l'écran
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject) // Vérifie si le cube est visé
        {
            gazeTimer += Time.deltaTime;

            if (gazeTimer >= gazeDuration && !targetObject.activeSelf)
            {
                // Positionner l'objet devant le cube
                Vector3 forwardPosition = transform.position + transform.forward * distanceFromCube;
                targetObject.transform.position = forwardPosition;
                targetObject.SetActive(true); // Faire apparaître l'objet
            }
        }
        else
        {
            gazeTimer = 0f; // Réinitialiser si la fixation est perdue
            targetObject.SetActive(false);
        }
    }
}