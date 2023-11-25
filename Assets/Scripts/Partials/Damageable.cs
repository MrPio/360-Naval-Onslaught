using System.Collections.Generic;
using ExtensionsFunctions;
using TMPro;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private GameObject damageText;
    [SerializeField] private float offset = 1, textSize = 36;
    [SerializeField] private Color color = Color.red;
    [SerializeField] private List<AudioClip> damageClips = new();

    public void Damage(int damage)
    {
        SpawnText(damage);
        MakeSound();
    }
    private void SpawnText(int damage)
    {
        Instantiate(
            original: damageText,
            parent: GameObject.FindWithTag("damage_text_container").transform
        ).Apply(go =>
        {
            go.transform.position = transform.position + Vector3.up * offset;
            go.transform.GetComponentInChildren<TextMeshProUGUI>().Apply(text =>
            {
                text.color = color;
                text.fontSize = textSize;
                text.text = "-" + damage.ToString("N0");
            });
        });
    }

    private void MakeSound()
    {
        if (damageClips.Count > 0)
            MainCamera.AudioSource.PlayOneShot(damageClips.RandomItem(),0.8f);
    }
}