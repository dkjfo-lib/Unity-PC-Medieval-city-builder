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

    public int DayLength => timeForDay + timeForNight;
    public bool IsNight => Time.timeSinceLevelLoad % DayLength > timeForDay;

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
            yield return waitForUpdate;
        }
    }

    private void UpdateDaytime()
    {
        float timeOfDay = Time.timeSinceLevelLoad % DayLength;
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
