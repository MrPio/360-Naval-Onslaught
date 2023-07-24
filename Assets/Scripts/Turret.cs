using ExtensionsFunctions;
using Managers;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private static PlayerManager Player => PlayerManager.Instance;
    [SerializeField] private GameObject leftSpawnPoint;
    [SerializeField] private GameObject rightSpawnPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private AmmoCounter ammoCounter;
    private float _fireAccumulator;
    
    void Update()
    {
        _fireAccumulator += Time.deltaTime;
        if (Input.GetMouseButton(0) && Player.Ammo > 0 && _fireAccumulator > 1 / Player.FireRate)
        {
            _fireAccumulator = 0;
            Fire(leftSpawnPoint);
            Fire(rightSpawnPoint);
        }
    }

    private void Fire(GameObject arm)
    {
        var newBullet = Instantiate(
            original: bullet,
            position: arm.transform.position,
            rotation: MainCamera.mainCam.AngleToMouse(transform.position)
        );
        newBullet.GetComponent<Rigidbody2D>().velocity = newBullet.transform.right * Player.BulletSpeed;
        --Player.Ammo;
        ammoCounter.UpdateUI();
    }
}