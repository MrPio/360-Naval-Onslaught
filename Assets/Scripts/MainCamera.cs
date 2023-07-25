using System.Collections;
using System.Collections.Generic;
using ExtensionsFunctions;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static Camera MainCam;
    public static float Height, Width;
    public static AudioSource AudioSource;

    private void Start()
    {
        MainCam = Camera.main;
        AudioSource = MainCam.GetComponent<AudioSource>();
        Height = MainCam.GetHeight();
        Width = MainCam.GetWidth();
    }
}
