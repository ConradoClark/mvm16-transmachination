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

public abstract class RoomObject : MonoBehaviour, IResettable, IInitializable, IActivable
{
    public virtual void PerformDestroy()
    {

    }
    public abstract bool PerformReset();
    public abstract void Initialize();
    public abstract bool Activate();

    public RoomScriptable CurrentRoom;
    protected BasicMachinery<object> DefaultMachinery;
    protected ITime GameTimer;
    protected Room Room;
    protected RoomExit.RoomExitEventArgs ActivationEvent;

    protected void Awake()
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
            gameObject.SetActive(false);
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

