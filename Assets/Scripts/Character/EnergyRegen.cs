using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using UnityEngine;

public class EnergyRegen : BaseObject
{
    public int RegenAmount;
    public float IntervalInMs;

    private Player _player;
    protected override void UnityAwake()
    {
        base.UnityAwake();
        _player = Player.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Regen());
    }

    private IEnumerable<IEnumerable<Action>> Regen()
    {
        while (isActiveAndEnabled)
        {
            _player.Stats.RestoreEnergy(RegenAmount);
            yield return TimeYields.WaitMilliseconds(GameTimer, IntervalInMs);
        }
    }
}