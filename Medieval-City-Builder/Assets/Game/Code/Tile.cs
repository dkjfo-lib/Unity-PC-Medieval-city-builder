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
        BuildBuilding(building);
    }

    public void BuildBuilding(Building buildingPrefab)
    {
        if (hasBuilding)
        {
            DestroyBuilding();
        }
        if (buildingPrefab == null) return;
        ConstructBuilding(buildingPrefab);
    }

    public void DestroyBuilding()
    {
        DestroyImmediate(building.gameObject);
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
