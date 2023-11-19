using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Managers;
using Model;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static TurretModel Model => Game.CurrentTurretModel;
    private static InputManager In => InputManager.Instance;

    [SerializeField] private GameObject leftSpawnPoint, rightSpawnPoint, bullet, laser;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public ReloadBar reloadBar;
    [SerializeField] private AudioClip reloadStart, reloadMiss, reloadFinish, noAmmo;
    [SerializeField] private AudioSource bulletAudioSource;
    [SerializeField] private List<AudioClip> clatchAudioClips;
    [SerializeField] private Animator overrideAnimator;
    private AmmoCounter _ammoCounter;
    private float _fireAccumulator = 9999f, _lastFireTriggered;
    private AudioClip _fireClip;
    private bool _isLaserFiring, _lastFireInput;
    private int _fireCount = 0;

    private void OnEnable()
    {
        spriteRenderer.sprite = Resources.Load<Sprite>(Model.Sprite);
        _fireClip = Resources.Load<AudioClip>(Model.FireClip);
        _ammoCounter = GameObject.FindWithTag("ammo_counter").GetComponent<AmmoCounter>();
    }

    private void Update()
    {
        // Rotate with mouse movement
        transform.rotation = In.GetInput().ToQuaternion();

        var currentFireInput = In.GetTurret();
        _fireAccumulator += Time.deltaTime;
        if (currentFireInput && !_lastFireInput && Game.Ammo <= 0)
            MainCamera.AudioSource.PlayOneShot(noAmmo);
        if (currentFireInput && Game.Ammo > 0 && _fireAccumulator > 100f / Model.Rate && !reloadBar.IsReloading &&
            (Model.Name != "Laser Gun" || !_isLaserFiring))
        {
            _fireAccumulator = 0;
            ++_fireCount;
            if (Game.CurrentTurret == 2)
            {
                Fire(Game.Ammo % 2 == 0 ? leftSpawnPoint : rightSpawnPoint);
            }
            else
            {
                Fire(leftSpawnPoint);
                Fire(rightSpawnPoint);
            }

            bulletAudioSource.PlayOneShot(_fireClip);
        }

        // Button DOWN
        if (!_lastFireInput && currentFireInput)
        {
            if (!Game.HasOverride && Time.time - _lastFireTriggered < 0.2)
                Override(true);
            _lastFireTriggered = Time.time;
        }

        // Button UP
        if (_lastFireInput && !currentFireInput)
        {
            Override(false);
            if (_isLaserFiring)
                bulletAudioSource.Stop();
            _isLaserFiring = false;
            _fireAccumulator += 99999f;
            if (_fireCount > 4 && new[] { 0, 1, 4 }.Contains(Game.CurrentTurret))
                bulletAudioSource.PlayOneShot(clatchAudioClips.RandomItem());
            _fireCount = 0;
        }

        if (!reloadBar.IsReloading && !Game.IsSpecialWave && In.GetReloadingDown())
            if (Game.Ammo < Game.CurrentTurretModel.Ammo)
                Reload();
            else
                MainCamera.AudioSource.PlayOneShot(reloadMiss);

        _lastFireInput = currentFireInput;
    }

    private void Fire(GameObject arm)
    {
        var armPos = arm.transform.position;
        var newBullet = Instantiate(
            original: Model.Name == "Laser Gun" ? laser : bullet,
            position: armPos,
            rotation: In.GetInput().ToQuaternion() //MainCamera.MainCam.AngleToMouse(transform.position)
        );
        if (Model.Name == "Laser Gun")
        {
            var laser = newBullet.GetComponent<Laser>();
            laser.Arm = arm.transform;
            laser.Turret = transform;
            _isLaserFiring = true;
            return;
        }

        newBullet.GetComponent<Rigidbody2D>().velocity = newBullet.transform.right * Model.Speed / 100f;
        --Game.Ammo;
        ++Game.CurrentWaveTurretFired;
        _ammoCounter.UpdateUI();

        // Reload if no more ammo
        if (!reloadBar.IsReloading && Game.Ammo <= 0)
            Reload();
    }

    public void Reload()
    {
        Override(false);
        MainCamera.AudioSource.PlayOneShot(reloadStart);
        reloadBar.Reload(
            duration: 100f / Model.Reload,
            reloadCallback: () =>
            {
                MainCamera.AudioSource.PlayOneShot(reloadFinish);
                Game.Ammo = Game.CurrentTurretModel.Ammo;
                GameObject.FindWithTag("ammo_counter")?.GetComponent<AmmoCounter>()?.UpdateUI();
            });
    }

    public void Override(bool value)
    {
        if (!value && Game.HasOverride)
        {
            overrideAnimator.SetTrigger(Animator.StringToHash("reverse"));
            Game.HasOverride = false;
            bulletAudioSource.pitch = 1f;
        }
        else if (value && !Game.HasOverride)
        {
            overrideAnimator.SetTrigger(Animator.StringToHash("start"));
            Game.HasOverride = true;
            bulletAudioSource.pitch = 1.4f;
        }
    }
}