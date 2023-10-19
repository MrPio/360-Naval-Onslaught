using UnityEngine;

[ExecuteInEditMode]
public class GlitchController : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float strenght;
    private static readonly int Strenght = Shader.PropertyToID("_Strenght");

    private void Update()
    {
        material.SetFloat(Strenght, strenght);
    }
}