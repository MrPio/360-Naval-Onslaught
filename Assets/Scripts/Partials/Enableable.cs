using System.Collections.Generic;
using UnityEngine;

public class Enableable : MonoBehaviour
{
    [SerializeField] private List<Animator> animators = new();
    [SerializeField] private string trigger;

    private void OnEnable()
    {
        foreach (var animator in animators)
            animator.SetTrigger(trigger);
    }

    public void Disable() => gameObject.SetActive(false);
    public void Enable() => gameObject.SetActive(true);
}