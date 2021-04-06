using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    [System.Obsolete("Use Methods!", false)]
    public BuildingStats stats;
    [System.Obsolete("Use Methods!", false)]
    public int pplCount => ppl.Count;
    [System.Obsolete("Use Methods!", false)]
    public List<Person> ppl;
    [System.Obsolete("Use Methods!", false)]
    public List<Job> jobs;

    public BuildingStats Stats => stats;

    public List<Job> emptyJobs => jobs.Where((s) => s.NoWorker).ToList();
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
            yield return new WaitUntil(() => city.popCap > city.pplCount);
            while (pplCount < Stats.GetPopCapRise && city.popCap > city.pplCount)
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
        ppl.Add(person);
        person.Settle(this);
    }
    public void RemovePerson(Person person)
    {
        person.Unsettle();
        ppl.Remove(person);
    }
    public void KillAllPeople()
    {
        for (int i = pplCount - 1; i > -1; --i)
            Destroy(ppl[i].gameObject);
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
