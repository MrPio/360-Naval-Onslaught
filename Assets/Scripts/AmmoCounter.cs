using System;
using Managers;
using Model;
using TMPro;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    private static GameManager Game => GameManager.Instance;
    private static TurretModel TurretModel => Game.CurrentTurretModel;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Animator animator;
    private static readonly int Bounce = Animator.StringToHash("bounce");

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        textMeshPro.text = Game.Ammo + "/" + TurretModel.Ammo;
        animator.SetTrigger(Bounce);
    }
}