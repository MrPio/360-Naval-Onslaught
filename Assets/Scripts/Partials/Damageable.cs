using System.Collections.Generic;
using ExtensionsFunctions;
using TMPro;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private GameObject damageText;
    [SerializeField] private float offset = 1, textSize = 36;
    [SerializeField] private Color color = Color.red;
    [SerializeField] private List<AudioClip> damageClips = new(), criticalDamageClips = new();

    public void Damage(int damage, bool critical = false)
    {
        SpawnText(damage, critical);
        MakeSound(critical);
    }

    private void SpawnText(int damage, bool critical)
    {
        Instantiate(
            original: damageText,
            parent: GameObject.FindWithTag("damage_text_container").transform
        ).Apply(go =>
        {
            go.transform.position = transform.position + Vector3.up * offset;
            go.transform.GetComponentInChildren<TextMeshProUGUI>().Apply(text =>
            {
                text.color = critical ? Color.red : color;
                text.fontSize = textSize * (critical ? 1.25f : 1f);
                text.text = "-" + damage.ToString("N0");
            });
        });
    }

    private void MakeSound(bool critical)
    {
        var clips = critical ? criticalDamageClips : damageClips;
        if (clips.Count > 0)
            MainCamera.AudioSource.PlayOneShot(clips.RandomItem(), 0.65f);
    }
}