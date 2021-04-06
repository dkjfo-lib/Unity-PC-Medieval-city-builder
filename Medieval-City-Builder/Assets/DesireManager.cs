using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireManager :
    BaseSingleton<DesireManager>,
    IComparer<Job>
{
    PopulationManager population => PopulationManager.GetInstance;
    ResourceManager resources => ResourceManager.GetInstance;
    public int FoodPerDay => population.pplCount * resources.PERSON_FOOD_CONSUMPTION_PER_DAY;
    public float FoodDays => FoodPerDay == 0 ? 0 :
        (float)resources.GetResourceAmmount(ResourceType.food) / FoodPerDay;

    public int Compare(Job x, Job y)
    {
        if (GetJobDesire(x) > GetJobDesire(y))
            return 1;
        if (GetJobDesire(x) < GetJobDesire(y))
            return -1;
        else
            return 0;
    }

    float GetJobDesire(Job job)
    {
        // sun(job resource production * resource desirability)
        return 1;
    }
    float GetResourceDesire(ResourceType resource)
    {
        // sun(job resource production * resource desirability)
        return 1;
    }
}
