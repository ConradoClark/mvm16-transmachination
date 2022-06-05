using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Pooling;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MinimapObject : EffectPoolable
{
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    public override void OnActivation()
    {

    }

    public override bool IsEffectOver { get; protected set; }

    public void SetActive(bool active)
    {
        _spriteRenderer.enabled = active;
    }

    public void SetRelevantPosition(bool condition)
    {
        _spriteRenderer.color = condition ? Color.red : _originalColor;
    }
}   

