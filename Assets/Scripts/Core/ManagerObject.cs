using UnityEngine;
public class ManagerObject<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T Instance => FindObjectOfType<T>();
}

