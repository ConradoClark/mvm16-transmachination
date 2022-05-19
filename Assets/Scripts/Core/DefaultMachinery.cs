using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class DefaultMachinery : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;

    public static BasicMachinery<object> GetDefaultMachinery()
    {
        var obj = FindObjectOfType<DefaultMachinery>();
        return obj.MachineryRef.Machinery;
    }
}
