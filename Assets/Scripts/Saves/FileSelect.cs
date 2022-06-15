using UnityEngine;

[DefaultExecutionOrder(-10)]
public class FileSelect : MonoBehaviour
{
    public SavePoint SaveFile;
    public SavePoint EmptySaveFile;
    public bool Debug;

    private void Awake()
    {
        if (Debug) return;
        var file = SavePoint.LoadFile(SaveFile.Slot);
        if (file == null)
        {
            var slot = SaveFile.Slot;
            SaveFile = EmptySaveFile;
            SaveFile.Slot = slot;
        }
        else SaveFile.Deserialize(file);
    }
}
