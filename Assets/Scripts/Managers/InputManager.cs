using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class InputManager
    {
        private static InputManager _instance;

        private InputManager()
        {
        }

        public static InputManager Instance => _instance ??= new InputManager();

        private bool? _hasJoystick = null;

        public Vector2 GetInput() =>
            HasJoystick()
                ? new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))
                : MainCamera.MainCam.ScreenToWorldPoint(Input.mousePosition);

        private bool HasJoystick() => _hasJoystick ??=
            Input.GetJoystickNames().Length > 0;

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
            else
                return Input.GetKeyDown(_specialsKeyboard[index]);
        }

        public bool GetTurretDown() =>
            HasJoystick() ? Input.GetAxis("Triggers")>0.3 : Input.GetMouseButtonDown(0);
        public bool GetTurretUp() =>
            HasJoystick() ? Input.GetAxis("Triggers")>0.3 : Input.GetMouseButtonUp(0);
        public bool GetTurret() =>
            HasJoystick() ? Input.GetAxis("Triggers")>0.3 : Input.GetMouseButton(0);
        public bool GetReloadingDown() =>
            Input.GetKeyDown(HasJoystick()?KeyCode.Joystick1Button2:KeyCode.R);
        public bool GetCannonDown() =>
            Input.GetKeyDown(HasJoystick()?KeyCode.Joystick1Button0:KeyCode.Space);
        public bool GetCannonUp() =>
            Input.GetKeyUp(HasJoystick()?KeyCode.Joystick1Button0:KeyCode.Space);

    }
}