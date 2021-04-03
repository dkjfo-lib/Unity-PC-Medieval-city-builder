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

    public Job(Building building)
    {
        jobType = building.stats.jobType;
        worker = null;
        workplace = building;
    }

    public void EmployWorker(Person newWorker)
    {
        if (!NoWorker)
        {
            UnemployWorker();
        }
        worker = newWorker;
        worker.Employ(this);
    }
    public void UnemployWorker()
    {
        if (NoWorker) return;
        worker.Unemploy();
        worker = null;
    }
    public bool NoWorker => worker == null;
}

[System.Serializable]
public class Employment
{
    [System.Obsolete("Use Methods!", false)]
    public bool worksHere = false;
    [System.Obsolete("Use Methods!", false)]
    public Job job;

    public void Employ(Job job)
    {
        worksHere = true;
        this.job = job;
    }
    public void Unemploy()
    {
        worksHere = false;
    }
    public bool IsUnemployed => !worksHere;
}