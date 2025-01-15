using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))] // ajoute automatiquement ce script s'il n'est pas présent 
public class ARSimulationPlacementManager : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameObject spawnedObject;
    private GameObject trackablesPlanes;

    [SerializeField]
    private GameObject PlaceablePrefab;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector3 mousePosition = new Vector3(0, 0, 0);

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
            Vector2 mousePositionOnScreen = new Vector2(mousePosition[0], mousePosition[1]);

            if (raycastManager.Raycast(mousePositionOnScreen, hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("on plane");
                var hitPos = hits[0].pose;
                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(PlaceablePrefab, hitPos.position, hitPos.rotation);

                    trackablesPlanes = GameObject.Find("Trackables");
                    trackablesPlanes.SetActive(false);
                    planeManager.enabled = false;
                }

            }


        }
        



    }



}