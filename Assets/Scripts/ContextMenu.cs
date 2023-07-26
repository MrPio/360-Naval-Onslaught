using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private static GameManager Game => GameManager.Instance;
    private static DataManager Data => DataManager.Instance;

    enum ContextMenuType
    {
        Repair,
        UpgradeHealth,
        CannonSpeed,
        CannonDamage,
        CannonReload,
        CannonRadius,
        TurretSpeed,
        TurretDamage,
        TurretReload,
        TurretAmmo,
        TurretRate
    }

    [SerializeField] private GameObject contextMenu, canvas;
    [SerializeField] private bool followMouse = true;
    [SerializeField] private ContextMenuType type;
    [SerializeField] private int turretIndex = -1, cannonIndex = -1;
    private Transform _contextMenu;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _contextMenu = Instantiate(
            original: contextMenu,
            position: (followMouse
                ? (Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)
                : transform.position) + Vector2.up * 0.215f,
            rotation: Quaternion.identity,
            canvas.transform
        ).transform;
        if (contextMenu.name == "upgrade_context_menu")
        {
            var title = new Dictionary<ContextMenuType, string>
            {
                { ContextMenuType.Repair, "repair" },
                { ContextMenuType.UpgradeHealth, "hp" },

                { ContextMenuType.CannonDamage, "damage" },
                { ContextMenuType.CannonRadius, "radius" },
                { ContextMenuType.CannonReload, "reload" },
                { ContextMenuType.CannonSpeed, "speed" },

                { ContextMenuType.TurretDamage, "damage" },
                { ContextMenuType.TurretAmmo, "ammo" },
                { ContextMenuType.TurretRate, "rate" },
                { ContextMenuType.TurretReload, "reload" },
                { ContextMenuType.TurretSpeed, "speed" },
            };
            var increment = type switch
            {
                ContextMenuType.Repair => Game.MaxHealth - Game.Health,
                ContextMenuType.UpgradeHealth => Game.HealthStep,

                ContextMenuType.CannonDamage => Game.CurrentCannonModel.DamageLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentCannonModel.DamageLevel).ElementAt(0).Value,
                ContextMenuType.CannonRadius => Game.CurrentCannonModel.RadiusLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentCannonModel.RadiusLevel).ElementAt(0).Value,
                ContextMenuType.CannonReload => Game.CurrentCannonModel.ReloadLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentCannonModel.ReloadLevel).ElementAt(0).Value,
                ContextMenuType.CannonSpeed => Game.CurrentCannonModel.SpeedLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentCannonModel.SpeedLevel).ElementAt(0).Value,

                ContextMenuType.TurretDamage => Game.CurrentTurretModel.DamageLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentTurretModel.DamageLevel).ElementAt(0).Value,
                ContextMenuType.TurretAmmo => Game.CurrentTurretModel.AmmoLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentTurretModel.AmmoLevel).ElementAt(0).Value,
                ContextMenuType.TurretRate => Game.CurrentTurretModel.RateLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentTurretModel.RateLevel).ElementAt(0).Value,
                ContextMenuType.TurretReload => Game.CurrentTurretModel.ReloadLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentTurretModel.ReloadLevel).ElementAt(0).Value,
                ContextMenuType.TurretSpeed => Game.CurrentTurretModel.SpeedLevelSteps
                    .Where(entry => entry.Key >= Game.CurrentTurretModel.SpeedLevel).ElementAt(0).Value,

                _ => 0
            };

            var cost = type switch
            {
                ContextMenuType.Repair => Game.RepairCost,
                ContextMenuType.UpgradeHealth => Game.HealthCost,

                ContextMenuType.CannonDamage => Game.CurrentCannonModel.DamageCost,
                ContextMenuType.CannonRadius => Game.CurrentCannonModel.RadiusCost,
                ContextMenuType.CannonReload => Game.CurrentCannonModel.ReloadCost,
                ContextMenuType.CannonSpeed => Game.CurrentCannonModel.SpeedCost,

                ContextMenuType.TurretDamage => Game.CurrentTurretModel.DamageCost,
                ContextMenuType.TurretAmmo => Game.CurrentTurretModel.AmmoCost,
                ContextMenuType.TurretRate => Game.CurrentTurretModel.RateCost,
                ContextMenuType.TurretReload => Game.CurrentTurretModel.ReloadCost,
                ContextMenuType.TurretSpeed => Game.CurrentTurretModel.SpeedCost,

                _ => 0
            };
            _contextMenu.Find("title_text").GetComponent<TextMeshProUGUI>().text = $"{title[type]}:  +{increment}";
            _contextMenu.Find("money_text").GetComponent<TextMeshProUGUI>().text = cost.ToString("N0");
        }

        if (contextMenu.name == "turret_context_menu")
        {
            var turret = Data.Turrets[turretIndex];
            _contextMenu.Find("title").GetComponent<TextMeshProUGUI>().text = $"( {turret.Name} )";
            _contextMenu.Find("speed_text").GetComponent<TextMeshProUGUI>().text = turret.Speed.ToString("N0");
            _contextMenu.Find("ammo_text").GetComponent<TextMeshProUGUI>().text = turret.Ammo.ToString("N0");
            _contextMenu.Find("damage_text").GetComponent<TextMeshProUGUI>().text = turret.Damage.ToString("N0");
            _contextMenu.Find("rate_text").GetComponent<TextMeshProUGUI>().text = turret.Rate.ToString("N0");
            _contextMenu.Find("reload_text").GetComponent<TextMeshProUGUI>().text = turret.Reload.ToString("N0");
        }

        if (contextMenu.name == "cannon_context_menu")
        {
            var cannon = Data.Cannons[cannonIndex];
            _contextMenu.Find("title").GetComponent<TextMeshProUGUI>().text = $"( {cannon.Name} )";
            _contextMenu.Find("speed_text").GetComponent<TextMeshProUGUI>().text = cannon.Speed.ToString("N0");
            _contextMenu.Find("damage_text").GetComponent<TextMeshProUGUI>().text = cannon.Damage.ToString("N0");
            _contextMenu.Find("radius_text").GetComponent<TextMeshProUGUI>().text = cannon.Radius.ToString("N0");
            _contextMenu.Find("reload_text").GetComponent<TextMeshProUGUI>().text = cannon.Reload.ToString("N0");

        }
    }
    //funzioni di onclick da collegare da inspector + parametri

    public void OnPointerExit(PointerEventData eventData) =>
        Destroy(_contextMenu.gameObject);

    public void OnPointerMove(PointerEventData eventData)
    {
        if (followMouse)
        {
            _contextMenu.transform.position =
                ((Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)) + Vector2.up * 0.3f;
        }
    }
}