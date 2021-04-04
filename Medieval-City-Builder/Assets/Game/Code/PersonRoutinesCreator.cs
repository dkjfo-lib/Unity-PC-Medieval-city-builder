using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersonRoutinesCreator
{
    const int TimeToWork = 20;
    const int TimeToRest = 5;

    const int FarmerFoodCreation = 5;
    const int MinerStoneCreation = 5;
    const int ForesterWoodCreation = 5;

    const int OnRestHappinessChange = 10;

    public static void CreateHouseRoutines(Person person, Building house, out IPersonRoutine goHome, out IPersonRoutine doRest)
    {
        goHome = new PersonRoutineGoTo(house, person, PersonState.GoesToHouse);
        doRest = new PersonRoutineTimespan(TimeToRest, PersonState.RestsAtHome, GetOnFinishResting(person));
    }
    public static void CreateJobRoutines(Person person, Job job, out IPersonRoutine goToWork, out IPersonRoutine doWork)
    {
        goToWork = new PersonRoutineGoTo(job.Workplace, person, PersonState.GoesToWork);
        doWork = new PersonRoutineTimespan(TimeToWork, PersonState.Works, GetOnWorkFinish(job.JobType));
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

