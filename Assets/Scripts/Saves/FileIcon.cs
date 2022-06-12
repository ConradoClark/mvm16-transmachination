using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileIcon : MonoBehaviour
{
    public FileSelect FileSelect;
    public ScriptableTrigger Trigger;
    public SpriteRenderer Icon;

    private void OnEnable()
    {
        Icon.enabled = FileSelect.SaveFile.Triggers.FirstOrDefault(t => t.Trigger == Trigger).Enabled;
    }
}
