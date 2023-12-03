using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionsFunctions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

namespace Managers
{
    public class InputManager
    {
        public static void Reset() => _instance = new InputManager();

        private static InputManager _instance;

        private InputManager()
        {
        }

        public static InputManager Instance => _instance ??= new InputManager();

        public static bool IsMobile =>
            Application.platform is (RuntimePlatform.Android or RuntimePlatform.IPhonePlayer);

        private bool? _hasJoystick = null;
        private Vector2 _lastJoystickInput = Vector2.zero;
        public Vector2 MobileTurretDirection = Vector2.right;

        public bool IsMobileCannonPadDown,
            IsMobileCannonPadUp,
            IsMobileTurretPadDown,
            IsMobilePausePadDown,
            IsMobileReloadPadDown;

        public bool[] IsMobileSpecialDown = new bool[4];

        public Vector2 GetInput()
        {
            if (HasJoystick())
            {
                var newInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (newInput.sqrMagnitude > 0.3f)
                    _lastJoystickInput = newInput;
            }

            return HasJoystick()
                ? _lastJoystickInput
                : IsMobile
                    ? MobileTurretDirection
                    : MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition);
        }

        public bool HasJoystick() => _hasJoystick ??=
            Gamepad.current is { };

        private List<KeyCode> _specialsKeyboard = new()
            { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        private List<KeyCode> _specialsJoystick = new()
            { KeyCode.Joystick1Button5, KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button8 };

        public int GetDPad()
        {
            var h = Input.GetAxis("D-Pad Horizontal");
            var v = Input.GetAxis("D-Pad Vertical");
            if (v > 0) return 2;
            if (v < 0) return 0;
            if (h > 0) return 1;
            if (h < 0) return 3;
            return -1;
        }

        public bool SpecialDown(int index)
        {
            if (HasJoystick())
                return GetDPad() == index;
            if (IsMobile)
            {
                if (IsMobileSpecialDown[index])
                {
                    IsMobileSpecialDown[index] = false;
                    return true;
                }

                return false;
            }

            return Input.GetKeyDown(_specialsKeyboard[index]);
        }

        public bool GetTurretDown() =>
            IsMobile ? Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began :
            HasJoystick() ? Input.GetAxis("Triggers") > 0.3 : Input.GetMouseButtonDown(0);

        public bool GetTurretUp() =>
            IsMobile ? !IsMobileTurretPadDown :
            HasJoystick() ? Input.GetAxis("Triggers") > 0.3 : Input.GetMouseButtonUp(0);

        public bool GetTurret() =>
            IsMobile ? IsMobileTurretPadDown :
            HasJoystick() ? Input.GetAxis("Triggers") > 0.3 : Input.GetMouseButton(0);

        public bool GetReloadingDown() =>
            IsMobile
                ? IsMobileReloadPadDown.GetAsTrigger()
                : HasJoystick()
                    ? Input.GetKeyDown(KeyCode.Joystick1Button2)
                    : Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(2);

        public bool GetCannonUp() =>
            IsMobile
                ? IsMobileCannonPadUp.GetAsTrigger()
                : HasJoystick()
                    ? Input.GetKeyUp(KeyCode.Joystick1Button0)
                    : Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(1);

        public bool GetCannonDown() =>
            IsMobile
                ? IsMobileCannonPadDown.GetAsTrigger()
                : HasJoystick()
                    ? Input.GetKeyDown(KeyCode.Joystick1Button0)
                    : Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1);

        public bool GetPause() =>
            IsMobile
                ? IsMobilePausePadDown.GetAsTrigger()
                : HasJoystick()
                    ? Input.GetKeyDown(KeyCode.Joystick1Button6)
                    : Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);


        public IEnumerator Vibrate(float duration = 0.35f)
        {
            if (HasJoystick())
            {
                Gamepad.current.SetMotorSpeeds(1f, 1f);
                yield return new WaitForSeconds(duration);
                Gamepad.current.ResetHaptics();
            }
            else if (IsMobile)
            {
                //Handheld.Vibrate();
            }
        }
    }
}