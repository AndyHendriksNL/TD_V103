using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance; // (Singleton pattern)

    public LayerMask groundLayerMask;

    protected GameObject _buildingPrefab;
    protected BuildingDataSO _buildingDataSO;
    protected GameObject _toBuild;

    protected Camera _mainCamera;

    protected Ray _ray;
    protected RaycastHit _hit;

    private void Awake()
    {
        instance = this; // (Singleton pattern)
        _mainCamera = Camera.main;
        _buildingPrefab = null;
        _buildingDataSO = null;
    }

    private void Update()
    {
        if (_buildingPrefab != null)
        { // if in build mode

            // right-click: cancel build mode
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(_toBuild);
                _toBuild = null;
                _buildingPrefab = null;
                return;
            }

            // hide preview when hovering UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (_toBuild.activeSelf) _toBuild.SetActive(false);
                return;
            }
            else if (!_toBuild.activeSelf) _toBuild.SetActive(true);

            // rotate preview with R
            if (Input.GetKeyDown(KeyCode.R))
            {
                _toBuild.transform.Rotate(Vector3.up, 90);
            }

            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 1000f, groundLayerMask))
            {
                if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                _toBuild.transform.position = _hit.point;

                if (Input.GetMouseButtonDown(0))
                { // if left-click
                    BuildingPlacementValidator m = _toBuild.GetComponent<BuildingPlacementValidator>();
                    if (m.hasValidPlacement)
                    {
                        m.SetPlacementMode(PlacementMode.Fixed);

                        // shift-key: chain builds
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        {
                            _toBuild = null; // (to avoid destruction)
                            _PrepareBuilding();
                        }
                        // exit build mode
                        else
                        {
                            _buildingPrefab = null;
                            _toBuild = null;
                        }
                    }
                }

            }
            else if (_toBuild.activeSelf) _toBuild.SetActive(false);
        }
    }

    public void SetBuildingPrefab(BuildingDataSO buildingSO)
    {
        // check of er genoeg goud is om het gebouw te betalen
        if(CanPlaceBuilding(buildingSO))
        {
            _buildingDataSO = buildingSO;
            _buildingPrefab = buildingSO.buildingPrefab;        
            _PrepareBuilding();
            EventSystem.current.SetSelectedGameObject(null); // cancel keyboard UI nav
        }
        else
        {
            Debug.Log("Niet genoeg goud voor gebouw: " + buildingSO.name);
        }             
    }

    protected virtual void _PrepareBuilding()
    {
        if (_toBuild) Destroy(_toBuild);

        _toBuild = Instantiate(_buildingPrefab);
        _toBuild.SetActive(false);

        BuildingPlacementValidator m = _toBuild.GetComponent<BuildingPlacementValidator>();
        m.isFixed = false;
        m.SetPlacementMode(PlacementMode.Valid);
    }

    public bool CanPlaceBuilding(BuildingDataSO buildingData)
    {
        // Haal het goud resource type op
        ResourceTypeSO goldType = ResourceManager.instance.resourceTypeList.list.Find(r => r.type == ResourceType.Gold);

        // Controleer of er genoeg goud is om het gebouw te plaatsen
        if (ResourceManager.instance.GetResourceAmount(goldType) >= buildingData.goldCost)
        {
            return true; // Genoeg goud
        }

        //Debug.Log("Niet genoeg goud om dit gebouw te plaatsen.");
        //Debug.Log("Send noditifcation: Not enough gold!");
        NotificationUI.instance.SetNotificationMessage("Not enough gold!");
        return false; // Niet genoeg goud
    }

}