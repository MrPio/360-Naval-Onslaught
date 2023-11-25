using Managers;
using UnityEngine;

public class Scalable : MonoBehaviour
{
    [SerializeField] [Range(0.5f, 2f)] private float mobileScale = 1.25f, noMobileScale = 1f;

    private void Start() =>
        transform.localScale *= InputManager.IsMobile ? mobileScale : noMobileScale;
}