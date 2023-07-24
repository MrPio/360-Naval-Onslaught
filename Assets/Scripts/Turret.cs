using System;
using ExtensionsFunctions;
using Managers;
using Model;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static TurretModel TurretModel => Game.CurrentTurret;
    [SerializeField] private GameObject leftSpawnPoint;
    [SerializeField] private GameObject rightSpawnPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private AmmoCounter _ammoCounter;
    private float _fireAccumulator;
    private AudioClip _fireClip;

    private void Awake()
    {
        spriteRenderer.sprite = Resources.Load<Sprite>(TurretModel.Sprite);
        _fireClip = Resources.Load<AudioClip>(TurretModel.Clip);
        _ammoCounter = GameObject.FindWithTag("ammo_counter").GetComponent<AmmoCounter>();
    }

    void Update()
    {
        _fireAccumulator += Time.deltaTime;
        if (Input.GetMouseButton(0) && Game.Ammo > 0 && _fireAccumulator > 100f / TurretModel.Rate)
        {
            _fireAccumulator = 0;
            Fire(leftSpawnPoint);
            Fire(rightSpawnPoint);
            MainCamera.AudioSource.PlayOneShot(_fireClip);
        }
    }

    private void Fire(GameObject arm)
    {
        var newBullet = Instantiate(
            original: bullet,
            position: arm.transform.position,
            rotation: MainCamera.mainCam.AngleToMouse(transform.position)
        );
        newBullet.GetComponent<Rigidbody2D>().velocity = newBullet.transform.right * TurretModel.Speed / 100f;
        --Game.Ammo;
        _ammoCounter.UpdateUI();
    }
}