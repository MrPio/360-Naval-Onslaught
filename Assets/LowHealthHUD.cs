using UnityEngine;

public class LowHealthHUD : MonoBehaviour
{
    private const float LowHealthLimit = 0.25f;
    [SerializeField] private CanvasGroup canvasGroup;

    // Set the alpha according to the player's current health
    public void Evaluate(float health)
    {
        if (health < LowHealthLimit)
            canvasGroup.alpha = health * (1f - health * (100f / LowHealthLimit));
        else
            canvasGroup.alpha = 0;
    }

    private void Start()
    {
        Evaluate(1f);
    }
}