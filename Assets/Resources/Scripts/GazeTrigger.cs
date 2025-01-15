using UnityEngine;

public class GazeTrigger : MonoBehaviour
{
    public Camera mainCamera; // Cam�ra publique s�lectionnable
    public GameObject targetObject; // Objet � faire appara�tre
    public float gazeDuration = 2f; // Temps n�cessaire de fixation
    public float distanceFromCube = 1f; // Distance � laquelle l'objet appara�t devant le cube
    private float gazeTimer = 0f;

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Aucune cam�ra assign�e au script !");
        }
        targetObject.SetActive(false); // D�sactiver l'objet au d�marrage
    }

    void Update()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Rayon au centre de l'�cran
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject) // V�rifie si le cube est vis�
        {
            gazeTimer += Time.deltaTime;

            if (gazeTimer >= gazeDuration && !targetObject.activeSelf)
            {
                // Positionner l'objet devant le cube
                Vector3 forwardPosition = transform.position + transform.forward * distanceFromCube;
                targetObject.transform.position = forwardPosition;
                targetObject.SetActive(true); // Faire appara�tre l'objet
            }
        }
        else
        {
            gazeTimer = 0f; // R�initialiser si la fixation est perdue
            targetObject.SetActive(false);
        }
    }
}