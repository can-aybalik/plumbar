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
    public GameObject arPlane;
    public GameObject arSessionOrigin;

    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Touch touch;
    private int idOfLastObject = -1;

    public GameObject deleteButton;
    public GameObject rotateButton;

    private int upperY = 975;
    private int lowerY = -895;

    private bool checkRotate = false;
    private bool seePlane = true;

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

            else if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
               

                if (spawnedObject == null && touch.phase == TouchPhase.Began)
                {
                    spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation); //OLUŞTURMA
                }
                else if(touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    if (spawnedObject.tag == "Selected")
                    {
                        spawnedObject.transform.position = hitPose.position; //YERİNİ DEĞİŞTİRME
                    }
                }
            }



        }

        if (checkRotate == true && GameObject.FindGameObjectWithTag("Selected") != null)
        {

            GameObject.FindGameObjectWithTag("Selected").transform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.Self);
        }

    }

    public void setPipeType(GameObject pipe)
    {
        if(spawnedObject != null)
        {
            unselectObject(spawnedObject);
        }


        spawnedObject = null;
        gameObjectToInstantiate = pipe;
        deleteButton.SetActive(false);
        rotateButton.SetActive(false);
    }

    public void deleteObject()
    {
        if (GameObject.FindGameObjectWithTag("Selected") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Selected"));
        }
        deleteButton.SetActive(false);
    }

    public void rotateObject()
    {
        checkRotate = true;
       

      
    }

    public void stopRotateObject()
    {
        checkRotate = false;
    }

    public void togglePlane()
    {
        if(seePlane == true)
        {
            //arPlane.GetComponent<LineRenderer>().enabled = false;
            //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
            //arSessionOrigin.GetComponent<ARPlaneManager>().requestedDetectionMode = PlaneDetectionMode.None;


            foreach (var plane in arSessionOrigin.GetComponent<ARPlaneManager>().trackables)
            {
                plane.gameObject.SetActive(false);
            }
            seePlane = false;
        }
        else
        {
            //arPlane.GetComponent<LineRenderer>().enabled = true;
            //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
            //arSessionOrigin.GetComponent<ARPlaneManager>().requestedDetectionMode = PlaneDetectionMode.None;
            foreach (var plane in arSessionOrigin.GetComponent<ARPlaneManager>().trackables)
            {
                plane.gameObject.SetActive(true);
            }
            seePlane = true;
        }

        

    }

    void ChangeSelectedObject(GameObject gameObject)
    {
        if(gameObject.tag != "Selected") //Object Not Selected
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.black;
            if(GameObject.FindGameObjectWithTag("Selected") != null)
            {
                unselectObject(GameObject.FindGameObjectWithTag("Selected"));

            }
            gameObject.tag = "Selected";
            deleteButton.SetActive(true);
            rotateButton.SetActive(true);
        }
        

      
    }

    void unselectObject(GameObject gameObject)
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.red;
        gameObject.tag = "Unselected";
    }

}