using UnityEngine;

[CreateAssetMenu(fileName = "New Building Stats", menuName = "Building Stats")]
public class BuildingStats : ScriptableObject
{
    [System.Obsolete("Use Methods!", false)]
    public int popCapRise = 0;
    [Space]
    [System.Obsolete("Use Methods!", false)]
    public JobType jobType = JobType.no;
    [System.Obsolete("Use Methods!", false)]
    public int jobsCount = 0;

    public int GetPopCapRise => popCapRise;
    public JobType GetJobType => jobType;
    public int GetJobsCount => jobsCount;
}

public enum JobType
{
    no,
    farmer,
    forester,
    stoneMiner,
    soldier,
}

[System.Serializable]
public class Job
{
    [System.Obsolete("Use Methods!", false)]
    public JobType jobType;
    [System.Obsolete("Use Methods!", false)]
    public Person worker;
    [System.Obsolete("Use Methods!", false)]
    public Building workplace;

    public JobType JobType { get => jobType; private set => jobType = value; }
    public Person Worker { get => worker; private set => worker = value; }
    public Building Workplace { get => workplace; private set => workplace = value; }
    public bool NoWorker => Worker == null;

    public Job(Building building)
    {
        JobType = building.stats.jobType;
        Worker = null;
        Workplace = building;
    }

    public void EmployWorker(Person newWorker)
    {
        if (!NoWorker)
        {
            UnemployWorker();
        }
        Worker = newWorker;
        Worker.Employ(this);
    }
    public void UnemployWorker()
    {
        if (NoWorker) return;
        Worker.Unemploy();
        Worker = null;
    }
}

[System.Serializable]
public class Employment
{
    [System.Obsolete("Use Methods!", false)]
    public bool worksHere = false;
    [System.Obsolete("Use Methods!", false)]
    public Job job;

    public bool WorksHere { get => worksHere; private set => worksHere = value; }
    public Job Job { get => job; private set => job = value; }

    public bool IsUnemployed => !WorksHere;

    public void Employ(Job job)
    {
        WorksHere = true;
        Job = job;
    }
    public void Unemploy()
    {
        WorksHere = false;
    }
}