using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowMenuByEsc : NetworkBehaviour
{
    private Menu _menu;

    IEnumerator Start()
    {
        while (!MyNetworkManager.allClientsReady)
        {
            yield return new WaitForEndOfFrame();
        }
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
        if (hasFocus && _menu.MenuActive)
        {
            Cursor.visible = true;
        }
    }
}
