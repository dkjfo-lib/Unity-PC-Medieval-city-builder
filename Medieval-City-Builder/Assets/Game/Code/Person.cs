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
    public int timeToRest = 5;
    [System.Obsolete("Use Methods!", false)]
    public int timeToWork = 20;
    [System.Obsolete("Use Methods!", false)]
    public int happiness = 0;

    public Building House { get => house; private set => house = value; }
    public Employment Employment { get => employment; private set => employment = value; }
    public PersonState PersonState { get => personState; private set => personState = value; }
    public int TimeToRest { get => timeToRest; }
    public int TimeToWork { get => timeToWork; }
    public int Happiness
    {
        get => happiness;
        private set
        {
            happiness = value;
            happiness = Mathf.Clamp(happiness, -MAX_HAPPINESS, MAX_HAPPINESS);
        }
    }
    public bool IsUnemployed => Employment.IsUnemployed;

    private CityManager city => CityManager.GetInstance;

    private IPersonRoutine restAtHomeRoutine;
    private IPersonRoutine workRoutine;
    private IPersonRoutine goToHouseRoutine;
    private IPersonRoutine goToWorkRoutine;

    private void Start()
    {
        StartCoroutine(DailyRoutine());
    }

    IEnumerator DailyRoutine()
    {
        while (true)
        {
            yield return DoRoutine(restAtHomeRoutine);
            yield return DoRoutine(goToWorkRoutine);
            yield return DoRoutine(workRoutine);
            yield return DoRoutine(goToHouseRoutine);
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
        goToHouseRoutine = new PersonRoutineGoTo(newHome, this, PersonState.GoesToHouse);
        restAtHomeRoutine = new PersonRoutineTimespan(TimeToRest, PersonState.RestsAtHome, () =>
        {
            if (ResourceManager.GetInstance.TryRemoveResource(ResourceType.food, 1))
            {
                Happiness += 10;
            }
            else
            {
                Happiness -= 10;
            }
        });
    }

    public void Unsettle()
    {
        goToHouseRoutine = null;
        restAtHomeRoutine = null;
    }
    public void Employ(Job newJob)
    {
        Employment.Employ(newJob);
        goToWorkRoutine = new PersonRoutineGoTo(newJob.Workplace, this, PersonState.GoesToWork);
        workRoutine = new PersonRoutineTimespan(TimeToWork, PersonState.Works, () => ResourceManager.GetInstance.AddResource(ResourceType.food, 5));
    }
    public void Unemploy()
    {
        Employment.Unemploy();
        goToWorkRoutine = null;
        workRoutine = null;
    }

    private void OnDestroy()
    {
        House.RemovePerson(this);
        city.RemovePerson(this);
    }
}

public interface IPersonRoutine
{
    PersonState PersonState { get; }
    bool IsDone { get; }
    System.Action OnFinish { get; }
    void Start();
}

public static class RoutineExecuter
{
    public static IEnumerator Execute(IPersonRoutine personRoutine)
    {
        personRoutine.Start();
        yield return new WaitUntil(() => personRoutine.IsDone);
        personRoutine.OnFinish?.Invoke();
    }
}

[System.Serializable]
public class PersonRoutineTimespan : IPersonRoutine
{
    public PersonState PersonState { get; }
    public bool IsDone => TimeDoing > TimeToDo;
    public System.Action OnFinish { get; }

    private int TimeToDo { get; }
    private float timeStartedAt;
    private float TimeDoing => Time.timeSinceLevelLoad - timeStartedAt;

    public PersonRoutineTimespan(int timeToDo, PersonState personState,
        System.Action doOnFinish = null)
    {
        TimeToDo = timeToDo;
        PersonState = personState;
        OnFinish = doOnFinish;
    }

    public void Start()
    {
        timeStartedAt = Time.timeSinceLevelLoad;
    }
}

[System.Serializable]
public class PersonRoutineGoTo : IPersonRoutine
{
    const float MIN_DIST = 1f;

    public bool IsDone => Vector3.SqrMagnitude(DeltaVector) < MIN_DIST * MIN_DIST;
    public PersonState PersonState { get; }
    public System.Action OnFinish { get; }

    public Building targetBuilding;
    private float speed = 5;
    private Person person;
    private Vector3 DeltaVector => targetBuilding.transform.position - person.transform.position;

    public PersonRoutineGoTo(Building building, Person person, PersonState personState,
        System.Action doOnFinish = null)
    {
        targetBuilding = building;
        this.person = person;
        PersonState = personState;
        OnFinish = doOnFinish;
    }

    public void Start()
    {
        person.StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (!IsDone)
        {
            Vector3 direction = DeltaVector.normalized;
            Vector3 movement = direction * speed * Time.deltaTime;
            person.transform.position += movement;
            yield return null;
        }
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