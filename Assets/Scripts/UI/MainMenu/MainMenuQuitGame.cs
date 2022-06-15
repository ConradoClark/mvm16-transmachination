using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuQuitGame: MainMenuUIButton
{
    public override bool IsBlocked => Application.platform == RuntimePlatform.WebGLPlayer;

    protected override void UnityAwake()
    {
        if (IsBlocked)
        {
            gameObject.SetActive(false);
        }
    }

    public override IEnumerable<IEnumerable<Action>> OnAction()
    {
        Application.Quit();
        yield break;
    }
}
