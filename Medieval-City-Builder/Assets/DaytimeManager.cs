using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaytimeManager : BaseSingleton<DaytimeManager>
{
    public Light lightDay;
    [Space]
    public Color colorDay;
    public Color colorNight;
    [Space]
    public int timeForDay = 45;
    public int timeForNight = 15;
    [Space]
    public float maxStrength = 2;
    public float minStrength = 0;
    [Space]
    public Vector2 startRotation = new Vector2(0, 90);
    public Vector2 endRotation = new Vector2(36, 270);
    [Space]
    public float timeFromTheCreation = 0;

    public int DayLength => timeForDay + timeForNight;

    public float DayPercent => (timeFromTheCreation / DayLength) % 1;
    public bool IsDay => DayPercent < NightAt;
    public bool IsDayWL(float laziness) => DayPercent > laziness && DayPercent < NightAt + laziness;
    public float NightAt => (float)timeForDay / DayLength;

    private float timeLastUpdate = 0;

    private void Start()
    {
        StartCoroutine(ManageDaytime());
    }

    private IEnumerator ManageDaytime()
    {
        float updateEvery = .1f;
        var waitForUpdate = new WaitForSeconds(updateEvery);
        while (true)
        {
            UpdateDaytime();
            timeLastUpdate = Time.timeSinceLevelLoad;
            yield return waitForUpdate;
        }
    }

    private void UpdateDaytime()
    {
        timeFromTheCreation += Time.timeSinceLevelLoad - timeLastUpdate;
        float timeOfDay = timeFromTheCreation % DayLength;
        if (timeOfDay > timeForDay)
        {
            //night
            lightDay.intensity = 0;
        }
        else
        {
            //day
            float dayPercentLinear = timeOfDay / timeForDay;
            float dayPercentCircular = dayPercentLinear - .5f;
            dayPercentCircular *= dayPercentCircular;
            dayPercentCircular = (.25f - dayPercentCircular) * 4;

            float rotationY = dayPercentLinear * (endRotation.y - startRotation.y) + startRotation.y;
            float rotationX = dayPercentCircular * (endRotation.x - startRotation.x) + startRotation.x;

            lightDay.intensity = dayPercentCircular * maxStrength;
            lightDay.color = Color.Lerp(colorNight, colorDay, dayPercentCircular);
            lightDay.transform.eulerAngles = new Vector3(
                rotationX,
                rotationY,
                transform.eulerAngles.z);
        }
    }
}
