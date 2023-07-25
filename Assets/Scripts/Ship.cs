using System;
using System.Collections;
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
    private ShipModel _model;
    private int _health;

    private void Awake()
    {
        _model = GameManager.Instance.CurrentWave.Spawn();
        spriteRenderer.sprite = Resources.Load<Sprite>(_model.Sprite);
        boxCollider.size = spriteRenderer.bounds.size;
        if (_model.ExplodeClip != null)
            _explodeClip = Resources.Load<AudioClip>(_model.ExplodeClip);
        _fireClip = Resources.Load<AudioClip>(_model.FireClip);
        _moneyCounter = GameObject.FindWithTag("money_counter").GetComponent<MoneyCounter>();
        GetComponent<ShipPath>().Model = _model;
        var pos = MainCamera.mainCam.RandomBoundaryPoint() * 1.1f;
        transform.SetPositionAndRotation(pos, pos.toQuaternion());


        // Custom Path for SpeedBoat
        if (_model.Name == "SpeedBoat")
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
        _health = _model.Health;
        healthBar.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (_health > 0)
        {
            _health -= damage;
            animator.SetTrigger(ShipDamage);
            healthBar.SetValue(_health / (float)_model.Health);
            if (_health <= 0)
                Explode();
        }
    }

    private void Explode(bool reward = true)
    {
        if (GetComponent<ShipPath>().dead) return;

        GetComponent<ShipPath>().dead = true;
        MainCamera.AudioSource.PlayOneShot(_explodeClip);
        animator.SetTrigger(ShipDestroy);
        Instantiate(explosions.RandomItem(), transform);

        // Money Reward
        if (reward)
        {
            var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
            floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {_model.Money} $";
            floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
            GameManager.Instance.Money += _model.Money;
            _moneyCounter.UpdateUI();
        }
        
        IEnumerator myWaitCoroutine()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        StartCoroutine(myWaitCoroutine());
    }

    public void End() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("base"))
        {
            GameObject.FindWithTag("base").GetComponent<Base>().TakeDamage(_model.Damage);
            Explode(false);
        }
    }
}