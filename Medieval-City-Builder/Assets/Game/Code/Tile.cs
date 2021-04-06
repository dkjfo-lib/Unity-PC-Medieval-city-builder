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
        bool can = true;
        for (int i = 0; i < stats.ConstructionCost.Resources.Length; i++)
        {
            can &= ResourceManager.GetInstance.HasResource((ResourceType)i, stats.ConstructionCost.Resources[i]);
        }
        return can;
    }

    public void DestroyBuilding()
    {
        DestroyImmediate(building.gameObject);
        building = null;
    }

    public void ConstructBuilding(Building buildingPrefab)
    {
        for (int i = 0; i < buildingPrefab.Stats.ConstructionCost.Resources.Length; i++)
        {
            ResourceManager.GetInstance.RemoveResource((ResourceType)i, buildingPrefab.Stats.ConstructionCost.Resources[i]);
        }
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
