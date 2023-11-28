using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Stack : MonoBehaviour
{
    private List<Transform> _children=new ();
    [SerializeField] private Transform target;
    [SerializeField] private float gap = 1, offset;
    [SerializeField] private bool vertical = true, useRectTransform, auto = false;

    [EditorCools.Button("Update")]
    public void UpdateUI()
    {
        // Retrieve first level children
        if (_children.Count == 0)
            _children = transform.GetComponentsInChildren<Transform>(true).ToList().Where(it => it.parent == transform)
                .ToList();

        // Update position according to the target to follow
        if (target)
            transform.position = target.position + (vertical ? Vector3.up : Vector3.right) * offset;

        // Arrange the children in a stack
        var located = 0;
        var activeChildren = _children.Where(child => child.gameObject.activeSelf).ToList();
        var rect = GetComponent<RectTransform>().rect;
        var step = (vertical ? rect.height : rect.width) / math.max(1, activeChildren.Count - 1);
        foreach (var child in activeChildren)
        {
            // Calculate child position
            var childPos = (vertical ? Vector3.up : Vector3.right) * ((auto ? step : gap) * located++);

            // Assign child position
            if (useRectTransform)
                child.GetComponent<RectTransform>().anchoredPosition = childPos;
            else
                child.transform.position = childPos;
        }
    }

    private void Start() => UpdateUI();
}