using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private void End() => Destroy(transform.parent.gameObject);
}