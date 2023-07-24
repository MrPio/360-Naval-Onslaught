using System;
using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ship : MonoBehaviour
{
    private static readonly int ShipDamage = Animator.StringToHash("ship_damage");
    private static readonly int ShipDestroy = Animator.StringToHash("ship_destroy");
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private List<GameObject> explosions;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject floatingTextBig;
    private AudioClip _fireClip;
    private MoneyCounter _moneyCounter;
    private AudioClip _explodeClip;
    public ShipModel Model;
    private int _health;

    private void Awake()
    {
        Model = GameManager.Instance.CurrentWave.Spawn();
        spriteRenderer.sprite = Resources.Load<Sprite>(Model.Sprite);
        boxCollider.size = spriteRenderer.bounds.size;
        if (Model.ExplodeClip != null)
            _explodeClip = Resources.Load<AudioClip>(Model.ExplodeClip);
        _fireClip = Resources.Load<AudioClip>(Model.FireClip);
        _moneyCounter = GameObject.FindWithTag("money_counter").GetComponent<MoneyCounter>();
        GetComponent<ShipPath>().Model = Model;
        var pos = MainCamera.mainCam.RandomBoundaryPoint() * 1.1f;
        transform.SetPositionAndRotation(pos, pos.toQuaternion());


        // Custom Path for SpeedBoat
        if (Model.Name == "SpeedBoat")
        {
            GetComponent<ShipPath>().AddPath(
                Random.Range(0, 2) == 0
                    ? new List<Vector3> { Vector3.zero }
                    : new List<Vector3>
                    {
                        Quaternion.AngleAxis(Random.Range(-20f, 20f), Vector3.forward) * pos,
                        Vector3.zero
                    }
            );
        }
    }

    void Start()
    {
        _health = Model.Health;
        healthBar.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (_health > 0)
        {
            _health -= damage;
            animator.SetTrigger(ShipDamage);
            healthBar.gameObject.SetActive(true);
            healthBar.setValue(_health / (float)Model.Health);
            if (_health <= 0)
                Explode();
        }
    }

    public void Explode(bool reward = true)
    {
        MainCamera.AudioSource.PlayOneShot(_explodeClip);
        animator.SetTrigger(ShipDestroy);
        Instantiate(explosions.RandomItem(), transform);

        // Money Reward
        if (reward)
        {
            var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
            floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {Model.Money} $";
            floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
            GameManager.Instance.Money += Model.Money;
            _moneyCounter.UpdateUI();
        }
    }

    public void End() => Destroy(gameObject);
}