using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireManager : BaseSingleton<DesireManager>
{
    [System.Obsolete("Use Methods!", false)]
    public float foodDesire;
    [System.Obsolete("Use Methods!", false)]
    public float woodDesire;
    [System.Obsolete("Use Methods!", false)]
    public float stoneDesire;
    [Space]
    [System.Obsolete("Use Methods!", false)]
    public float distanceImpact = .2f;
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
    public float DistanceImpact => distanceImpact;

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

    public float GetResourceDesire(ResourceType resource)
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

public class JobChooser : IComparer<Job>
{
    public Person targetPerson { get; set; }

    private DesireManager desire => DesireManager.GetInstance;

    // used in PopulationManager when choosing jobs
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

    public float GetJobDesire(Job job)
    {
        float resourceDesire = StorageHelper.GetSum((type) =>
            job.JobProduction.GetResourceAmmount(type) * desire.GetResourceDesire(type));
        float distanceDesire = (job.Workplace.transform.position - targetPerson.transform.position).sqrMagnitude;
        return resourceDesire - distanceDesire * desire.DistanceImpact * desire.DistanceImpact;
    }
}
