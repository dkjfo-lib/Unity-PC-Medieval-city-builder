using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
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

    public Building House { get => house; private set => house = value; }
    public Employment Employment { get => employment; private set => employment = value; }
    public PersonState PersonState { get => personState; private set => personState = value; }
    public bool IsUnemployed => Employment.IsUnemployed;

    private CityManager city => CityManager.GetInstance;

    public int TimeToRest { get => timeToRest; }
    public int TimeToWork { get => timeToWork; }

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
        // set correct current routine
        IPersonRoutine currentRoutine;
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
            routine.Start();
            yield return new WaitUntil(() => routine.IsDone);
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
        restAtHomeRoutine = new PersonRoutineTimespan(TimeToRest, PersonState.RestsAtHome);
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
        workRoutine = new PersonRoutineTimespan(TimeToWork, PersonState.Works);
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
    void Start();
}

[System.Serializable]
public class PersonRoutineTimespan : IPersonRoutine
{
    public PersonState PersonState { get; }
    public bool IsDone => TimeDoing > TimeToDo;

    private int TimeToDo { get; }
    private float timeStartedAt;
    private float TimeDoing => Time.timeSinceLevelLoad - timeStartedAt;

    public PersonRoutineTimespan(int timeToDo, PersonState personState)
    {
        TimeToDo = timeToDo;
        PersonState = personState;
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

    public Building targetBuilding;
    public bool IsDone => Vector3.SqrMagnitude(DeltaVector) < MIN_DIST * MIN_DIST;
    public PersonState PersonState { get; }

    private float speed = 5;
    private Person person;
    private Vector3 DeltaVector => targetBuilding.transform.position - person.transform.position;

    public PersonRoutineGoTo(Building building, Person person, PersonState personState)
    {
        targetBuilding = building;
        this.person = person;
        PersonState = personState;
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