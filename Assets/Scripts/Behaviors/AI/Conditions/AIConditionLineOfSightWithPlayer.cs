using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Extensions;
using UnityEngine;

public class AIConditionLineOfSightWithPlayer : AICondition
{
    public Vector2 Offset;
    public Vector2 Direction;
    public ContactFilter2D ContactFilter;

    private readonly RaycastHit2D[] _results = new RaycastHit2D[10];

    public override bool CheckCondition()
    {
        var rayCast = Physics2D.Raycast((Vector2)transform.position + Offset, Direction, ContactFilter, _results);
        return rayCast > 0 && Player.Instance().LayerMask.Contains(_results[0].collider.gameObject.layer);
    }
}
