using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : BaseSingleton<ResourceManager>, IResourceStorage
{
    [System.Obsolete("Use Methods!", false)]
    public ResourceStorage storage;
    private ResourceStorage Storage { get => storage; set => storage = value; }

    public void AddResource(ResourceType type, int ammount)
    {
        Storage.AddResource(type, ammount);
    }
    public bool HasResource(ResourceType type, int ammount)
    {
        return Storage.HasResource(type, ammount);
    }
    public void RemoveResource(ResourceType type, int ammount)
    {
        Storage.RemoveResource(type, ammount);
    }
    public bool TryRemoveResource(ResourceType type, int ammount)
    {
        bool hasEnoughResources = HasResource(type, ammount);
        if (hasEnoughResources)
            RemoveResource(type, ammount);
        return hasEnoughResources;
    }
}

public enum ResourceType
{
    food,
    wood,
    stone
}

public interface IResourceStorage
{
    void AddResource(ResourceType type, int ammount);
    bool HasResource(ResourceType type, int ammount);
    void RemoveResource(ResourceType type, int ammount);
}

[System.Serializable]
public class ResourceStorage : IResourceStorage
{
    [System.Obsolete("Use Methods!", false)]
    public int[] resources = new int[System.Enum.GetValues(typeof(ResourceType)).Length];
    public int[] Resources { get => resources; }

    public void AddResource(ResourceType type, int ammount)
    {
        Resources[(int)type] += ammount;
    }
    public bool HasResource(ResourceType type, int ammount)
    {
        return Resources[(int)type] >= ammount;
    }
    public void RemoveResource(ResourceType type, int ammount)
    {
        Resources[(int)type] -= ammount;
    }
}