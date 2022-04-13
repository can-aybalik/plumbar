using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;
using System.Linq;


[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MonoBehaviour
{

    [SerializeField]
    private Camera arCamera;

    public Text debugText;

    public bool namePanelCheck = false;
    public bool resolveCheck = false;

    private GameObject[] pipeObjects;
    private GameObject[] selectedObjects;
    private GameObject[] unselectedObjects;

    public GameObject getPipesButton;
    public ARPlaneManager aRPlaneManager;

    public pipeController pipecontroller;

    public GameObject anchor;
    public GameObject[] pipes;
    public ARAnchorManager AnchorManager;
    private GameObject gameObjectToInstantiate;

    public GameObject arPlane;
    public GameObject arSessionOrigin;
    public GameObject arSession;

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
    private bool isVertical = false;

    // Start is called before the first frame update

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //private List<GameObject> gameObjects = new List<GameObject>();

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

            if (touchPosition.y < Screen.height * 1 / 5 || touchPosition.y > Screen.height * 4 / 5)
            {
                return false;
            }

            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if(pipecontroller.check == true)
        {
            pipecontroller.check = false;
            getPipes();
        }
        if (checkRotate == true && GameObject.FindGameObjectWithTag("Selected") != null)
        {

            GameObject.FindGameObjectWithTag("Selected").transform.Rotate(Vector3.forward * 50 * Time.deltaTime, Space.Self);
        }

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject) && !namePanelCheck && !resolveCheck)
        {
            if (hitObject.transform.tag != "Plane" && touch.phase == TouchPhase.Began) //Object Hit
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
                    //


                    Quaternion orientation = Quaternion.identity;
                    Quaternion zUp = Quaternion.identity;
                    GetWallPlacement(hits[0], out orientation, out zUp);

                    if (hitObject.transform.GetComponent<ARPlane>().alignment == PlaneAlignment.Vertical)
                    {
                        orientation *= Quaternion.Euler(new Vector3(90, 0, 0));
                        zUp *= Quaternion.Euler(new Vector3(90, 0, 0));
                        isVertical = true;
                    }
                    else
                    {
                        isVertical = false;
                    }

                    spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, orientation); //OLUŞTURMA
                    spawnedObject.transform.rotation = zUp;


                    //Pose objectPose = new Pose(spawnedObject.transform.position, spawnedObject.transform.rotation);
                    //ARAnchor referencePoint = AnchorManager.AddAnchor(objectPose);

                    

                    /*
                    if(spawnedObject != null && anchor != null)
                    {
                        //spawnedObject.transform.parent = anchor.transform;
                        spawnedObject.transform.SetParent(anchor.transform);
                    }
                    */  
                    
                    
                    //gameObjects.Add(gameObjectToInstantiate);
                }
                else if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    if (spawnedObject.tag == "Selected")
                    {
                        if (isVertical)
                        {
                            spawnedObject.transform.position = hitPose.position + new Vector3(0, -spawnedObject.GetComponent<BoxCollider>().center.y, 0); //YERİNİ DEĞİŞTİRME
                            //spawnedObject.transform.position = hitPose.position;
                        }
                        else
                        {
                            spawnedObject.transform.position = hitPose.position + new Vector3(-spawnedObject.GetComponent<BoxCollider>().center.x, 0, 0);
                            //spawnedObject.transform.position = hitPose.position;
                        }
                    }
                }
            }



        }

        

    }

    public void setPipeType(GameObject pipe)
    {
        if (spawnedObject != null)
        {
            unselectObject(spawnedObject);
        }


        spawnedObject = null;
        gameObjectToInstantiate = pipe;
        deleteButton.GetComponent<Button>().interactable = false;
        rotateButton.GetComponent<Button>().interactable = false;
    }

    public void getPipes()
    {
        GameObject myAnchor = Instantiate(anchor, GameObject.FindGameObjectWithTag("Anchor").transform); //Instantiate Anchor

        Transform anchorPos = myAnchor.transform;

        GameObject.FindGameObjectWithTag("Anchor").GetComponent<MeshRenderer>().enabled = false;

        debugText.text = pipecontroller.pipe_data;

        string[] pipe_star = (pipecontroller.pipe_data).Split('*');

        string pipe_name = "";
        Vector3 pipe_position = new Vector3(0,0,0);
        Quaternion pipe_rotation = new Quaternion(0,0,0,0);


        for( int i = 0; i < pipe_star.Length - 1; i++)
        {
            string[] pipe_dash = pipe_star[i].Split('|');

            
            pipe_name = pipe_dash[0];
            pipe_position = getVector3(pipe_dash[1]);
            pipe_rotation = getQuaternion(pipe_dash[2]);


            //Instantiate(pipes[int.Parse(pipe_name) - 1], pipe_position + anchorPos.position, Quaternion.Euler(pipe_rotation + anchorPos.rotation.eulerAngles));
            Instantiate(pipes[int.Parse(pipe_name) - 1], pipe_position + anchorPos.position, pipe_rotation * anchorPos.rotation);
        }

        getPipesButton.SetActive(false);

        foreach (var plane in arSessionOrigin.GetComponent<ARPlaneManager>().trackables)
        {
            plane.gameObject.SetActive(false);
        }
        seePlane = false;
        arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
    }

    public Vector3 getVector3(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');


        double x = double.Parse(temp[0], CultureInfo.InvariantCulture.NumberFormat);
        double y = double.Parse(temp[1], CultureInfo.InvariantCulture.NumberFormat);
        double z = double.Parse(temp[2], CultureInfo.InvariantCulture.NumberFormat);

        Vector3 rValue = new Vector3((float)x, (float)y, (float)z);
        return rValue;
    }

    public Quaternion getQuaternion(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');


        double x = double.Parse(temp[0], CultureInfo.InvariantCulture.NumberFormat);
        double y = double.Parse(temp[1], CultureInfo.InvariantCulture.NumberFormat);
        double z = double.Parse(temp[2], CultureInfo.InvariantCulture.NumberFormat);
        double w = double.Parse(temp[3], CultureInfo.InvariantCulture.NumberFormat);

        Quaternion rValue = new Quaternion((float)x, (float)y, (float)z, (float)w);
        return rValue;
    }

    public void deleteObject()
    {
        if (GameObject.FindGameObjectWithTag("Selected") != null)
        {
            //gameObjects.Remove(GameObject.FindGameObjectWithTag("Selected"));
            Destroy(GameObject.FindGameObjectWithTag("Selected"));
        }
        deleteButton.GetComponent<Button>().interactable = false;
        rotateButton.GetComponent<Button>().interactable = false;
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
        if (seePlane == true)
        {
            //arPlane.GetComponent<LineRenderer>().enabled = false;
            //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
            //arSessionOrigin.GetComponent<ARPlaneManager>().requestedDetectionMode = PlaneDetectionMode.None;


            foreach (var plane in arSessionOrigin.GetComponent<ARPlaneManager>().trackables)
            {
                plane.gameObject.SetActive(false);
            }
            seePlane = false;
            arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        }
        else
        {
            //arPlane.GetComponent<LineRenderer>().enabled = true;
            //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
            //arSessionOrigin.GetComponent<ARPlaneManager>().requestedDetectionMode = PlaneDetectionMode.None;
            arSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
            foreach (var plane in arSessionOrigin.GetComponent<ARPlaneManager>().trackables)
            {
                plane.gameObject.SetActive(true);
            }
            seePlane = true;
        }



    }

    public void resetPlane()
    {

        //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;

    }


    public void resetSession()
    {

        pipeObjects = GameObject.FindGameObjectsWithTag("Pipe");
        selectedObjects = GameObject.FindGameObjectsWithTag("Selected");
        unselectedObjects = GameObject.FindGameObjectsWithTag("Unselected");

        var finalArray = pipeObjects.Concat(selectedObjects);
        finalArray = finalArray.Concat(unselectedObjects);
        finalArray.ToArray();

        foreach(GameObject pipe in selectedObjects)
        {
            unselectObject(pipe);
        }

        spawnedObject = null;
        gameObjectToInstantiate = null;
        deleteButton.GetComponent<Button>().interactable = false;
        rotateButton.GetComponent<Button>().interactable = false;

        foreach (GameObject pipe in finalArray)
        {
            Destroy(pipe.gameObject);
        }

        arSession.GetComponent<ARSession>().Reset();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);


    }

    void ChangeSelectedObject(GameObject gameObject)
    {
        if (gameObject.tag != "Selected") //Object Not Selected
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.red;
            if (GameObject.FindGameObjectWithTag("Selected") != null)
            {
                unselectObject(GameObject.FindGameObjectWithTag("Selected"));

            }
            gameObject.tag = "Selected";
            deleteButton.GetComponent<Button>().interactable = true;
            rotateButton.GetComponent<Button>().interactable = true;
        }



    }

    void unselectObject(GameObject gameObject)
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.white;
        gameObject.tag = "Unselected";
    }

    private void GetWallPlacement(ARRaycastHit _planeHit, out Quaternion orientation, out Quaternion zUp)
    {
        TrackableId planeHit_ID = _planeHit.trackableId;
        ARPlane planeHit = arSessionOrigin.GetComponent<ARPlaneManager>().GetPlane(planeHit_ID);
        Vector3 planeNormal = planeHit.normal;
        orientation = Quaternion.FromToRotation(Vector3.up, planeNormal);
        Vector3 forward = _planeHit.pose.position - (_planeHit.pose.position + Vector3.down);
        zUp = Quaternion.LookRotation(forward, planeNormal);
    }

}