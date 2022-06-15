using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

public class DefaultMachinery : SceneObject<DefaultMachinery>
{
    public BasicMachineryScriptable MachineryRef;

    public static BasicMachinery<object> GetDefaultMachinery()
    {
        return Instance().MachineryRef.Machinery;
    }
}
