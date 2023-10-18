using System.Collections.Generic;
using UnityEngine;

public class Enableable : MonoBehaviour
{
    [SerializeField] private List<Animator> animators;
    [SerializeField] private string trigger;

    private void OnEnable()
    {
        print(gameObject.name);
        foreach (var animator in animators)
            animator.SetTrigger(trigger);
    }
}