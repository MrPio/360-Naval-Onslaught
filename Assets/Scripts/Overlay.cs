using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    public void End()
    {
        GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().BeginWave();
        gameObject.SetActive(false);
    }

    public void Middle()
    {
        Destroy(GameObject.FindWithTag("wave_spawner").GetComponent<WaveSpawner>().mainMenu);
    }
}