using Licht.Interfaces.Time;
using Licht.Unity.Objects;

public class DefaultGameTimer : SceneObject<DefaultGameTimer>
{
    public TimerScriptable TimerRef;

    public static ITime GetTimer()
    {
        return Instance().TimerRef.Timer;
    }
}
