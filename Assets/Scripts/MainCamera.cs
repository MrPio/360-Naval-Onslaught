using System;
using ExtensionsFunctions;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static Camera MainCam;
    public static float Height, Width;
    public static AudioSource AudioSource;
    [SerializeField] private AnimationCurve transitionCurve;
    private float _transitionAcc;
    private Vector2? transitionTo, transitionFrom;
    [SerializeField] private float transitionDuration = 3f;

    private void Start()
    {
        MainCam = Camera.main;
        AudioSource = MainCam.GetComponent<AudioSource>();
        Height = MainCam.GetHeight();
        Width = MainCam.GetWidth();
        SetFixedAspectRatio(16.0f / 9.0f);
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
        if (transitionTo is { } && transitionFrom is { })
        {
            transform.position = (Vector3)Vector2.Lerp((Vector2)transitionFrom, (Vector2)transitionTo,
                transitionCurve.Evaluate(_transitionAcc / transitionDuration)) + new Vector3(0, 0, -10);
            _transitionAcc += Time.fixedDeltaTime;
            if (_transitionAcc >= transitionDuration)
            {
                transitionTo = null;
                transitionFrom = null;
            }
        }
    }

    public void TransitionTo(Vector2 to)
    {
        transitionFrom = transform.position;
        transitionTo = to;
        _transitionAcc = 0;
    }
}