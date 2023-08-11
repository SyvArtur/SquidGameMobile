using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenuByEscForSingleGame : MonoBehaviour
{
    private Menu _menu;

    void Awake()
    {
        _menu = new Menu();
        _menu.CreateMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_menu.MenuActive)
            {
                _menu.ShowMenu();
            }
            else
                _menu.HideMenu();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (_menu.MenuActive)
        {
            if (hasFocus)
            Cursor.visible = true;
        }
    }
}
