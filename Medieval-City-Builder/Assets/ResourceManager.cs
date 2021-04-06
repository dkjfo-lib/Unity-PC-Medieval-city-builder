using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : BaseSingleton<ResourceManager>, IResourceStorage
{
    public int PERSON_FOOD_CONSUMPTION_PER_DAY { get; } = 1;

    [System.Obsolete("Use Methods!", false)]
    public ResourcePack storage;
    private IResourceStorage Storage { get => storage; }

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
    public int GetResourceAmmount(ResourceType type)
    {
        return Storage.GetResourceAmmount(type);
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
    int GetResourceAmmount(ResourceType type);
}

public static class StorageHelper
{
    public static void DoForEachResource(System.Action<ResourceType> doForEachResource)
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            doForEachResource(type);
        }
    }
    public static float GetSum(System.Func<ResourceType, float> ResourceConverter)
    {
        float sum = 0;
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            sum += ResourceConverter(type);
        }
        return sum;
    }

    public static void AddAll(IResourceStorage target, IResourceStorage add)
    {
        DoForEachResource((t) => target.AddResource(t, add.GetResourceAmmount(t)));
    }
    public static bool HasAll(IResourceStorage target, IResourceStorage required)
    {
        bool can = true;
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            can &= target.HasResource(type, required.GetResourceAmmount(type));
        }
        return can;
    }
    public static void RemoveAll(IResourceStorage target, IResourceStorage remove)
    {
        DoForEachResource((t) => target.RemoveResource(t, remove.GetResourceAmmount(t)));
    }
}

[System.Serializable]
public class ResourcePack : BaseEnumMap<ResourceType, int>, IResourceStorage
{
    public void AddResource(ResourceType type, int ammount)
    {
        Set(type, Get(type) + ammount);
    }

    public int GetResourceAmmount(ResourceType type)
    {
        return Get(type);
    }

    public bool HasResource(ResourceType type, int ammount)
    {
        return Get(type) >= ammount;
    }

    public void RemoveResource(ResourceType type, int ammount)
    {
        Set(type, Get(type) - ammount);
    }
}