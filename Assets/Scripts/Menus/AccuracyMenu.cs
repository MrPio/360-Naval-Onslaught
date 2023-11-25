using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccuracyMenu : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;

    [SerializeField] private TextMeshProUGUI turretFiredText,
        turretHitText,
        turretAccuracyText,
        cannonFiredText,
        cannonHitText,
        cannonAccuracyText,closeButton;

    [SerializeField] private Image turretImage, cannonImage;

    private void OnEnable()
    {
        turretImage.sprite = Resources.Load<Sprite>(Game.CurrentTurretModel.Sprite);
        turretFiredText.text = Game.CurrentWaveTurretFired.ToString("N0");
        turretHitText.text = Game.CurrentWaveTurretHit.ToString("N0");
        turretAccuracyText.text = (Game.CurrentWaveTurretAccuracy * 100f).ToString("N0") + " %";
        turretAccuracyText.color = Game.HasTurretAccuracyBonus
            ? Color.yellow
            : new Color(0.1875f, 0.1875f, 0.1875f);

        cannonImage.sprite = Resources.Load<Sprite>(Game.CurrentCannonModel.Sprite);
        cannonFiredText.text = Game.CurrentWaveCannonFired.ToString("N0");
        cannonHitText.text = Game.CurrentWaveCannonHit.ToString("N0");
        cannonAccuracyText.text = (Game.CurrentWaveCannonAccuracy * 100f).ToString("N0") + " %";
        cannonAccuracyText.color = Game.HasCannonAccuracyBonus
            ? Color.yellow
            : new Color(0.1875f, 0.1875f, 0.1875f);

        closeButton.text = Game.HasAccuracyBonus ? "Redeem" : "Close";
    }
}