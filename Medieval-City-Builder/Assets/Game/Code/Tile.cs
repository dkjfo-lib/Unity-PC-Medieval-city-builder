using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Building building;
    public Transform buildingPlacement;
    public bool hasBuilding => buildingPlacement.childCount > 0;
    [Space]
    public Material material_normal;
    public Material material_highlighted;
    public MeshRenderer mesh;

    private void Start()
    {
        BuildOnStart(building);
    }
    private void BuildOnStart(Building buildingPrefab)
    {
        if (buildingPrefab == null) return;
        ConstructBuilding(buildingPrefab);
    }

    public void BuildBuilding(Building buildingPrefab)
    {
        if (!CanBuild(buildingPrefab?.Stats)) return;
        if (hasBuilding)
        {
            DestroyBuilding();
        }
        if (buildingPrefab == null) return;
        ConstructBuilding(buildingPrefab);
    }

    bool CanBuild(BuildingStats stats)
    {
        if (stats == null) return true;
        if (stats == building?.Stats) return false;
        return StorageHelper.HasAll(ResourceManager.GetInstance, stats.ConstructionCost);
    }

    public void DestroyBuilding()
    {
        DestroyImmediate(building.gameObject);
        building = null;
    }

    public void ConstructBuilding(Building buildingPrefab)
    {
        StorageHelper.RemoveAll(ResourceManager.GetInstance, buildingPrefab.Stats.ConstructionCost);
        building = Instantiate(buildingPrefab, buildingPlacement, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BuildBuilding(BuildingManager.GetInstance.GetBuilding());
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BuildBuilding(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mesh.material = material_highlighted;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mesh.material = material_normal;
    }
}
