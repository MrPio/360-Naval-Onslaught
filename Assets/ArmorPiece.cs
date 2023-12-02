using ExtensionsFunctions;
using UnityEngine;

public class ArmorPiece : MonoBehaviour
{
    private bool _isDropping;
    private float _acc, _normalBias, _duration, _rotation;
    private Vector2 _basePos, _destination;
    [SerializeField] private AnimationCurve normalMovement;

    private void Update()
    {
        if (!_isDropping)
            return;
        _acc += Time.deltaTime;
        var progress = _acc / _duration;
        if (progress < 1)
            transform.SetPositionAndRotation(
                position: Vector2.Lerp(_basePos, _destination, progress), 
                          // + Vector2.Perpendicular(_destination-_basePos).normalized * (normalMovement.Evaluate(progress) * _normalBias),
                rotation: Quaternion.Euler(0, 0, _rotation * progress)
            );
        if (_acc - _duration > 1.25f + _duration / 2f)
            Destroy(gameObject);
    }

    public void Drop(Vector2 destination)
    {
        _destination = destination;
        _basePos = transform.position;
        var magnitude = _basePos.magnitude;
        // _normalBias = Random.Range(-0.25f, 0.25f);
        _rotation = Random.Range(180 * 2, 180 * 5) * (Random.Range(0, 1) == 0 ? 1 : -1);
        _duration = Random.Range(0.6f, 1.5f);
        _acc = 0;
        _isDropping = true;
    }
}