using System;
using JetBrains.Annotations;
using UnityEngine;

public class MobileShopConfirmMenu : MonoBehaviour
{
    [SerializeField] private GameObject contextContainer;
    [NonSerialized] [CanBeNull] public Action Action;

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

    public void SetContent(GameObject content) =>
        Instantiate(content, contextContainer.transform).transform.localPosition = Vector3.zero;
}