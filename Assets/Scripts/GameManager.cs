using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ResourceTypeSO goldResource; // Verwijzing naar het ResourceTypeSO voor goud
    public ResourceTypeSO villagersResource; // Verwijzing naar het ResourceTypeSO voor villagers
    public BuildingDataSO goldMineSO;
    public BuildingDataSO cityCenterSO;

    private Dictionary<BuildingDataSO, int> placedBuildings = new Dictionary<BuildingDataSO, int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Blijft tussen verschillende sc�nes bestaan
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        InitializeDefaultBuildings();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }

    public void Victory()
    {
        Debug.Log("Victory!");
    }

    private void InitializeDefaultBuildings()
    {
        // Plaats het City Center
        Vector3 cityCenterPosition = new Vector3(-5, 0, -5); // Aanpassen naar de gewenste positie
        GameObject cityCenter = Instantiate(cityCenterSO.buildingPrefab, cityCenterPosition, Quaternion.identity);
        BuildingPlacementValidator cityCenterValidator = cityCenter.GetComponent<BuildingPlacementValidator>();

        // Voeg toe aan placedBuildings
        AddBuilding(cityCenterSO);

        // Plaats de GoldMine
        Vector3 goldMinePosition = new Vector3(10, 0, 10); // Aanpassen naar de gewenste positie
        GameObject goldMine = Instantiate(goldMineSO.buildingPrefab, goldMinePosition, Quaternion.identity);
        BuildingPlacementValidator goldMineValidator = goldMine.GetComponent<BuildingPlacementValidator>();

        // Voeg toe aan placedBuildings
        AddBuilding(goldMineSO);
    }

    public void AddBuilding(BuildingDataSO buildingData)
    {
        //Debug.Log("Add to placedBuildings: " + buildingData.name);
        
        if (!placedBuildings.ContainsKey(buildingData))
        {
            placedBuildings[buildingData] = 0; // Initialiseer met 0
        }

        placedBuildings[buildingData] += 1; // Verhoog het aantal gebouwen

        ResourceManager.instance.OnBuildingPlaced();

    }

    public Dictionary<BuildingDataSO, int> GetPlacedBuildings()
    {
        return placedBuildings;
    }
}
