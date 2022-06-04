using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.CharacterControllers;

public class BaseMovementController : LichtMovementController
{
    protected BasicMachinery<object> DefaultMachinery;
    protected ITime GameTimer;

    protected override void Awake()
    {
        base.Awake();
        DefaultMachinery = global::DefaultMachinery.GetDefaultMachinery();
        GameTimer = DefaultGameTimer.GetTimer();
        UnityAwake();
    }

    protected virtual void UnityAwake()
    {

    }
}