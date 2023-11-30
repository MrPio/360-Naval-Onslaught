using System;
using System.Collections.Generic;
using ExtensionsFunctions;
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

    private enum ContextMenuType
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
        UpgradeCannonCriticalChance,
        PowerUp,
        UpgradePowerUpSpawnChance,
        PlayHowToPlay,
    }

    [SerializeField] private GameObject contextMenu, lockedContextMenu, mainBase, mobileShopConfirm;
    [SerializeField] private bool followMouse = true;
    [SerializeField] private ContextMenuType type;
    [SerializeField] private int turretIndex = -1, cannonIndex = -1, specialIndex = 0, powerUpIndex = -1;
    private Transform _contextMenu;
    private static WaveSpawner _waveSpawner;
    private static AudioClip _buy, _noBuy, _weaponSelect, _click, _hover;

    private int CostMoney => type switch
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
        ContextMenuType.UpgradePowerUpSpawnChance => Game.PowerUpSpawnChanceCost,

        ContextMenuType.PowerUp => Data.PowerUps[powerUpIndex]
            .Select(it => it.IsLocked ? it.UnlockCost : it.UpgradeCost),


        _ => -1
    };

    private int CostDiamonds => type switch
    {
        ContextMenuType.PowerUp => Data.PowerUps[powerUpIndex]
            .Select(it => it.IsLocked ? it.UnlockDiamondCost : 0),
        _ => 0
    };


    private void Awake()
    {
        _buy ??= Resources.Load<AudioClip>("Audio/buy");
        _noBuy ??= Resources.Load<AudioClip>("Audio/no_buy");
        _weaponSelect ??= Resources.Load<AudioClip>("Audio/menu_weapon_select");
        _click ??= Resources.Load<AudioClip>("Audio/menu_click");
        _hover ??= Resources.Load<AudioClip>("Audio/menu_click_2");
        _waveSpawner ??= GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>();
    }

    private void OnEnable()
    {
        if (type is ContextMenuType.QualitySetting)
            GetComponentInChildren<TextMeshProUGUI>().text = "Quality: " + Game.QualityNames[Game.Quality];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MainCamera.AudioSource.PlayOneShot(_hover);

        if (contextMenu == null || CostMoney + CostDiamonds == 0)
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
                { ContextMenuType.UpgradePowerUpSpawnChance, "chance" },
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

                ContextMenuType.UpgradeCriticalFactor => Math.Round(Game.CriticalFactorStep() * 100, 1),
                ContextMenuType.UpgradeTurretCriticalChance => Math.Round(Game.TurretCriticalChanceStep * 100, 1),
                ContextMenuType.UpgradeCannonCriticalChance => Math.Round(Game.CannonCriticalChanceStep * 100, 1),
                ContextMenuType.UpgradePowerUpSpawnChance => Math.Round(Game.PowerUpSpawnChanceStep * 100, 1),

                _ => 0
            };


            _contextMenu.Find("title_text").GetComponent<TextMeshProUGUI>().text = $"{title[type]}:  +{increment}";
            _contextMenu.Find("money_text").GetComponent<TextMeshProUGUI>().text = CostMoney.ToString("N0");
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

        if (contextMenu.name == "power_up_context_menu")
        {
            var powerUp = Data.PowerUps[powerUpIndex];
            if (powerUp.IsLocked)
            {
                InstantiateContextMenu(lockedContextMenu);
                new Dictionary<string, string>
                    {
                        ["title"] = $"( {powerUp.Name} )",
                        ["description"] = powerUp.Description,
                        ["money_text"] = powerUp.UnlockCost.ToString("N0"),
                        ["diamond_text"] = powerUp.UnlockDiamondCost.ToString("N0"),
                    }
                    .ForEach((k, v) => _contextMenu.Find(k).GetComponent<TextMeshProUGUI>().text = v);
            }
            else
            {
                InstantiateContextMenu(contextMenu);
                new Dictionary<string, string>
                    {
                        ["title"] = $"( {powerUp.Name} )",
                        ["description"] = powerUp.Description,
                        ["money_text"] = powerUp.UpgradeCost.ToString("N0"),

                        ["level_old"] = (powerUp.Level + 1).ToString("N0"),
                        ["strength_old"] = powerUp.Strength.ToString("N2") + "x",
                        ["duration_old"] = powerUp.HasDuration ? powerUp.Duration.ToString("N0") + "s" : "-",

                        ["level_new"] = (powerUp.Level + 2).ToString("N0"),
                        ["strength_new"] =
                            (powerUp.Strength + powerUp.BaseStrength * powerUp.StrengthStepFactor).ToString("N2") + "x",
                        ["duration_new"] =
                            powerUp.HasDuration
                                ? (powerUp.Duration + powerUp.BaseDuration * powerUp.DurationStepFactor)
                                .ToString("N0") + "s"
                                : "-",
                    }
                    .ForEach((k, v) => _contextMenu.Find(k).GetComponent<TextMeshProUGUI>().text = v);
                // if(powerUp.Level>=powerUp.MaxLevel) 
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (contextMenu == null || CostMoney + CostDiamonds == 0 || _contextMenu.IsDestroyed())
            return;
        Destroy(_contextMenu.gameObject);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (contextMenu == null) return;
        if (followMouse)
            _contextMenu.transform.position =
                ((Vector2)MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition)) +
                Vector2.up * (InputManager.IsMobile ? 1f : 0.3f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (type == ContextMenuType.NextWave)
            _waveSpawner.BeginWave();
        else if (type == ContextMenuType.PlayHowToPlay)
            _waveSpawner.NewGame();
        else if (type == ContextMenuType.MainMenuPlay)
            _waveSpawner.howToPlayMenu.Apply(it =>
            {
                it.SetActive(true);
                it.GetComponent<HowToPlayMenu>().IsNewGame = true;
            });
        else if (type == ContextMenuType.MainMenuContinue)
            _waveSpawner.LoadGame();
        else if (type == ContextMenuType.MainMenuHowToPlay)
            _waveSpawner.howToPlayMenu.Apply(it =>
            {
                it.SetActive(true);
                it.GetComponent<HowToPlayMenu>().IsNewGame = false;
            });
        else if (type == ContextMenuType.GameOverGotoMainMenu)
            _waveSpawner.RestartGame();
        else if (type == ContextMenuType.CloseSpecialsMenu)
            GameObject.FindWithTag("specials_menu").SetActive(false);
        else if (type == ContextMenuType.OpenSpecialsMenu)
            _waveSpawner.specialsMenu.SetActive(true);
        else if (type == ContextMenuType.ClosePauseMenu)
        {
            mainBase.SetActive(true);
            GameObject.FindWithTag("pause_menu").SetActive(false);
            _waveSpawner.isPaused = false;
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
                _waveSpawner.bonusMenu.SetActive(true);
            else
                _waveSpawner.shopMenu.SetActive(true);
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
            _waveSpawner.shopMenu.SetActive(true);
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

        // If is something to buy
        if (contextMenu.name is "upgrade_context_menu" or "power_up_context_menu")
        {
            if (CostMoney > Game.Money || CostDiamonds > Game.Diamonds || CostMoney + CostDiamonds == 0)
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
                        Data.Save();
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
                    case ContextMenuType.UpgradePowerUpSpawnChance:
                        Game.BuyPowerUpSpawnChance();
                        break;
                    case ContextMenuType.PowerUp:
                        Data.PowerUps[powerUpIndex].Apply(it =>
                        {
                            if (it.IsLocked)
                                it.Unlock();
                            else
                                it.Upgrade();
                        });
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