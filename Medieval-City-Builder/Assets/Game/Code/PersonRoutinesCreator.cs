using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersonRoutinesCreator
{
    private static DaytimeManager Daytime => DaytimeManager.GetInstance;

    const int TimeToWork = 20;
    const int TimeToRest = 5;

    const int FarmerFoodCreation = 5;
    const int MinerStoneCreation = 5;
    const int ForesterWoodCreation = 5;

    const int OnRestHappinessChange = 10;

    public static IPersonRoutine CreateHouseRoutines(Person person, Building house)
    {
        IPersonRoutine goHome = new PersonRoutineGoTo(() => true, house, person, PersonState.Goes);
        IPersonRoutine doRest = new PersonRoutineUntilCondition(
            startCondition: () => !Daytime.IsDay, 
            finishCondition: () => house == null || Daytime.IsDayWL(person.laziness),
            PersonState.RestsAtHome, GetOnFinishResting(person));
        return new PersonRoutineGoToDo(() => goHome.CanStart() && doRest.CanStart(), person, goHome, doRest, null);
    }
    public static IPersonRoutine CreateJobRoutines(Person person, Job job)
    {
        IPersonRoutine goToWork = new PersonRoutineGoTo(() => true, job.Workplace, person, PersonState.Goes);
        IPersonRoutine doWork = new PersonRoutineUntilCondition(
            startCondition: () => Daytime.IsDay, 
            finishCondition: () => job.Workplace == null || !Daytime.IsDayWL(person.laziness),
            PersonState.Works, GetOnWorkFinish(job.JobType));
        return new PersonRoutineGoToDo(() => goToWork.CanStart() && doWork.CanStart(), person, goToWork, doWork, null);
    }

    private static System.Action GetOnWorkFinish(JobType jobType)
    {
        System.Action onWorkFinish;
        switch (jobType)
        {
            case JobType.farmer:
                onWorkFinish = () => ResourceManager.GetInstance.AddResource(ResourceType.food, FarmerFoodCreation);
                break;
            case JobType.forester:
                onWorkFinish = () => ResourceManager.GetInstance.AddResource(ResourceType.wood, ForesterWoodCreation);
                break;
            case JobType.stoneMiner:
                onWorkFinish = () => ResourceManager.GetInstance.AddResource(ResourceType.stone, MinerStoneCreation);
                break;
            default:
                onWorkFinish = null;
                break;
        }
        return onWorkFinish;
    }

    private static System.Action GetOnFinishResting(Person person)
    {
        System.Action onRestingFinished = () =>
        {
            if (ResourceManager.GetInstance.TryRemoveResource(ResourceType.food, 1))
            {
                person.Happiness += OnRestHappinessChange;
            }
            else
            {
                person.Happiness -= OnRestHappinessChange;
            }
        };
        return onRestingFinished;
    }
}

public static class RoutineExecuter
{
    const int WaitToSkipTaskSec = 1;
    static WaitForSeconds waitIfNull = new WaitForSeconds(WaitToSkipTaskSec);

    public static IEnumerator Execute(Person person, IPersonRoutine routine)
    {
        bool? canDo = routine?.CanStart?.Invoke();
        if (canDo.HasValue && canDo.Value)
        {
            person.PersonState = routine.PersonState;
            yield return Execute(routine);
        }
        else
        {
            person.PersonState = PersonState.Nothing;
            yield return waitIfNull;
        }
    }

    private static IEnumerator Execute(IPersonRoutine personRoutine)
    {
        personRoutine.Start();
        yield return new WaitUntil(() => personRoutine.IsDone);
        personRoutine.OnFinish?.Invoke();
    }
}

public interface IPersonRoutine
{
    PersonState PersonState { get; }
    bool IsDone { get; }
    System.Func<bool> CanStart { get; }
    System.Action OnFinish { get; }
    void Start();
}

public class PersonRoutineGoToDo : IPersonRoutine
{
    public PersonState PersonState { get; } = PersonState.Nothing;
    public bool IsDone => GoThere.IsDone && DoThat.IsDone;
    public System.Func<bool> CanStart { get; }
    public System.Action OnFinish { get; }

    public IPersonRoutine GoThere { get; }
    public IPersonRoutine DoThat { get; }
    public Person Person { get; }

    public PersonRoutineGoToDo(System.Func<bool> startCondition,
        Person person, IPersonRoutine goThere, IPersonRoutine doWhat, System.Action doOnFinish = null)
    {
        CanStart = startCondition;
        OnFinish = doOnFinish;
        Person = person;
        GoThere = goThere;
        DoThat = doWhat;
    }

    public void Start()
    {
        Person.StartCoroutine(Execute());
    }

    private IEnumerator Execute()
    {
        yield return RoutineExecuter.Execute(Person, GoThere);
        yield return RoutineExecuter.Execute(Person, DoThat);
    }
}

[System.Serializable]
public class PersonRoutineTimespan : IPersonRoutine
{
    public PersonState PersonState { get; }
    public bool IsDone => TimeDoing > TimeToDo;
    public System.Func<bool> CanStart { get; }
    public System.Action OnFinish { get; }

    private int TimeToDo { get; }
    private float timeStartedAt;
    private float TimeDoing => Time.timeSinceLevelLoad - timeStartedAt;

    public PersonRoutineTimespan(System.Func<bool> startCondition, int timeToDo, PersonState personState,
        System.Action doOnFinish = null)
    {
        CanStart = startCondition;
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
public class PersonRoutineUntilCondition : IPersonRoutine
{
    public PersonState PersonState { get; }
    public bool IsDone => Condition.Invoke();
    public System.Func<bool> CanStart { get; }
    public System.Action OnFinish { get; }

    private System.Func<bool> Condition { get; }

    public PersonRoutineUntilCondition(System.Func<bool> startCondition, System.Func<bool> finishCondition,
        PersonState personState, System.Action doOnFinish = null)
    {
        CanStart = startCondition;
        Condition = finishCondition;
        PersonState = personState;
        OnFinish = doOnFinish;
    }

    public void Start() { }
}

[System.Serializable]
public class PersonRoutineGoTo : IPersonRoutine
{
    const float MIN_DIST = 1f;

    public bool IsDone => Vector3.SqrMagnitude(DeltaVector) < MIN_DIST * MIN_DIST;
    public PersonState PersonState { get; }
    public System.Func<bool> CanStart { get; }
    public System.Action OnFinish { get; }

    public Building targetBuilding;
    private float speed = 5;
    private Person person;
    public Vector3 TargetBuildingPosition => targetBuilding != null ? targetBuilding.transform.position : person.transform.position;
    private Vector3 DeltaVector => TargetBuildingPosition - person.transform.position;

    public PersonRoutineGoTo(System.Func<bool> startCondition, Building building, Person person,
        PersonState personState, System.Action doOnFinish = null)
    {
        CanStart = startCondition;
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

