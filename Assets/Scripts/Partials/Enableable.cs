    using System.Collections.Generic;
using UnityEngine;

public class Enableable : MonoBehaviour
{
    [SerializeField] private List<Animator> animators;
    [SerializeField] private string trigger;

    private void OnEnable()
    {
        foreach (var animator in animators)
            animator.SetTrigger(trigger);
    }

    private void Disable() => gameObject.SetActive(false);
    private void Enable() => gameObject.SetActive(true);
}