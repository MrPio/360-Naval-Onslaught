using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class MobileShopConfirmMenu : MonoBehaviour
{
    [SerializeField] private GameObject contextContainer;
    [NonSerialized] [CanBeNull] public Action Action;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Close()
    {
        foreach (Transform child in contextContainer.transform)
            Destroy(child.gameObject);
        gameObject.SetActive(false);
    }

    public void NoOnClick() => Close();

    public void YesOnClick()
    {
        Action?.Invoke();
        Close();
    }

    public void SetContent(GameObject content, bool hideTitle = false)
    {
        titleText.gameObject.SetActive(!hideTitle);
        contextContainer.transform.localScale = hideTitle ? Vector3.one * .75f : Vector3.one;
        Instantiate(content, contextContainer.transform).transform.localPosition = Vector3.zero;
    }
}