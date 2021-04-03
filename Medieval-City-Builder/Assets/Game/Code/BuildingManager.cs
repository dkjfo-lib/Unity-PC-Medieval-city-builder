using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : BaseSingleton<BuildingManager>
{
    public List<Building> Buildings;
    public int currentBuildingId = 0;

    private void Start()
    {
        Buildings.Insert(0, null);
    }

    public void SetGameIndex(int id)
    {
        currentBuildingId = id;
    }

    public Building GetBuilding()
    {
        return Buildings[currentBuildingId];
    }
}
