using System;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine.SceneManagement;

public class QuitToMainMenu : UIAction
{
    private Player _player;
    public BasicMachineryScriptable PhysicsMachinery;
    public BasicMachineryScriptable PostUpdateMachinery;

    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        _player.Stats.ResetHitPoints();
        
        DefaultMachinery.FinalizeWith(() =>
        {
            PhysicsMachinery.Machinery.FinalizeWith(() => { });
            PhysicsMachinery.Machinery.Update();
            PostUpdateMachinery.Machinery.FinalizeWith(() => { });
            PostUpdateMachinery.Machinery.Update();
            SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
        });
        yield break;
    }

    public override void OnSelect(bool manual)
    {
    }

    public override void OnDeselect()
    {
    }

    public override void OnInit()
    {
        _player = Player.Instance();
    }
}
