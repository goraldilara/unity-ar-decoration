using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    //ar variables
    private ARRaycastManager aRRaycastManager;
    Camera arCam;

    //placement elements
    public GameObject placementIndicator;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private GameObject objectToPlace;
    GameObject spawnedObject;
    private bool isPlaced = false;
    public GameObject noticePanel;

    //ui elements
    public Button catalogButton;
    public GameObject catalogPanel;
    private bool isClicked = false;

    //button element
    public RectTransform parentPanel;
    private Vector3 buttonLocation;
    private bool newButtonFlag = false;
    //GameObject selectedButton;
    private GameObject newButtonSample;
    int buttonCount = 1;
    //try
    //private GameObject selectedButton;
    private GameObject spawnedButton;

    //delete that
    public TMP_Text textarea;
    public TMP_Text textarea2;
    public TMP_Text textarea3;
    public Button buttonssss;

    //raycast
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private bool isTouched = false;
    private bool isSelected = false;

    //spawned
    Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();
    Dictionary<string, int> arObjectCounts = new Dictionary<string, int>();
    private GameObject selectedObject;
    private int counter;

    //scale
    float endDistance;
    float currentDistance;

    Vector3 scaleBigger = new Vector3(0.005f, 0.005f, 0.005f);
    Vector3 scaleSmaller = new Vector3(-0.005f, -0.005f, -0.005f);

    //rigidbody
    private Rigidbody rigidOfObject;

    // Start is called before the first frame update
    void Start()
    {
        //ar initialization
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();

        catalogPanel.SetActive(false);
        arObjects.Clear();
    }

    public void ShowCatalog(GameObject panel)
    {
        //hide catalog panel if it's shown
        if(panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
        //show catalog panel if it's hidden
        else
        {
            panel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (isClicked == true && isPlaced == false)
        {
            if (placementPoseIsValid  && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }

        ////try for normal objects
        //if(Input.touchCount > 0)
        //{
        //    if(Input.touchCount == 1)
        //    {
        //        RaycastHit hit;
        //        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        //        if(aRRaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        //        {
        //            if(Input.GetTouch(0).phase == TouchPhase.Began)
        //            {
        //                if(Physics.Raycast(ray.origin, ray.direction, out hit))
        //                {
        //                    if(hit.collider.tag == "bb")
        //                    {
        //                        selectedObject = hit.transform.gameObject;
        //                        //hit.collider.transform.position = m_Hits[0].pose.position;
        //                    }
        //                }
        //            }

        //            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //            {
        //                selectedObject.transform.position = m_Hits[0].pose.position;
        //            }
        //        }


        //    }
        //}

        if (isSelected)
        {
            if (Input.touchCount > 0)
            {
                //drag, move and rotate operations with one touch on the screen
                if (Input.touchCount == 1)
                {
                    //variables for ray casting on spawned object
                    RaycastHit hit;
                    Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

                    if (aRRaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            if (Physics.Raycast(ray.origin, ray.direction, out hit))
                            {
                                //control for if ray hit an object on the screen
                                if (hit.collider.name == selectedObject.name)
                                {
                                    isTouched = true;
                                }
                            }
                        }
                        //move and drag operation
                        else if (Input.GetTouch(0).phase == TouchPhase.Moved && isTouched == true)
                        {
                            selectedObject.transform.position = m_Hits[0].pose.position;
                        }
                        //rotate operation
                        else if (Input.GetTouch(0).phase == TouchPhase.Moved && isTouched == false)
                        {
                            selectedObject.transform.Rotate(0f, Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y);
                        }
                        //cut the connection on object when touch is ended
                        if (Input.GetTouch(0).phase == TouchPhase.Ended)
                        {
                            isTouched = false;
                        }
                    }
                }
                //pinch and scale operation with two touch on the screen
                else if (Input.touchCount == 2)
                {
                    Touch firstTouch = Input.GetTouch(0);
                    Touch secondTouch = Input.GetTouch(1);

                    if (firstTouch.phase == TouchPhase.Began && secondTouch.phase == TouchPhase.Began)
                    {
                        currentDistance = Vector3.Distance(firstTouch.position, secondTouch.position);
                    }
                    else if (firstTouch.phase == TouchPhase.Moved && secondTouch.phase == TouchPhase.Moved)
                    {
                        //calculate distance between two points and decide to increase/decrease in size
                        endDistance = Vector3.Distance(firstTouch.position, secondTouch.position);

                        if (currentDistance > endDistance)
                        {
                            scale(scaleSmaller);
                        }
                        else
                        {
                            scale(scaleBigger);
                        }
                        currentDistance = endDistance;
                    }
                }
            }
        }


    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (isClicked == true && isPlaced == false && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
    
    public void SetObjectToPlace(GameObject ARObject)
    { 
        objectToPlace = ARObject;
        isClicked = true;
        isPlaced = false;
    }
    public void SetSelectedButton(GameObject buttonPrefab)
    {
        newButtonSample = buttonPrefab;
    }

    private void PlaceObject()
    {
        if (arObjects.ContainsKey(objectToPlace.name+"(Clone)"))
        {
            //string tempName = objectToPlace.name + "(Clone)";
            counter = arObjectCounts[objectToPlace.name] + 1;
            objectToPlace.name += "-" + counter.ToString();
            spawnedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
            if (counter > 9)
            {
                objectToPlace.name = objectToPlace.name.Remove(objectToPlace.name.Length - 3, 3);
            }
            else if(counter == 25)
            {
                noticePanel.SetActive(true);
                return;
            }
            else
            {
                objectToPlace.name = objectToPlace.name.Remove(objectToPlace.name.Length - 2, 2);
            }
            textarea.text = objectToPlace.name;
            textarea2.text = spawnedObject.name;
            arObjects.Add(spawnedObject.name, spawnedObject);
            arObjectCounts[objectToPlace.name] = counter;
            CreateSelectedButton();
        }
        else
        {
            spawnedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
            arObjects.Add(spawnedObject.name, spawnedObject);
            //þu mantýklý mý
            arObjectCounts.Add(objectToPlace.name, 1);
            CreateSelectedButton();
        }
        isPlaced = true;
        isClicked = false;
    }

    private void CreateSelectedButton()
    {
        newButtonSample.name = spawnedObject.name.Remove(spawnedObject.name.Length - 7, 7);
        GameObject selectedButton = Instantiate(newButtonSample, parentPanel);
        selectedButton.transform.localScale = new Vector3(1, 1, 1);
        //selectedButton.transform.localPosition = new Vector3(parentPanel.localPosition.x+10, 700-(buttonCount*180), parentPanel.localPosition.z);
        buttonCount += 1;
        textarea3.text = selectedButton.name;
        selectedButton.GetComponent<Button>().onClick.AddListener(() => SetSelectedObject(selectedButton.name));
        selectedButton.GetComponent<Button>().onClick.AddListener(() => SetSpawnedButton(selectedButton));
    }

    private void SetSelectedObject(string buttonName)
    {
        //set spawnedobject as selected object
        arObjects.TryGetValue(buttonName, out selectedObject);
        isSelected = true;
    }

    private void SetSpawnedButton(GameObject selectedButton)
    {
        spawnedButton = selectedButton;
    }

    //public void ColorSelectedObject(GameObject objectToColor)
    //{
    //    if(objectToColor.GetComponent<Outline>() == null)
    //    {
    //        Outline outline = objectToColor.AddComponent<Outline>();
    //        outline.enabled = true;
    //        objectToColor.GetComponent<Outline>().OutlineColor = Color.red;
    //        objectToColor.GetComponent<Outline>().OutlineWidth = 7.0f;
    //    }
    //}

    void scale(Vector3 scaleChange)
    {
        selectedObject.transform.localScale += scaleChange;
    }

    public void DeleteSpawnedObject()
    {
        if (isSelected)
        {
            Destroy(selectedObject);
            Destroy(spawnedButton);
        }
    }
}
