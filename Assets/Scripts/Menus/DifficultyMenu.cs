using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyMenu : MonoBehaviour
{
    [SerializeField] private List<Image> cells;
    [SerializeField] private Sprite onCell, offCell;
    [NonSerialized] public Action OnClose;

    private void OnEnable() => SetDifficulty(GameManager.Instance.Difficulty);

    public void SetDifficulty(int index)
    {
        for (var i = 0; i < cells.Count; i++)
            cells[i].sprite = i == index ? onCell : offCell;
        GameManager.Instance.Difficulty = index;
        GameManager.Instance.Money = new[] { 300, 200, 100 }[index];
    }

    public void Close()
    {
        OnClose.Invoke();
        gameObject.SetActive(false);
    }
}