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

    public int popCap => bldngs.Sum(s => s.Stats.GetPopCapRise);
    public int pplCount => bldngs.Sum(s => s.pplCount);
    public Job[] emptyJobs => bldngs.SelectMany(s => s.emptyJobs).ToArray();
    public Person[] unemployedPpl => ppl.Where(s => s.IsUnemployed).ToArray();

    private void Start()
    {
        StartCoroutine(ManageUnemplyment());
    }

    IEnumerator ManageUnemplyment()
    {
        int checkUnemploymentEverySec = 2;
        var waitForUpdate = new WaitForSeconds(checkUnemploymentEverySec);
        while (true)
        {
            Job[] _emptyJobs = emptyJobs;
            Person[] _unemployedPpl = unemployedPpl;
            int ableToEmploy = Mathf.Min(_emptyJobs.Length, _unemployedPpl.Length);
            for (int i = 0; i < ableToEmploy; i++)
            {
                System.Array.Sort<Job>(_emptyJobs, DesireManager.GetInstance);
                _emptyJobs[i].EmployWorker(_unemployedPpl[i]);
            }
            yield return waitForUpdate;
        }
    }

    public void AddBuilding(Building building)
    {
        bldngs.Add(building);
    }
    public void RemoveBuilding(Building building)
    {
        bldngs.Remove(building);
    }
    public void AddPerson(Person person)
    {
        ppl.Add(person);
    }
    public void RemovePerson(Person person)
    {
        ppl.Remove(person);
    }
}
