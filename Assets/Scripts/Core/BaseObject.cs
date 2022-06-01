using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    protected BasicMachinery<object> DefaultMachinery;
    protected ITime GameTimer;

    protected void Awake()
    {
        DefaultMachinery = global::DefaultMachinery.GetDefaultMachinery();
        GameTimer = DefaultGameTimer.GetTimer();
        UnityAwake();
    }

    protected virtual void UnityAwake()
    {

    }
}
