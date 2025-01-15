using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class PlacementManager : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameObject spawnedObject;
    private GameObject trackablesPlanes;

    [SerializeField]
    private GameObject PlaceablePrefab;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }
    private void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(PlaceablePrefab, hitPos.position, hitPos.rotation);
                planeManager.enabled = false;
                trackablesPlanes = GameObject.Find("Trackables");
                trackablesPlanes.SetActive(false);
            }

        }
    }

}
