using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameSpawn : MonoBehaviour
{
    public SavePoint SavePoint;

    private void Awake()
    {
        SavePoint.Spawn();
    }
}
