using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using Model;
using TMPro;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private static readonly int ShipDamage = Animator.StringToHash("ship_damage");
    private static readonly int ShipDestroy = Animator.StringToHash("ship_destroy");
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject floatingTextBig;
    private AudioClip _fireClip;
    private AudioClip _explodeClip;
    private ShipModel _model;
    private int _health;

    private void Awake()
    {
        _model = PlayerManager.Instance.CurrentWave.Spawn();
        spriteRenderer.sprite = Resources.Load<Sprite>(_model.Sprite);
        boxCollider.size = spriteRenderer.bounds.size;
        if (_model.ExplodeClip != null)
            _explodeClip = Resources.Load<AudioClip>(_model.ExplodeClip);
        _fireClip = Resources.Load<AudioClip>(_model.FireClip);

        // Custom Path for SpeedBoat
        if (_model.Name == "SpeedBoat")
        {
            GetComponent<ShipPath>().AddPath(
                Random.Range(0, 2) == 0
                    ? new List<Vector3> { Vector3.zero }
                    : new List<Vector3>
                    {
                        Quaternion.AngleAxis(Random.Range(-20f, 20f), Vector3.forward) * transform.position,
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
            healthBar.gameObject.SetActive(true);
            healthBar.setValue(_health / (float)_model.Health);
            if (_health <= 0)
                Explode();
        }
    }

    private void Explode()
    {
        MainCamera.AudioSource.PlayOneShot(_explodeClip);
        animator.SetTrigger(ShipDestroy);
        Instantiate(explosion, transform);

        // Money Reward
        var floatingTextBig = Instantiate(this.floatingTextBig, GameObject.FindWithTag("canvas").transform);
        floatingTextBig.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+ {_model.Money} $";
        floatingTextBig.transform.position = transform.position + Vector3.up * 0.5f;
        PlayerManager.Instance.Money += _model.Money;
    }

    public void End() => Destroy(gameObject);
}