using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForWin : NetworkBehaviour
{
    [SerializeField] private KeyEvents_UltimateKnockout _keyEvents;

    private int countOfInnactivePLayers = 0;
    public int CountOfInnactivePLayers { get => countOfInnactivePLayers; set => countOfInnactivePLayers = value; }


    void Start()
    {
        if (isServer)
        {
            _keyEvents.SubscribeToRevising(() =>
            {
                CheckForPlayerWin();
            });
        }
    }

    private void CheckForPlayerWin()
    {
        if ((CountOfInnactivePLayers >= NetworkServer.connections.Count - 1) && NetworkServer.connections.Count != 1)
        {
            for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
            {
                if (MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>().playerActive)
                {
                    PlayerProperties playerProperties = MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>();
                    playerProperties.countOfWins++;
                    playerProperties.UpdatePlayerNameFromServer();
                    MyNetworkManager.singleton.ServerChangeScene("Lobby");
                    return;
                }
            }
        }
        if (CountOfInnactivePLayers >= NetworkServer.connections.Count)
        {
            MyNetworkManager.singleton.ServerChangeScene("Lobby");
            return;
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
