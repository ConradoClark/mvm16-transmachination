using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableForm", menuName = "TM/Triggers/ScriptableForm", order = 1)]
public class ScriptableForm : ScriptableObject
{
    public CharacterForm Form;
    public enum CharacterForm
    {
        Robot,
        Human
    }
}
