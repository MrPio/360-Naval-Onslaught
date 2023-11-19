using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PadManager : MonoBehaviour
{
    private static InputManager In => InputManager.Instance;

    [SerializeField]
    private Collider2D turretCollider, fireTurretCollider, cannonCollider, reloadCollider, pauseCollider;

    [SerializeField] private Collider2D[] specialColliders;
    [SerializeField] private RectTransform turretCircleTransform, turretCircleParentTransform;


    private void Update()
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            var touchPhase = touch.phase;
            var worldPos = MainCamera.MainCam.ScreenToWorldPoint(touch.position);
            if (touchPhase is TouchPhase.Began)
            {
                if (turretCollider.OverlapPoint(worldPos))
                    HandleTurretMovement(worldPos);
                if (fireTurretCollider.OverlapPoint(worldPos))
                    In.IsMobileTurretPadDown = true;
                if (cannonCollider.OverlapPoint(worldPos))
                    In.IsMobileCannonPadDown = true;
                if (reloadCollider.OverlapPoint(worldPos))
                    In.IsMobileReloadPadDown = true;
                if (pauseCollider.OverlapPoint(worldPos))
                    In.IsMobilePausePadDown = true;
            }

            else if (touchPhase is TouchPhase.Canceled or TouchPhase.Ended)
            {
                if (turretCollider.OverlapPoint(worldPos))
                    turretCircleTransform.anchoredPosition = Vector2.zero;
                if (fireTurretCollider.OverlapPoint(worldPos))
                    In.IsMobileTurretPadDown = false;
                if (cannonCollider.OverlapPoint(worldPos))
                    In.IsMobileCannonPadUp = true;
                for (var j = 0; j < 4; j++)
                    if (specialColliders[j].OverlapPoint(worldPos))
                        In.IsMobileSpecialDown[j] = true;
            }
            else if (touchPhase is TouchPhase.Moved)
            {
                //right finger
                if (worldPos.x > 0)
                {
                    if (turretCollider.OverlapPoint(worldPos))
                        HandleTurretMovement(worldPos);
                    // left finger
                    else
                        turretCircleTransform.anchoredPosition = Vector2.zero;
                }
                // left finger
                else
                {
                    if (In.IsMobileTurretPadDown && !fireTurretCollider.OverlapPoint(worldPos))
                        In.IsMobileTurretPadDown = false;
                    if (!In.IsMobileTurretPadDown && fireTurretCollider.OverlapPoint(worldPos))
                        In.IsMobileTurretPadDown = true;
                    
                    if (!In.IsMobileCannonPadUp && !cannonCollider.OverlapPoint(worldPos))
                        In.IsMobileCannonPadUp = true;
                    //if (!In.IsMobileCannonPadDown && cannonCollider.OverlapPoint(worldPos))
                     //   In.IsMobileCannonPadDown = true;
                }
            }
        }
    }
    
    private void HandleTurretMovement(Vector3 worldPos)
    {
        var dir = turretCircleParentTransform.InverseTransformPoint(worldPos);
        In.MobileTurretDirection = dir.normalized;
        turretCircleTransform.anchoredPosition = dir / 2f;
    }
}