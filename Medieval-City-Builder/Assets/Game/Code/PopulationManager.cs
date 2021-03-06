using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : BaseSingleton<PopulationManager>
{
    [System.Obsolete("Use Methods!", false)]
    public List<Person> ppl;
    [System.Obsolete("Use Methods!", false)]
    public List<Building> bldngs;

    public List<Person> People => ppl;
    public List<Building> Buildings => bldngs;
    public int popCap => Buildings.Sum(s => s.Stats.GetPopCapRise);
    public int PplCount => Buildings.Sum(s => s.PplCount);
    public IEnumerable<Job> EmptyJobs => Buildings.SelectMany(s => s.EmptyJobs);
    public IEnumerable<Person> UnemployedPpl => People.Where(s => s.IsUnemployed);
    public IEnumerable<Person> EmployedPpl => People.Where(s => !s.IsUnemployed);

    private void Start()
    {
        StartCoroutine(ManageJobs());
    }

    IEnumerator ManageJobs()
    {
        float waitUpdateSec = 2;
        var waitUpdate = new WaitForSeconds(waitUpdateSec);
        float waitNextPersonSec = .5f;
        var waitNextPerson = new WaitForSeconds(waitNextPersonSec);
        JobChooser jobChooser = new JobChooser();
        while (true)
        {
            List<Job> _emptyJobs = EmptyJobs.ToList();
            List<Person> _ppl = People.ToList();
            foreach (var person in _ppl)
            {
                if (person == null)
                {
                    continue;
                }
                _emptyJobs.RemoveAll((s) => s.Workplace == null);
                if (_emptyJobs.Count == 0)
                {
                    break;
                }
                jobChooser.targetPerson = person;
                _emptyJobs.Sort(jobChooser);

                Job bestEmptyJob = _emptyJobs[0];
                if (person.Employment.IsUnemployed)
                {
                    bestEmptyJob.EmployWorker(person);
                    _emptyJobs.Remove(bestEmptyJob);
                }
                else
                {
                    if (jobChooser.GetJobDesire(person.Employment.Job) < jobChooser.GetJobDesire(bestEmptyJob))
                    {
                        _emptyJobs.Add(person.Employment.Job);
                        person.Employment.Job.UnemployWorker();
                        bestEmptyJob.EmployWorker(person);
                        _emptyJobs.Remove(bestEmptyJob);
                    }
                }
                yield return waitNextPerson;
            }
            yield return waitUpdate;
        }
    }

    public void AddBuilding(Building building)
    {
        Buildings.Add(building);
    }
    public void RemoveBuilding(Building building)
    {
        Buildings.Remove(building);
    }
    public void AddPerson(Person person)
    {
        People.Add(person);
    }
    public void RemovePerson(Person person)
    {
        People.Remove(person);
    }
}
