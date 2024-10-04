using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class BuildingInteraction : MonoBehaviour
{
    public static BuildingInteraction instance;
    public LayerMask selectableLayerMask;
    public GameObject detailsPanel; // The UI panel to show object details
    public TextMeshProUGUI nameText; // UI text to display object name

    private Camera _mainCamera;
    private GameObject selectedObject;
    private bool buildingModeEnabled;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            // destroy duplicates
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        detailsPanel.SetActive(false); // Hide panel initially
    }

    private void Update()
    {
        //Debug.Log("BuildMode: " + buildingModeEnabled);

        if (!buildingModeEnabled)
        {
        
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) // Left-click
            {
                SelectObject();
            }
            
        }

        if(Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            DeselectObject();
        }
    }

    void SelectObject()
    {        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, selectableLayerMask))
        {
            //Debug.Log("Hit " + hit);
            if (hit.collider.gameObject != selectedObject)
            {
                //Debug.Log("selectedObject " + selectedObject);
                DeselectObject(); // Deselect the previous object
                selectedObject = hit.collider.gameObject;

                var outline = selectedObject.GetComponent<Outline>() ?? selectedObject.AddComponent<Outline>();
                outline.enabled = true;

                ShowDetails(selectedObject); // Show object details in UI
            }
        }
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Outline>().enabled = false;          
            selectedObject = null;
            detailsPanel.SetActive(false); // Hide UI panel
        }
    }

    void ShowDetails(GameObject obj)
    {
        BuildingPlacementValidator building = obj.GetComponent<BuildingPlacementValidator>();
        if (building != null && building.buildingData != null)
        {
            // Update UI panel with the building's information
            nameText.text = building.buildingData.buildingName;

            detailsPanel.SetActive(true); // Show the details UI panel
        }
    }

    public void SetBUildingMode(bool buildingMode)
    {
        buildingModeEnabled = buildingMode;        
    }
}
