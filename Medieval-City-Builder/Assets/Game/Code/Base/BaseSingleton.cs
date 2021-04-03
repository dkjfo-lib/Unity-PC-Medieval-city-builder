using UnityEngine;

public class BaseSingleton<T> : MonoBehaviour
    where T : BaseSingleton<T>
{
    public static T GetInstance { get; private set; }
    private void Awake()
    {
        GetInstance = (T)this;
    }
}
