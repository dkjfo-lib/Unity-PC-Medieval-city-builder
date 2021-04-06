using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    private static PopulationManager city => PopulationManager.GetInstance;
    const int MAX_HAPPINESS = 50;

    [System.Obsolete("Use Methods!", false)]
    public Building house;
    [System.Obsolete("Use Methods!", false)]
    public Employment employment;
    [System.Obsolete("Use Methods!", false)]
    public PersonState personState = PersonState.Nothing;
    [System.Obsolete("Use Methods!", false)]
    public int happiness = 0;

    public float laziness { get; private set; }

    public Building House { get => house; private set => house = value; }
    public Employment Employment { get => employment; private set => employment = value; }
    public PersonState PersonState { get => personState; set => personState = value; }
    public int Happiness
    {
        get => happiness;
        set
        {
            happiness = value;
            happiness = Mathf.Clamp(happiness, -MAX_HAPPINESS, MAX_HAPPINESS);
        }
    }
    public bool IsUnemployed => Employment.IsUnemployed;


    private IPersonRoutine doRest;
    private IPersonRoutine doWork;

    private void Start()
    {
        laziness = Random.Range(0, .1f);
        StartCoroutine(DailyRoutine());
    }

    IEnumerator DailyRoutine()
    {
        while (true)
        {
            yield return RoutineExecuter.Execute(this, doRest);
            yield return RoutineExecuter.Execute(this, doWork);
        }
    }

    public void Settle(Building newHome)
    {
        House = newHome;
        doRest = PersonRoutinesCreator.CreateHouseRoutines(this, House);
    }

    public void Unsettle()
    {
        doRest = null;
    }
    public void Employ(Job newJob)
    {
        Employment.Employ(newJob);
        doWork = PersonRoutinesCreator.CreateJobRoutines(this, newJob);
    }
    public void Unemploy()
    {
        Employment.Unemploy();
        doWork = null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        House.RemovePerson(this);
        city.RemovePerson(this);
    }
}

public enum PersonState
{
    Nothing,
    GoesToHouse,
    RestsAtHome,
    Goes,
    Works,
}