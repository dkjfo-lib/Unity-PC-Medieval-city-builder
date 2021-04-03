using UnityEngine;

public class FPS : MonoBehaviour
{
    [Range(30, 120)]
    public int fps = 30;

    void Awake()
    {
        SetFPS(fps);
    }

    public void SetFPS(int newFps)
    {
        fps = newFps;
        Application.targetFrameRate = newFps;
    }
}
