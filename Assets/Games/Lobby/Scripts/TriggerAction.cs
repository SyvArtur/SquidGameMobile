using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerAction : NetworkBehaviour
{
    private string nameGame;
    public string NameGame { get => nameGame; set => nameGame = value; }

    private bool multiplayer;
    public bool Multiplayer { get => multiplayer; set => multiplayer = value; }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (NetworkServer.active)
            {
                ChangeScene(collider);
            }
        }
    }


    void ChangeScene(Collider collider)
    {
        bool hostCollider = collider.gameObject.GetComponent<NetworkIdentity>().Equals(MyNetworkManager.clientObjects[0].GetComponent<NetworkIdentity>());
        if (Multiplayer)
        {
            if (hostCollider)
            {
                MyNetworkManager.singleton.ServerChangeScene(NameGame);
            }
        }
        else
        {
            if (hostCollider)
            {
                for (int i = MyNetworkManager.clientObjects.Count-1; i > 0; i--)
                {
                    collider.gameObject.GetComponent<PlayerProperties>().TargetDisconnectAndChangeScene(MyNetworkManager.clientObjects[i].gameObject.GetComponent<NetworkIdentity>().connectionToClient, NameGame);
                }
                Invoke(nameof(HostLoadSceneAndDisconnect), 1f);
                //Invoke collider.gameObject.GetComponent<PlayerProperties>().TargetDisconnectAndChangeScene(MyNetworkManager.clientObjects[0].gameObject.GetComponent<NetworkIdentity>().connectionToClient, NameGame);
                return;
            }
            NetworkIdentity targetIdentity = collider.gameObject.GetComponent<NetworkIdentity>();
            collider.gameObject.GetComponent<PlayerProperties>().TargetDisconnectAndChangeScene(targetIdentity.connectionToClient, NameGame);
            //TargetDisconnectAndChangeScene(MyNetworkManager.clientObjects[0].GetComponent<NetworkIdentity>().connectionToClient);
            //Destroy(MyNetworkManager.singleton.gameObject);
            //SceneManager.LoadScene(NameGame);
        }
        //StartCoroutine(LoadSceneAfterAllPlayersReadyCoroutine(NameGame));
    }

    private void HostLoadSceneAndDisconnect()
    {
        GameObject host = MyNetworkManager.clientObjects[0].gameObject;
        host.GetComponent<PlayerProperties>().TargetDisconnectAndChangeScene(host.GetComponent<NetworkIdentity>().connectionToClient, NameGame);
    }

}
