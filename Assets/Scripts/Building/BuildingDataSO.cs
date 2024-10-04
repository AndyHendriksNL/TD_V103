using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Objects/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    public string buildingName;
    public GameObject buildingPrefab;
    public Button buildButton;
    public int buildingHealth;
    public int goldCost;
    public int villagerUse;
    public int villagerIncrease;
    public int goldIncrease;
}
