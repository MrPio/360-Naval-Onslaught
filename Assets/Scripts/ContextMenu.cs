using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler,
    IPointerClickHandler
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
        TurretRate,
        NextWave,
        CloseHowToPlay,
        MainMenuPlay,
        MainMenuHowToPlay,
        GameOverGotoMainMenu,
        OpenSpecialsMenu,
        CloseSpecialsMenu,
        BuySpecial,
        ClosePauseMenu,
    }

    [SerializeField] private GameObject contextMenu, lockedContextMenu, canvas,mainBase;
    [SerializeField] private bool followMouse = true;
    [SerializeField] private ContextMenuType type;
    [SerializeField] private int turretIndex = -1, cannonIndex = -1, specialIndex = 0;
    private Transform _contextMenu;
    private static AudioClip _buy, _noBuy, _weaponSelect, _click, _hover;

    private void Awake()
    {
        if (_buy is null)
        {
            _buy = Resources.Load<AudioClip>("Audio/buy");
            _noBuy = Resources.Load<AudioClip>("Audio/no_buy");
            _weaponSelect = Resources.Load<AudioClip>("Audio/menu_weapon_select");
            _click = Resources.Load<AudioClip>("Audio/menu_click");
            _hover = Resources.Load<AudioClip>("Audio/menu_click_2");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MainCamera.AudioSource.PlayOneShot(_hover);

        if (contextMenu == null)
            return;

        void InstantiateContextMenu(GameObject menu)
        {
            _contextMenu = Instantiate(
                original: menu,
                position: (followMouse
                    ? (Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)
                    : transform.position) + Vector2.up * 0.215f,
                rotation: Quaternion.identity,
                canvas.transform
            ).transform;
        }

        if (contextMenu.name == "upgrade_context_menu")
        {
            InstantiateContextMenu(contextMenu);
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

                { ContextMenuType.BuySpecial, Game.SpecialsName[specialIndex < 0 ? 0 : specialIndex] },
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

                ContextMenuType.BuySpecial => 1,

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

                ContextMenuType.BuySpecial => Game.SpecialCost(specialIndex),

                _ => 0
            };
            _contextMenu.Find("title_text").GetComponent<TextMeshProUGUI>().text = $"{title[type]}:  +{increment}";
            _contextMenu.Find("money_text").GetComponent<TextMeshProUGUI>().text = cost.ToString("N0");
        }

        if (contextMenu.name == "turret_context_menu")
        {
            var turret = Data.Turrets[turretIndex];
            if (turret.IsLocked)
            {
                InstantiateContextMenu(lockedContextMenu);
                _contextMenu.Find("title_text").GetComponent<TextMeshProUGUI>().text = $"( {turret.Name} )";
                _contextMenu.Find("wave_text").GetComponent<TextMeshProUGUI>().text = turret.WaveUnlock.ToString();
            }
            else
            {
                InstantiateContextMenu(contextMenu);
                _contextMenu.Find("title").GetComponent<TextMeshProUGUI>().text = $"( {turret.Name} )";
                _contextMenu.Find("speed_text").GetComponent<TextMeshProUGUI>().text = turret.Speed.ToString("N0");
                _contextMenu.Find("ammo_text").GetComponent<TextMeshProUGUI>().text = turret.Ammo.ToString("N0");
                _contextMenu.Find("damage_text").GetComponent<TextMeshProUGUI>().text = turret.Damage.ToString("N0");
                _contextMenu.Find("rate_text").GetComponent<TextMeshProUGUI>().text = turret.Rate.ToString("N0");
                _contextMenu.Find("reload_text").GetComponent<TextMeshProUGUI>().text = turret.Reload.ToString("N0");
            }
        }

        if (contextMenu.name == "cannon_context_menu")
        {
            var cannon = Data.Cannons[cannonIndex];
            if (cannon.IsLocked)
            {
                InstantiateContextMenu(lockedContextMenu);
                _contextMenu.Find("title_text").GetComponent<TextMeshProUGUI>().text = $"( {cannon.Name} )";
                _contextMenu.Find("wave_text").GetComponent<TextMeshProUGUI>().text = cannon.WaveUnlock.ToString();
            }
            else
            {
                InstantiateContextMenu(contextMenu);
                _contextMenu.Find("title").GetComponent<TextMeshProUGUI>().text = $"( {cannon.Name} )";
                _contextMenu.Find("speed_text").GetComponent<TextMeshProUGUI>().text = cannon.Speed.ToString("N0");
                _contextMenu.Find("damage_text").GetComponent<TextMeshProUGUI>().text = cannon.Damage.ToString("N0");
                _contextMenu.Find("radius_text").GetComponent<TextMeshProUGUI>().text = cannon.Radius.ToString("N0");
                _contextMenu.Find("reload_text").GetComponent<TextMeshProUGUI>().text = cannon.Reload.ToString("N0");
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (contextMenu == null)
            return;
        Destroy(_contextMenu.gameObject);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (contextMenu == null) return;
        if (followMouse)
        {
            _contextMenu.transform.position =
                ((Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)) + Vector2.up * 0.3f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (type == ContextMenuType.NextWave)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().BeginWave();
        else if (type == ContextMenuType.CloseHowToPlay)
            GameObject.FindWithTag("how_to_play_menu").SetActive(false);
        else if (type == ContextMenuType.MainMenuPlay)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().overlay.SetActive(true);
        else if (type == ContextMenuType.MainMenuHowToPlay)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().howToPlayMenu.SetActive(true);
        else if (type == ContextMenuType.GameOverGotoMainMenu)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().RestartGame();
        else if (type == ContextMenuType.CloseSpecialsMenu)
            GameObject.FindWithTag("specials_menu").SetActive(false);
        else if (type == ContextMenuType.OpenSpecialsMenu)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().specialsMenu.SetActive(true);
        else if (type == ContextMenuType.ClosePauseMenu)
        {
            mainBase.SetActive(true);
            GameObject.FindWithTag("pause_menu").SetActive(false);
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().isPaused = false;
            foreach (var ship in GameObject.FindGameObjectsWithTag("ship"))
            {
                ship.GetComponent<Ship>().IsFreezed = false;
                ship.GetComponent<ShipPath>().IsFreezed = false;
            }
        }

        
        if (contextMenu == null)
        {
            MainCamera.AudioSource.PlayOneShot(_click);
            return;
        }


        if (contextMenu.name == "upgrade_context_menu")
        {
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

                ContextMenuType.BuySpecial => Game.SpecialCost(specialIndex),

                _ => 0
            };
            if (cost > Game.Money || cost == 0)
            {
                MainCamera.AudioSource.PlayOneShot(_noBuy);
                return;
            }

            switch (type)
            {
                case ContextMenuType.Repair:
                    Game.BuyRepair();
                    break;
                case ContextMenuType.UpgradeHealth:
                    Game.BuyHealth();
                    break;
                case ContextMenuType.CannonDamage:
                    Game.CurrentCannonModel.BuyDamage();
                    break;
                case ContextMenuType.CannonRadius:
                    Game.CurrentCannonModel.BuyRadius();
                    break;
                case ContextMenuType.CannonReload:
                    Game.CurrentCannonModel.BuyReload();
                    break;
                case ContextMenuType.CannonSpeed:
                    Game.CurrentCannonModel.BuySpeed();
                    break;
                case ContextMenuType.TurretDamage:
                    Game.CurrentTurretModel.BuyDamage();
                    break;
                case ContextMenuType.TurretAmmo:
                    Game.CurrentTurretModel.BuyAmmo();
                    break;
                case ContextMenuType.TurretRate:
                    Game.CurrentTurretModel.BuyRate();
                    break;
                case ContextMenuType.TurretReload:
                    Game.CurrentTurretModel.BuyReload();
                    break;
                case ContextMenuType.TurretSpeed:
                    Game.CurrentTurretModel.BuySpeed();
                    break;
                case ContextMenuType.BuySpecial:
                    Game.BuySpecial(specialIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MainCamera.AudioSource.PlayOneShot(_buy);
        }

        if (contextMenu.name == "turret_context_menu")
        {
            var turret = Data.Turrets[turretIndex];
            if (turret.IsLocked)
            {
                MainCamera.AudioSource.PlayOneShot(_noBuy);
            }
            else
            {
                Game.CurrentTurret = turretIndex;
                MainCamera.AudioSource.PlayOneShot(_weaponSelect);
            }
        }

        if (contextMenu.name == "cannon_context_menu")
        {
            var cannon = Data.Cannons[cannonIndex];
            if (cannon.IsLocked)
            {
                MainCamera.AudioSource.PlayOneShot(_noBuy);
            }
            else
            {
                Game.CurrentCannon = cannonIndex;
                MainCamera.AudioSource.PlayOneShot(_weaponSelect);
            }
        }

        // Update the context menu
        OnPointerExit(null);
        OnPointerEnter(null);

        // Update the shop menu
        GameObject.FindWithTag("shop_menu").GetComponent<ShopMenu>().UpdateUI();
        if (type == ContextMenuType.BuySpecial)
            GameObject.FindWithTag("specials_menu").GetComponent<SpecialsMenu>().UpdateUI();
    }
}