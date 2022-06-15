using UnityEngine;

public class WanderingEye : RoomObject
{
    public SpriteRenderer Sprite;
    public Animator Animator;

    public override bool Activate()
    {
        Sprite.flipX = Random.value > 0.5f;
        Sprite.flipY = Random.value > 0.5f;
        if (isActiveAndEnabled)
        {
            Animator.Play(0, 0, Random.value);
            Animator.speed = 0.9f + Random.value * 0.2f;
        }
        return true;
    }

    public override void Initialize()
    {
    }

    public override bool PerformReset()
    {
        return true;
    }
}
