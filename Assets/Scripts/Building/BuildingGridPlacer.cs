using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingGridPlacer : BuildingPlacer
{
    public float cellSize;
    public Vector2 gridOffset;
    public Renderer gridRenderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _UpdateGridVisual();
    }
#endif

    private void Start()
    {
        _UpdateGridVisual();
        _EnableGridVisual(false);
    }

    private void Update()
    {
        if (NotificationUI.instance == null)
        {
            //Debug.LogError("NotificationUI instance is not assigned!");
            return;
        }
        
        if (_buildingPrefab != null && _buildingDataSO != null)
        { // if in build mode

            // Set the buildingmode to true
            BuildingInteraction.instance.SetBUildingMode(true);

            // right-click: cancel build mode
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(_toBuild);
                _toBuild = null;
                _buildingPrefab = null;
                _EnableGridVisual(false);
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
                _toBuild.transform.position = _ClampToNearest(_hit.point, cellSize);

                if (Input.GetMouseButtonDown(0))
                { // if left-click
                    BuildingPlacementValidator m = _toBuild.GetComponent<BuildingPlacementValidator>();
                    if (m.hasValidPlacement)
                    {
                        if(CanPlaceBuilding(_buildingDataSO))
                        {
                            // place the building
                            m.SetPlacementMode(PlacementMode.Fixed);

                            // Trek de kosten van het gebouw af van het totaal goud
                            ResourceTypeSO goldType = ResourceManager.instance.resourceTypeList.list.Find(r => r.type == ResourceType.Gold);
                            ResourceManager.instance.AddResource(goldType, -_buildingDataSO.goldCost);

                            // Meld het ResourceManager systeem dat er een nieuw gebouw is geplaatst
                            ResourceManager.instance.OnBuildingPlaced();

                            // add to placedBuildings
                            GameManager.instance.AddBuilding(_buildingDataSO);
                        }
                        else
                        {
                            //Debug.Log("Send noditifcation: Not enough gold!");
                            NotificationUI.instance.SetNotificationMessage("Not enough gold!");
                        }  

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
                            _buildingDataSO = null;
                            _toBuild = null;
                            _EnableGridVisual(false);

                            // Set the buildingmode to false
                            BuildingInteraction.instance.SetBUildingMode(false);
                        }
                    }
                    else
                    {
                        //Debug.Log("Send noditifcation: Cannot build here!");
                        NotificationUI.instance.SetNotificationMessage("Cannot build here!");
                    }
                }

            }
            else if (_toBuild.activeSelf) _toBuild.SetActive(false);
        }
    }

    protected override void _PrepareBuilding()
    {
        base._PrepareBuilding();
        _EnableGridVisual(true);
    }

    private Vector3 _ClampToNearest(Vector3 pos, float threshold)
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

        float s = threshold * 0.5f;
        v.x += s + gridOffset.x; // (recenter in middle of cells)
        v.z += s + gridOffset.y;

        return v;
    }

    private void _EnableGridVisual(bool on)
    {
        if (gridRenderer == null) return;
        gridRenderer.gameObject.SetActive(on);
    }

    private void _UpdateGridVisual()
    {
        if (gridRenderer == null) return;
        gridRenderer.sharedMaterial.SetVector(
            "_Cell_Size", new Vector4(cellSize, cellSize, 0, 0));
    }    
}
