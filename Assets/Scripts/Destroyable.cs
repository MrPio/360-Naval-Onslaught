using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public void End() => Destroy(gameObject);
}
