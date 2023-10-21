using System;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField] private bool withParent = false;
    [NonSerialized] public bool Condition = true;

    public void End()
    {
        if (Condition)
            Destroy(withParent ? transform.parent.gameObject : gameObject);
    }
}