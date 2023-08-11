using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyMultiplayerMethods : NetworkBehaviour
{
    private static MyMultiplayerMethods _instance;

    public static MyMultiplayerMethods Instance => _instance;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        if (isServer)
        {
            if (_instance == null)
            {
                _instance = this;
                NetworkServer.Spawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

/*    [ServerCallback]
    private void Start()
    {
        if (isServer)
        {
            if (_instance == null)
            {
                _instance = this;
                NetworkServer.Spawn(gameObject);
            }
*//*            else
            {
                Destroy(gameObject);
            }*//*
        }
    }*/

/*    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isServer && _instance == null)
        {
            // Зарегистрируйте префаб объекта синглтона на клиенте
            NetworkClient.RegisterPrefab(_prefab);
            // Создайте объект синглтона на клиенте, если он еще не существует
            if (NetworkServer.active)
            {
                // Если сервер активен, создайте объект синглтона на клиенте с помощью NetworkServer.Spawn
                _instance = Instantiate(_prefab).GetComponent<MyMultiplayerMethods>();
                NetworkServer.Spawn(_instance.gameObject);
            }
        }
    }*/

/*    [ClientRpc]
    public void RpcWriteTextOnSelectedElement(GameObject objectText, string text)
    {
        objectText.GetComponent<Text>().text = text;
        Debug.Log(objectText.name);
    }

    [TargetRpc]
    public void TargetCameraObservation(NetworkConnection conn, GameObject observationPlace)
    {
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = observationPlace.transform;
    }

    [TargetRpc]
    public void TargetSetPlayerPosition(NetworkConnection conn, Vector3 newPosition)
    {
        conn.identity.gameObject.transform.position = newPosition;
    }

    [TargetRpc]
    public void TargetPlayerWin(NetworkConnection conn)
    {
        Debug.Log("Hallo");
        conn.identity.gameObject.GetComponent<PlayerProperties>().countOfWins++;
        conn.identity.gameObject.GetComponent<PlayerProperties>().CmdUpdatePlayerName();
    }

    [ClientRpc]
    public void RPCPaintColorOnImage(GameObject objectImage, Color color)
    {
        objectImage.GetComponent<Image>().color = color;
    }

    [ClientRpc]
    public void RpcUpdateObjectMaterial(GameObject obj, Color color)
    {
        obj.GetComponent<MeshRenderer>().material.color = color;
    }*/

    [ClientRpc]
    public void RpcMakePlayerActive(GameObject player, bool active)
    {     
        player.SetActive(active);
    }
}
