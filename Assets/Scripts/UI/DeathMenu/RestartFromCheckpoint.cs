using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;

public class RestartFromCheckpoint : UIAction
{
    private DeathScreen _death;
    private Player _player;
    private GameSpawn _spawn;



    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        _player.Stats.ResetHitPoints();
        _player.UnblockAllMovement(_death.DeathEffects);
        _spawn.SpawnFromCheckpoint();

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
