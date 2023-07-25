using Managers;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject explosion;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Contains("ship"))
        {
            col.GetComponent<Ship>().TakeDamage(Game.CurrentTurretModel.Damage);
            Instantiate(
                original: explosion,
                position: transform.position,
                rotation: Quaternion.identity
            );
            Destroy(gameObject);
        }
    }
}