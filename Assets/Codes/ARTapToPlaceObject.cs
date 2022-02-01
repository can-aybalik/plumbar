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
    private int idOfLastObject = -1;

    public GameObject deleteButton;
    public GameObject rotateButton;

    private int upperY = 975;
    private int lowerY = -895;

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

            else if (hitObject.transform.tag == "Plane" && _arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) //Plane Hit
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
        touch = Input.GetTouch(0);

        if (GameObject.FindGameObjectWithTag("Selected") != null)
        {
            
            GameObject.FindGameObjectWithTag("Selected").transform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.Self);
            
            
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