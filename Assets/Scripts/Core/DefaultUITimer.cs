using Licht.Interfaces.Time;
using Licht.Unity.Objects;

internal class DefaultUITimer : SceneObject<DefaultUITimer>
{
    public TimerScriptable TimerRef;

    public static ITime GetTimer()
    {
        return Instance().TimerRef.Timer;
    }
}

