using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    [System.Obsolete("Use Methods!", false)]
    public BuildingStats stats;
    [System.Obsolete("Use Methods!", false)]
    public List<Person> ppl;
    [System.Obsolete("Use Methods!", false)]
    public List<Job> jobs;

    public List<Person> Ppl => ppl;
    public int PplCount => Ppl.Count;
    public BuildingStats Stats => stats;
    public IResourceStorage JobProduction => Stats.BaseJobProduction;

    public List<Job> EmptyJobs => jobs.Where((s) => s.NoWorker).ToList();
    [Space]
    public Person personPrefab;
    private PopulationManager city => PopulationManager.GetInstance;

    private void Start()
    {
        city.AddBuilding(this);
        StartCoroutine(SpawnVills());
        CreateJobs();
    }

    IEnumerator SpawnVills()
    {
        int checkPopEverySec = 15;
        int spawnPopEverySec = 2;
        while (true)
        {
            yield return new WaitUntil(() => city.popCap > city.PplCount);
            while (PplCount < Stats.GetPopCapRise && city.popCap > city.PplCount)
            {
                CreatePerson();
                yield return new WaitForSeconds(spawnPopEverySec);
            }
            yield return new WaitForSeconds(checkPopEverySec + Random.Range(-5, 5));
        }
    }

    void CreateJobs()
    {
        for (int i = 0; i < Stats.GetJobsCount; i++)
        {
            jobs.Add(new Job(this));
        }
    }

    private void CreatePerson()
    {
        var newPerson = Instantiate(personPrefab, transform.position, Quaternion.identity, city.transform);
        AddPerson(newPerson);
        city.AddPerson(newPerson);
    }
    public void AddPerson(Person person)
    {
        Ppl.Add(person);
        person.Settle(this);
    }
    public void RemovePerson(Person person)
    {
        person.Unsettle();
        Ppl.Remove(person);
    }
    public void KillAllPeople()
    {
        for (int i = PplCount - 1; i > -1; --i)
            Destroy(Ppl[i].gameObject);
    }
    public void UnassignAllJobs()
    {
        foreach (var job in jobs)
        {
            job.UnemployWorker();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        city.RemoveBuilding(this);
        KillAllPeople();
        UnassignAllJobs();
    }
}
