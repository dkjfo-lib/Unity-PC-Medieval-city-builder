using UnityEngine;

public class MyDebug : Debug
{
    public static MessageTheme messageTheme { get; set; }
    public static void Log(object message, MessageTheme messageTheme, Object context = null)
    {
        if (messageTheme == MessageTheme.GameError)
        {
            if (context == null)
                LogError(message);
            else
                LogError(message, context);
            return;
        }
        if (context == null)
            Log(message);
        else
            Log(message, context);
    }
}

public class DebugSettings
{
    [SerializeField]
    private MessageTheme messageTheme;
    public MessageTheme MessageTheme
    {
        get => MyDebug.messageTheme;
        set => MyDebug.messageTheme = value;
    }
}

public enum MessageTheme
{
    GameEvent,
    GameLoop,
    GameError,
}
