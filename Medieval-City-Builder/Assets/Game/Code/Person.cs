using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Building house;
    public Employment employment;
    private CityManager city => CityManager.GetInstance;
    public bool IsUnemployed => employment.IsUnemployed;

    public void Employ(Job job)
    {
        employment.Employ(job);
    }
    public void Unemploy()
    {
        employment.Unemploy();
    }

    private void OnDestroy()
    {
        house.RemovePerson(this);
        city.RemovePerson(this);
    }
}
