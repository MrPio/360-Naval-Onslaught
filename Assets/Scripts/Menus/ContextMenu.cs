using System;
using System.Collections.Generic;
using Managers;
using Model;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        CloseAccuracyMenu,
        BonusStop,
        BonusContinue,
        CloseGame,
        QualitySetting,
        MainMenuContinue,
        None,
        UpgradeCriticalFactor,
        UpgradeTurretCriticalChance,
        UpgradeCannonCriticalChance
    }

    [SerializeField] private GameObject contextMenu, lockedContextMenu, mainBase, mobileShopConfirm;
    [SerializeField] private bool followMouse = true;
    [SerializeField] private ContextMenuType type;
    [SerializeField] private int turretIndex = -1, cannonIndex = -1, specialIndex = 0;
    private Transform _contextMenu;
    private static AudioClip _buy, _noBuy, _weaponSelect, _click, _hover;

    private int Cost => type switch
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

        ContextMenuType.UpgradeCriticalFactor => Game.CriticalFactorCost,
        ContextMenuType.UpgradeTurretCriticalChance => Game.TurretCriticalChanceCost,
        ContextMenuType.UpgradeCannonCriticalChance => Game.CannonCriticalChanceCost,

        _ => 0
    };

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

    private void OnEnable()
    {
        if (type is ContextMenuType.QualitySetting)
            GetComponentInChildren<TextMeshProUGUI>().text = "Quality: " + Game.QualityNames[Game.Quality];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MainCamera.AudioSource.PlayOneShot(_hover);

        if (contextMenu == null || Cost == 0)
            return;

        void InstantiateContextMenu(GameObject menu)
        {
            _contextMenu = Instantiate(
                original: menu,
                position:
                (followMouse
                    ? (Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)
                    : transform.position) +
                Vector2.up * (InputManager.IsMobile ? 1f : 0.3f),
                rotation: Quaternion.identity,
                GameObject.FindWithTag("canvas").transform
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

                { ContextMenuType.UpgradeCriticalFactor, "damage" },
                { ContextMenuType.UpgradeTurretCriticalChance, "chance" },
                { ContextMenuType.UpgradeCannonCriticalChance, "chance" },
            };
            var increment = type switch
            {
                ContextMenuType.Repair => Game.MaxHealth - Game.Health,
                ContextMenuType.UpgradeHealth => Game.HealthStep,

                ContextMenuType.CannonDamage => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentCannonModel.Damage, 1)),
                ContextMenuType.CannonRadius => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentCannonModel.Radius, 1)),
                ContextMenuType.CannonReload => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentCannonModel.Reload, 1)),
                ContextMenuType.CannonSpeed => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentCannonModel.Speed, 1)),

                ContextMenuType.TurretDamage => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentTurretModel.Damage, 1)),
                ContextMenuType.TurretAmmo => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentTurretModel.Ammo, 1)),
                ContextMenuType.TurretRate => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentTurretModel.Rate, 1)),
                ContextMenuType.TurretReload => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentTurretModel.Reload, 1)),
                ContextMenuType.TurretSpeed => math.max(1,
                    Math.Round((TurretModel.UpgradeStep - 1f) * Game.CurrentTurretModel.Speed, 1)),

                ContextMenuType.BuySpecial => 1,

                ContextMenuType.UpgradeCriticalFactor => Math.Round(Game.CriticalFactorStep * 100, 1),
                ContextMenuType.UpgradeTurretCriticalChance => Math.Round(Game.TurretCriticalChanceStep * 100, 1),
                ContextMenuType.UpgradeCannonCriticalChance => Math.Round(Game.CannonCriticalChanceStep * 100, 1),

                _ => 0
            };


            _contextMenu.Find("title_text").GetComponent<TextMeshProUGUI>().text = $"{title[type]}:  +{increment}";
            _contextMenu.Find("money_text").GetComponent<TextMeshProUGUI>().text = Cost.ToString("N0");
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
        if (contextMenu == null || _contextMenu.IsDestroyed())
            return;
        Destroy(_contextMenu.gameObject);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (contextMenu == null) return;
        if (followMouse)
        {
            _contextMenu.transform.position =
                ((Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)) +
                Vector2.up * (InputManager.IsMobile ? 1f : 0.3f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (type == ContextMenuType.NextWave)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().BeginWave();
        else if (type == ContextMenuType.CloseHowToPlay)
            GameObject.FindWithTag("how_to_play_menu").SetActive(false);
        else if (type == ContextMenuType.MainMenuPlay)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().NewGame();
        else if (type == ContextMenuType.MainMenuContinue)
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().LoadGame();
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

            foreach (var bullet in GameObject.FindGameObjectsWithTag("bullet"))
                bullet.GetComponent<Bullet>().IsFrozen = false;
            foreach (var laser in GameObject.FindGameObjectsWithTag("laser"))
                laser.GetComponent<Laser>().IsFreezed = false;
        }
        else if (type == ContextMenuType.CloseAccuracyMenu)
        {
            if (Game.HasAccuracyBonus)
                GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().bonusMenu.SetActive(true);
            else
                GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().shopMenu.SetActive(true);
            GameObject.FindWithTag("accuracy_menu").SetActive(false);
        }
        else if (type == ContextMenuType.BonusStop)
        {
            type = ContextMenuType.BonusContinue;
            transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Continue";
            GameObject.FindWithTag("bonus_menu").GetComponent<BonusMenu>().Stop = true;
        }
        else if (type == ContextMenuType.BonusContinue)
        {
            type = ContextMenuType.BonusStop;
            GameObject.FindWithTag("bonus_menu").GetComponent<BonusMenu>().Redeem();
            GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().shopMenu.SetActive(true);
            GameObject.FindWithTag("bonus_menu").SetActive(false);
        }
        else if (type == ContextMenuType.CloseGame)
            Application.Quit();
        else if (type == ContextMenuType.QualitySetting)
        {
            Game.Quality = (Game.Quality + 1) % 3;
            GetComponentInChildren<TextMeshProUGUI>().text = "Quality: " + (Game.QualityNames[Game.Quality]);
            GameObject.FindWithTag("ocean").GetComponent<Renderer>().material =
                Resources.Load<Material>(Game.QualityOceanMaterials[Game.Quality]);
        }

        if (contextMenu == null)
        {
            MainCamera.AudioSource.PlayOneShot(_click);
            return;
        }

        if (contextMenu.name == "upgrade_context_menu")
        {
            if (Cost > Game.Money || Cost == 0)
            {
                MainCamera.AudioSource.PlayOneShot(_noBuy);
                return;
            }

            Action action = () =>
            {
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
                    case ContextMenuType.UpgradeCriticalFactor:
                        Game.BuyCriticalFactor();
                        break;
                    case ContextMenuType.UpgradeTurretCriticalChance:
                        Game.BuyTurretCriticalChance();
                        break;
                    case ContextMenuType.UpgradeCannonCriticalChance:
                        Game.BuyCannonCriticalChance();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                MainCamera.AudioSource.PlayOneShot(_buy);
                // Update the shop menu
                GameObject.FindWithTag("shop_menu").GetComponent<ShopMenu>().UpdateUI();
            };
            if (InputManager.IsMobile) //todo
            {
                mobileShopConfirm.SetActive(true);
                var script = mobileShopConfirm.GetComponent<MobileShopConfirmMenu>();
                script.Action = action;
                script.SetContent(_contextMenu.gameObject);
            }
            else
                action.Invoke();
        }

        if (contextMenu.name == "turret_context_menu")
        {
            if (Data.Turrets[turretIndex].IsLocked)
                MainCamera.AudioSource.PlayOneShot(_noBuy);
            else if (Game.CurrentTurret != turretIndex)
            {
                Game.CurrentTurret = turretIndex;
                MainCamera.AudioSource.PlayOneShot(_weaponSelect);
            }
        }

        if (contextMenu.name == "cannon_context_menu")
        {
            if (Data.Cannons[cannonIndex].IsLocked)
                MainCamera.AudioSource.PlayOneShot(_noBuy);
            else if (Game.CurrentCannon != cannonIndex)
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