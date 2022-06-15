using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ClearGame : BaseObject
{
    public LayerMask CollisionLayerMask;
    public Collider2D Collider;
    private Collider2D[] _results;

    public Transform Ending;

    public ScriptableTrigger[] CompletionTriggers;

    public TMP_Text CompletionText;
    public BasicMachineryScriptable PhysicsMachinery;
    public BasicMachineryScriptable PostUpdateMachinery;

    private PlayerInput _playerInput;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _results = new Collider2D[1];
        _playerInput = PlayerInput.GetPlayerByIndex(0);
        DefaultMachinery.AddBasicMachine(HandleCollision());
    }

    private IEnumerable<IEnumerable<Action>> HandleCollision()
    {
        var filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = CollisionLayerMask,
        };

        while (Collider.OverlapCollider(filter, _results) <= 0)
        {
            if (!isActiveAndEnabled) yield break;
            yield return TimeYields.WaitMilliseconds(GameTimer, 200);
        }

        var completion = (CompletionTriggers.Count(t => t.Triggered) / (float)CompletionTriggers.Length) * 100;
        var rounded = Mathf.CeilToInt(completion);

        Ending.gameObject.SetActive(true);
        CompletionText.text = $"Clear Completion: {rounded}%";

        yield return TimeYields.WaitSeconds(GameTimer, 2);

        var any = _playerInput.actions["AnyKey"];
        while (!any.WasPerformedThisFrame())
        {
            yield return TimeYields.WaitOneFrameX;
        }

        DefaultMachinery.FinalizeWith(() =>
        {
            PhysicsMachinery.Machinery.FinalizeWith(() => { });
            PhysicsMachinery.Machinery.Update();
            PostUpdateMachinery.Machinery.FinalizeWith(() => { });
            PostUpdateMachinery.Machinery.Update();
            SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
        });
    }
}
