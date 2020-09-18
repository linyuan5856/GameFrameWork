using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Welcome :UIWindow
{
    private Text _txtWelcome;
    protected override void OnInitWindow()
    {
        base.OnInitWindow();
        _txtWelcome = GetComponentInChildren<Text>();
    }

    protected override void OnOpenWindow()
    {
        base.OnOpenWindow();
        _txtWelcome.text = "welcome to demo";
    }
}
