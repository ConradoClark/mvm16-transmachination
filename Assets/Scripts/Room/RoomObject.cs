using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Interfaces.Update;
using UnityEngine;

public abstract class RoomObject : BaseObject, IResettable, IInitializable, IActivable
{
    public virtual void PerformDestroy()
    {

    }
    public abstract bool PerformReset();
    public abstract void Initialize();
    public abstract bool Activate();

    public RoomScriptable CurrentRoom;
    protected Room Room;
    protected RoomExit.RoomExitEventArgs ActivationEvent;

    protected override void UnityAwake()
    {
        DefaultMachinery = global::DefaultMachinery.GetDefaultMachinery();
        GameTimer = DefaultGameTimer.GetTimer();
        Room = GetComponentInParent<Room>();

        Initialize();

        if (Room.RoomDefinition != CurrentRoom.Value && gameObject.activeSelf) gameObject.SetActive(false);

        this.ObserveEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnRoomExit);
    }

    private void OnRoomExit(RoomExit.RoomExitEventArgs obj)
    {
        if (obj.Source.FromRoom == Room)
        {
            DefaultMachinery.AddBasicMachine(DeactivateAfterDelay());
            ActivationEvent = null;
            return;
        }
        if (obj.Source.ToRoom != Room) return;

        if (!gameObject.activeSelf)
        {
            ActivationEvent = obj;
            gameObject.SetActive(true);
        }
    }

    private IEnumerable<IEnumerable<Action>> DeactivateAfterDelay()
    {
        yield return TimeYields.WaitSeconds(GameTimer, 0.5f);
        if (Room.RoomDefinition != CurrentRoom.Value)
        {
            gameObject.SetActive(false);
        }
    }

    protected void OnDestroy()
    {
        PerformDestroy();
        this.StopObservingEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnRoomExit);
    }

    protected void OnEnable()
    {
        PerformReset();
        Activate();
    }

    public bool IsActive => isActiveAndEnabled;
}

