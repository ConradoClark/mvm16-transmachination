using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Pooling;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [Serializable]
    public struct EffectDefinition
    {
        public PrefabPool Pool;
        public string EffectName;
    }

    public EffectDefinition[] Effects;

    public PrefabPool GetEffect(string effectName)
    {
        return Effects.FirstOrDefault(eff => eff.EffectName == effectName).Pool;
    }

    public static EffectsManager GetInstance()
    {
        var obj = FindObjectOfType<EffectsManager>();
        return obj;
    }
}
