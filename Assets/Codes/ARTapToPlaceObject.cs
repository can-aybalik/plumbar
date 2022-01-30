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
    public GameObject sphere;

    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Touch touch;
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
            touch = Input.GetTouch(0);
            touchPosition = touch.position;
            
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            if(hitObject.transform.tag != "Plane" && touch.phase == TouchPhase.Began) //Object Hit
            {
                //Instantiate(sphere, hitObject.transform.position, hitObject.transform.rotation);
                ChangeSelectedObject(hitObject.collider.gameObject);
                spawnedObject = hitObject.collider.gameObject;
            }

            else if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) //Plane Hit
            {
                var hitPose = hits[0].pose;

                if (spawnedObject == null && touch.phase == TouchPhase.Began)
                {
                    spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation); //OLUÞTURMA
                }
                else if(touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    if (spawnedObject.tag == "Selected")
                    {
                        spawnedObject.transform.position = hitPose.position; //YERÝNÝ DEÐÝÞTÝRME
                    }
                }
            }


        }

    }

    public void setPipeType(GameObject pipe)
    {
        spawnedObject = null;
        gameObjectToInstantiate = pipe;
    }

    void ChangeSelectedObject(GameObject gameObject)
    {
        if(gameObject.tag != "Selected") //Object Not Selected
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.black;
            gameObject.tag = "Selected";
        }

        
           
    }
}