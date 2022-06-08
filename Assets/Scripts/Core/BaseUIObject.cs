using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

public class BaseUIObject : MonoBehaviour
{
    protected BasicMachinery<object> DefaultMachinery;
    protected ITime UITimer;

    protected void Awake()
    {
        DefaultMachinery = global::DefaultMachinery.GetDefaultMachinery();
        UITimer = DefaultUITimer.GetTimer();
        UnityAwake();
    }

    protected virtual void UnityAwake()
    {

    }
}
