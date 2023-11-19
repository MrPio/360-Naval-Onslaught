using System;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    public Action OnEnd;
    public void End()
    {
        OnEnd.Invoke();
        gameObject.SetActive(false);
    }

    public void Middle()
    {
        GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().mainMenu.SetActive(false);
    }
}