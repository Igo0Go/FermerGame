using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameConsole : MonoBehaviour
{
    private string input;

    private void Awake()
    {
        ConsoleEventCenter.Reload();
    }

    private void Update()
    {
        ShowConsoleToggle();
    }

    private void OnGUI()
    {
        if (!ConsoleEventCenter.ShowConsole)
            return;

        float y = 0;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    private void ShowConsoleToggle()
    {
        if(Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.BackQuote))
        {
            ConsoleEventCenter.ShowConsole = !ConsoleEventCenter.ShowConsole;
        }
    }
}
