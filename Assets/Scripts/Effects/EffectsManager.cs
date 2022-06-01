using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class EffectsManager : SceneObject<EffectsManager>
{
    [Serializable]
    public struct EffectDefinition
    {
        public PrefabPool Pool;
        public string EffectName;
    }

    public EffectDefinition[] Effects;
    public HitNumberPool HitNumberPool;

    public PrefabPool GetEffect(string effectName)
    {
        return Effects.FirstOrDefault(eff => eff.EffectName == effectName).Pool;
    }
}
