using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static Camera mainCam;
    public static AudioSource AudioSource;

    private void Start()
    {
        mainCam = Camera.main;
        AudioSource = mainCam.GetComponent<AudioSource>();
    }
}
