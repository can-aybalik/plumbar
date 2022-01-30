using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MonoBehaviour
{

    [SerializeField]
    private Camera arCamera;

    public GameObject gameObjectToInstantiate;

    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    // Start is called before the first frame update

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
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

    void Update()
    {

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        /*
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            GameObject gameObject = hitObject.transform.GetComponent<GameObject>();
            if (gameObject != null)
            {
                ChangeSelectedObject(gameObject);
            }
        }
        */
        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) //Cube Týklamazsak
        {
            var hitPose = hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }

    public void setPipeType(GameObject pipe)
    {
        spawnedObject = null;
        gameObjectToInstantiate = pipe;
    }

    void ChangeSelectedObject(GameObject selected)
    {
        
        MeshRenderer meshRenderer = selected.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.black;
           
    }
}