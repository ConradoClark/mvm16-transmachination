using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsChangeWindowMode : MainMenuUIButton
{
    public TMP_Text ModeText;

    private string _selectedMode = "FULLSCREEN";
    private string[] _modes = new[]
    {
        "FULLSCREEN",
        "WINDOW"
    };

    protected override void UnityAwake()
    {
        base.UnityAwake();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _selectedMode = "WEB";
        }
        else
            _selectedMode = Screen.fullScreenMode switch
            {
                FullScreenMode.Windowed => "WINDOW",
                _ => "FULLSCREEN"
            };

        ModeText.text = _selectedMode;
    }

    public override IEnumerable<IEnumerable<Action>> OnAction()
    {
        switch (_selectedMode)
        {
            case "WINDOW":
                _selectedMode = "FULLSCREEN";
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                ModeText.text = _selectedMode;
                break;
            case "FULLSCREEN":
                _selectedMode = "WINDOW";
                Screen.fullScreenMode = FullScreenMode.Windowed;
                ModeText.text = _selectedMode;
                break;
        }
        yield break;
    }
}
