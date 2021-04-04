using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    const int MAX_HAPPINESS = 50;

    [System.Obsolete("Use Methods!", false)]
    public Building house;
    [System.Obsolete("Use Methods!", false)]
    public Employment employment;
    [System.Obsolete("Use Methods!", false)]
    public PersonState personState = PersonState.Nothing;
    [System.Obsolete("Use Methods!", false)]
    public int happiness = 0;

    public Building House { get => house; private set => house = value; }
    public Employment Employment { get => employment; private set => employment = value; }
    public PersonState PersonState { get => personState; private set => personState = value; }
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

    private CityManager city => CityManager.GetInstance;

    private IPersonRoutine doRest;
    private IPersonRoutine doWork;
    private IPersonRoutine goHome;
    private IPersonRoutine goToWork;

    private void Start()
    {
        StartCoroutine(DailyRoutine());
    }

    IEnumerator DailyRoutine()
    {
        while (true)
        {
            yield return DoRoutine(doWork);
            yield return DoRoutine(goHome);
            yield return DoRoutine(doRest);
            yield return DoRoutine(goToWork);
        }
    }
    IEnumerator DoRoutine(IPersonRoutine routine)
    {
        int waitToSkipTaskSec = 1;
        if (routine != null)
        {
            PersonState = routine.PersonState;
            yield return RoutineExecuter.Execute(routine);
        }
        else
        {
            PersonState = PersonState.Nothing;
            yield return new WaitForSeconds(waitToSkipTaskSec);
        }
    }

    public void Settle(Building newHome)
    {
        House = newHome;
        PersonRoutinesCreator.CreateHouseRoutines(this, House, out goHome, out doRest);
    }

    public void Unsettle()
    {
        goHome = null;
        doRest = null;
    }
    public void Employ(Job newJob)
    {
        Employment.Employ(newJob);
        PersonRoutinesCreator.CreateJobRoutines(this, newJob, out goToWork, out doWork);
    }
    public void Unemploy()
    {
        Employment.Unemploy();
        goToWork = null;
        doWork = null;
    }

    private void OnDestroy()
    {
        House.RemovePerson(this);
        city.RemovePerson(this);
    }
}

public enum PersonState
{
    Nothing,
    GoesToHouse,
    RestsAtHome,
    GoesToWork,
    Works,
}