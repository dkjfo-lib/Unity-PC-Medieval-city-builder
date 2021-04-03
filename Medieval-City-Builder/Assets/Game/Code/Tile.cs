using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Building building;
    public Transform buildingPlacement;

    public void BuildBuilding(Building buildingPrefab)
    {
        if (building != null)
        {
            DestroyBuilding();
        }
        if (buildingPrefab == null) return;
        ConstructBuilding(buildingPrefab);
    }

    public void DestroyBuilding()
    {
        Destroy(building.gameObject);
    }

    public void ConstructBuilding(Building buildingPrefab)
    {
        building = Instantiate(buildingPrefab, buildingPlacement, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BuildBuilding(BuildingManager.GetInstance.GetBuilding());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
