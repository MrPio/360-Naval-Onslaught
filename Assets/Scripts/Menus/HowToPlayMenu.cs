using System.Collections.Generic;
using ExtensionsFunctions;
using Managers;
using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject pcScreen, gamepadScreen, mobileScreen, closeButton, playButton;
    [SerializeField] private List<GameObject> pages;
    private int _index;

    private int Index
    {
        get => _index;
        set => pages.ForEach((it, i) => it.SetActive(i == (_index = value)));
    }

    private void OnEnable()
    {
        var isPc = !InputManager.Instance.HasJoystick() && !InputManager.IsMobile;
        var isMobile = !InputManager.Instance.HasJoystick() && InputManager.IsMobile;
        var isGamepad = InputManager.Instance.HasJoystick();
        pcScreen.SetActive(isPc);
        gamepadScreen.SetActive(isGamepad);
        mobileScreen.SetActive(isMobile);
        Index = 0;
    }

    public void NextPage() => ++Index;
    public void PreviousPage() => --Index;

    public bool IsNewGame
    {
        set
        {
            closeButton.SetActive(!value);
            playButton.SetActive(value);
        }
    }
}