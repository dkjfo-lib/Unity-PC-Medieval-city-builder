using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : BaseSingleton<CityManager>
{
    public List<Person> ppl;
    public List<Building> bldngs;
    public int popCap => bldngs.Sum(s => s.stats.GetPopCapRise);
    public int pplCount => bldngs.Sum(s => s.pplCount);
    public Job[] emptyJobs => bldngs.SelectMany(s => s.emptyJobs).ToArray();
    public Person[] unemployedPpl => ppl.Where(s => s.IsUnemployed).ToArray();

    private void Start()
    {
        StartCoroutine(HandleUnemplyment());
    }

    IEnumerator HandleUnemplyment()
    {
        int checkUnemploymentEverySec = 2;
        while (true)
        {
            yield return new WaitForSeconds(checkUnemploymentEverySec);
            Job[] _emptyJobs = emptyJobs;
            Person[] _unemployedPpl = unemployedPpl;
            int ableToEmploy = Mathf.Min(_emptyJobs.Length, _unemployedPpl.Length);
            for (int i = 0; i < ableToEmploy; i++)
            {
                _emptyJobs[i].EmployWorker(_unemployedPpl[i]);
            }
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
