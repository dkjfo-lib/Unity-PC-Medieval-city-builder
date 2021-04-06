using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireManager :
    BaseSingleton<DesireManager>,
    IComparer<Job>
{
    [System.Obsolete("Use Methods!", false)]
    public float foodDesire;
    [System.Obsolete("Use Methods!", false)]
    public float woodDesire;
    [System.Obsolete("Use Methods!", false)]
    public float stoneDesire;
    [Space]
    [System.Obsolete("Use Methods!", false)]
    public float foodPerDay;
    [System.Obsolete("Use Methods!", false)]
    public float foodDays;

    public float FoodDesire { get => foodDesire; private set => foodDesire = value; }
    public float WoodDesire { get => woodDesire; private set => woodDesire = value; }
    public float StoneDesire { get => stoneDesire; private set => stoneDesire = value; }
    public float FoodPerDay { get => foodPerDay; private set => foodPerDay = value; }
    public float FoodDays { get => foodDays; private set => foodDays = value; }

    PopulationManager population => PopulationManager.GetInstance;
    ResourceManager storage => ResourceManager.GetInstance;

    private int FoodPerDayFunc => population.PplCount * storage.PERSON_FOOD_CONSUMPTION_PER_DAY;
    private float FoodDaysFunc => (float)storage.GetResourceAmmount(ResourceType.food) / FoodPerDay;

    private float DesireFoodFunc => FoodDaysFunc == 0 ? 10 : 100f / FoodDaysFunc;
    private float DesireWoodFunc => storage.GetResourceAmmount(ResourceType.wood) == 0 ? 10 :
        100f * population.PplCount / storage.GetResourceAmmount(ResourceType.wood);
    private float DesireStoneFunc => storage.GetResourceAmmount(ResourceType.stone) == 0 ? 10 :
        100f * population.PplCount / storage.GetResourceAmmount(ResourceType.stone);

    private void Start()
    {
        StartCoroutine(ManageDesires());
    }

    IEnumerator ManageDesires()
    {
        YieldInstruction waitUpdate = new WaitForSeconds(.25f);
        while (true)
        {
            FoodDesire = DesireFoodFunc;
            WoodDesire = DesireWoodFunc;
            StoneDesire = DesireStoneFunc;
            FoodPerDay = FoodPerDayFunc;
            FoodDays = FoodDaysFunc;
            yield return waitUpdate;
        }
    }

    // used in PopulationManager
    public int Compare(Job x, Job y)
    {
        float xv = GetJobDesire(x);
        float yv = GetJobDesire(y);
        if (xv > yv)
            return -1;
        if (xv < yv)
            return 1;
        else
            return 0;
    }

    float GetJobDesire(Job job)
    {
        float desire = StorageHelper.GetSum((type) =>
            job.JobProduction.GetResourceAmmount(type) * GetResourceDesire(type));
        return desire;
    }
    float GetResourceDesire(ResourceType resource)
    {
        float value = 0;
        switch (resource)
        {
            case ResourceType.food:
                value = FoodDesire;
                break;
            case ResourceType.wood:
                value = WoodDesire;
                break;
            case ResourceType.stone:
                value = StoneDesire;
                break;
        }
        return value;
    }
}
