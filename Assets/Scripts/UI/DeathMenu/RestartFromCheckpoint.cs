using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;

public class RestartFromCheckpoint : UIAction
{
    private DeathScreen _death;
    private Player _player;
    private GameSpawn _spawn;



    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        _player.UnblockAllMovement(_death.DeathEffects);
        _spawn.SpawnFromCheckpoint();
        _player.Stats.ResetHitPoints();

        yield return _death.DeathEffects.Respawn().AsCoroutine();
    }

    public override void OnSelect(bool manual)
    {
    }

    public override void OnDeselect()
    {
    }

    public override void OnInit()
    {
        _death = DeathScreen.Instance();
        _player = Player.Instance();
        _spawn = GameSpawn.Instance();
        
    }
}
