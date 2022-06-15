using Licht.Unity.Objects;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : SceneObject<GameCamera>
{
    public Camera Camera { get; private set; }
    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }
}
