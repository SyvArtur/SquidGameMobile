using Mirror;
using Mirror.Discovery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

class MyNetworkManager : NetworkManager
{
    private NetworkDiscovery networkDiscovery;
    private bool serverFound;
    [SerializeField] private float timeout = 4f;
    public static List<GameObject> clientObjects = new List<GameObject>();
    public static bool allClientsReady;


    public override void Start()
    {
        base.Start();
        GameObject[] networkManagers = GameObject.FindGameObjectsWithTag("NetworkManager");
        if (networkManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        networkDiscovery = gameObject.GetComponent<NetworkDiscovery>();
        //UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, ServerIsFounded);
        //UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { this, networkDiscovery }, "Set NetworkDiscovery");

        networkDiscovery.OnServerFound.AddListener(ServerIsFounded);
        serverFound = false;
        
        networkDiscovery.StartDiscovery();
        //ConnectToServer("192.168.1.17");
        StartCoroutine(CheckConnection());

        Cursor.visible = false;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        clientObjects.Add(player);

        NetworkServer.AddPlayerForConnection(conn, player);

    }


    private IEnumerator CheckConnection()
    {
        yield return new WaitForSeconds(4);
        if (!serverFound)
        {
            networkDiscovery.StopDiscovery();
            StartHost();
            networkDiscovery.AdvertiseServer();
        }
    }

    public void ServerIsFounded(ServerResponse serverResponse)
    {
        serverFound = true;
        networkDiscovery.StopDiscovery();
        ConnectToServer(serverResponse);
    }

    public void ConnectToServer(ServerResponse ipAddress)
    {
        //networkAddress = ipAddress.ToString();

        StartClient(ipAddress.uri);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Time.timeScale = 0;
        AudioListener.pause = true;
        StartCoroutine(WaitForAllClientsToLoadScene());
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        allClientsReady = false;
/*        foreach (var player in MyNetworkManager.clientObjects)
        {
            //player.SetActive(true);
            if (!player.activeSelf)
            {
                player.SetActive
            }
        }*/
        /*        allClientsReady = false;
                Time.timeScale = 0;
                AudioListener.pause = true;
                StartCoroutine(WaitForAllClientsToLoadScene());*/
    }


    private IEnumerator WaitForAllClientsToLoadScene()
    {
        //yield return new WaitForEndOfFrame();

        while (singleton.isNetworkActive && NetworkServer.connections.Count > 0)
        {
            bool allClientsLoaded = true;

            foreach (NetworkConnection conn in NetworkServer.connections.Values)
            {
                if (conn != null && conn.isReady == false)
                {
                    allClientsLoaded = false;
                    break;
                }
            }

            if (allClientsLoaded)
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
                yield return new WaitForSeconds(0.3f);
                allClientsReady = true;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
        Debug.LogError("Error waiting for all clients to load scene.");
    }
}