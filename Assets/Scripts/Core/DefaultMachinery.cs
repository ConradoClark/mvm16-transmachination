using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultMachinery : SceneObject<DefaultMachinery>
{
    public BasicMachineryScriptable MachineryRef;

    public static BasicMachinery<object> GetDefaultMachinery()
    {
        return Instance().MachineryRef.Machinery;
    }
}
