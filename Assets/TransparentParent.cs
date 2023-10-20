using System;
using System.Collections.Generic;
using UnityEngine;

public class TransparentParent : MonoBehaviour
{
    [SerializeField] private float alpha;
    private float _oldAlpha;
    [SerializeField] private List<SpriteRenderer> childrenSpriteRenderers;

    private void Update()
    {
        if (Math.Abs(alpha - _oldAlpha) > 0.0001)
        {
            SetAlpha(alpha);
            _oldAlpha = alpha;
        }
    }

    public void SetAlpha(float value)
    {
        foreach (var sprite in childrenSpriteRenderers)
        {
            var color = sprite.color;
            color.a = value;
            sprite.color = color;
        }
    }
}