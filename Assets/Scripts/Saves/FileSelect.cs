using UnityEngine;

[DefaultExecutionOrder(-10)]
public class FileSelect : MonoBehaviour
{
    public SavePoint SaveFile;
    public SavePoint EmptySaveFile;
    public int Slot;
    public bool Debug;

    private void Awake()
    {
        if (Debug) return;
        var file = SavePoint.LoadFile(SaveFile.Slot);
        if (file == null)
        {
            SaveFile = EmptySaveFile;
            SaveFile.Slot = Slot;
        }
        else SaveFile.Deserialize(file, Slot);
    }
}
