using System;
using ExtensionsFunctions;
using Managers;
using Model;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static TurretModel Model => Game.CurrentTurretModel;
    [SerializeField] private GameObject leftSpawnPoint;
    [SerializeField] private GameObject rightSpawnPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ReloadBar reloadBar;
    [SerializeField] private AudioClip reloadStart, reloadMiss, reloadFinish, noAmmo;

    private AmmoCounter _ammoCounter;
    private float _fireAccumulator;
    private AudioClip _fireClip;

    private void Awake()
    {
        spriteRenderer.sprite = Resources.Load<Sprite>(Model.Sprite);
        _fireClip = Resources.Load<AudioClip>(Model.FireClip);
        _ammoCounter = GameObject.FindWithTag("ammo_counter").GetComponent<AmmoCounter>();
    }

    void Update()
    {
        _fireAccumulator += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && Game.Ammo <= 0)
            MainCamera.AudioSource.PlayOneShot(noAmmo);
        if (Input.GetMouseButton(0) && Game.Ammo > 0 && _fireAccumulator > 100f / Model.Rate && !reloadBar.IsReloading)
        {
            _fireAccumulator = 0;
            Fire(leftSpawnPoint);
            Fire(rightSpawnPoint);
            MainCamera.AudioSource.PlayOneShot(_fireClip);
        }

        if (!reloadBar.IsReloading && Input.GetKeyDown(KeyCode.R))
            if (Game.Ammo < Game.CurrentTurretModel.Ammo)
                Reload();
            else
                MainCamera.AudioSource.PlayOneShot(reloadMiss);
    }


    private void Fire(GameObject arm)
    {
        var armPos = arm.transform.position;
        var newBullet = Instantiate(
            original: bullet,
            position: armPos,
            rotation: MainCamera.mainCam.AngleToMouse(transform.position)
        );
        newBullet.GetComponent<Rigidbody2D>().velocity = newBullet.transform.right * Model.Speed / 100f;
        --Game.Ammo;
        _ammoCounter.UpdateUI();

        // Reload if no more ammo
        if (!reloadBar.IsReloading && Game.Ammo <= 0)
            Reload();
    }

    private void Reload()
    {
        MainCamera.AudioSource.PlayOneShot(reloadStart);
        reloadBar.Reload(
            duration: 100f / Model.Reload,
            reloadCallback: () =>
            {
                MainCamera.AudioSource.PlayOneShot(reloadFinish);
                Game.Ammo = Game.CurrentTurretModel.Ammo;
                GameObject.FindWithTag("ammo_counter").GetComponent<AmmoCounter>().UpdateUI();
            });
    }
}