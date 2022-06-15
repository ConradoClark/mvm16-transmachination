using UnityEngine;

public abstract class AICondition : MonoBehaviour
{
    public bool Negate;
    public abstract bool CheckCondition();
}
