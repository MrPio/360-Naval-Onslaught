using System;
using ExtensionsFunctions;
using Managers;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static Camera MainCam;
    public static float Height, Width;
    public static AudioSource AudioSource;
    [SerializeField] private AnimationCurve transitionCurve;
    private float _transitionAcc;
    private Vector2? transitionTo, transitionFrom;
    private float? _orthoSizeFrom, _orthoSizeTo;
    [SerializeField] private float transitionDuration = 3f;
    private Vector2 _basePos = Vector2.zero;
    private float _orthoSize;
    public static float BaseOrthoSize;

    private void Start()
    {
        MainCam = Camera.main;
        AudioSource = MainCam.GetComponent<AudioSource>();
        Height = MainCam.GetHeight();
        Width = MainCam.GetWidth();
        //if (!InputManager.IsMobile)
        SetFixedAspectRatio(16.0f / 9.0f);
        if (InputManager.IsMobile)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        _orthoSize = MainCam.orthographicSize;
        BaseOrthoSize = _orthoSize;
    }

    private void SetFixedAspectRatio(float target)
    {
        // determine the game window's current aspect ratio
        var windowAspect = Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        var scaledHeight = windowAspect / target;

        // obtain camera component so we can modify its viewport
        var camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaledHeight < 1.0f)
        {
            var rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaledHeight;
            rect.x = 0;
            rect.y = (1.0f - scaledHeight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            var scaledWidth = 1.0f / scaledHeight;

            var rect = camera.rect;

            rect.width = scaledWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaledWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    private void FixedUpdate()
    {
        // Mouse movement following
        var biased = Vector2.zero;
        if (!InputManager.IsMobile)
        {
            var direction = ((Vector2)MainCam.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f));
            biased = direction.normalized * (Mathf.Pow(direction.magnitude, 2) * 0.55f); //*0.25f; 
        }

        if (transitionTo is { } && transitionFrom is { })
        {
            var t = transitionCurve.Evaluate(_transitionAcc / transitionDuration);
            _basePos = Vector2.Lerp((Vector2)transitionFrom, (Vector2)transitionTo, t);
            if (_orthoSizeFrom is { } && _orthoSizeTo is { })
                _orthoSize = (float)((1 - t) * _orthoSizeFrom + t * _orthoSizeTo);

            _transitionAcc += Time.fixedDeltaTime;
            if (_transitionAcc >= transitionDuration)
            {
                transitionTo = null;
                transitionFrom = null;
                _orthoSizeTo = null;
            }
        }

        transform.localPosition = (Vector3)(_basePos + biased) + new Vector3(0, 0, -10);
        MainCam.orthographicSize = _orthoSize;
    }

    public void TransitionTo(Vector2 to, float? orthoSizeTo = null)
    {
        transitionFrom = transform.position;
        _orthoSizeFrom = MainCam.orthographicSize;
        _orthoSizeTo = orthoSizeTo;
        transitionTo = to;
        _transitionAcc = 0;
    }
}