using Managers;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private static PlayerManager Player => PlayerManager.Instance;
    [SerializeField] private Rigidbody2D rb;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Contains("ship"))
        {
            col.GetComponent<Ship>().TakeDamage(Player.CurrentTurret.Damage);
            Destroy(gameObject);
        }
    }
}