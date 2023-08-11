using Mirror;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class AllServerLogic_BlockParty : NetworkBehaviour
{
    [SerializeField] private GameObject _prefabBlock;
    [SerializeField] private Material[] _materials;
    [SerializeField] private float _timeBeforeDestroy = 10;

    [SerializeField] private ClientLogic_BlockParty ClientLogic;
    //[SerializeField] private MyMultiplayerMethods myMultiplayerMethods;

    private float _timeLeft;

    private int _sizeMatrixPlatform = 12;

    IEnumerator Start()
    {
        if (isServer)
        {
            while (!MyNetworkManager.allClientsReady)
            {
                yield return new WaitForEndOfFrame();
            }


            Invoke(nameof(CheckPositionAndKill), 2f);

            StartNewRound();

            SetPlayersPositions();

        }
    }

    private void SetPlayersPositions()
    {
        bool allPlayersInPosition = true; 
        for (int i = 1; i < _sizeMatrixPlatform - 1; i++)
        {
            for (int j = 1; j < _sizeMatrixPlatform - 1; j++)
            {
                if ((i - 1) * (_sizeMatrixPlatform - 1) + j - 1 < MyNetworkManager.clientObjects.Count)
                {
                    if (Vector3.Distance(MyNetworkManager.clientObjects[(i - 1) * (_sizeMatrixPlatform - 1) + j - 1].transform.position, new Vector3(i * 3, 8, j * 3)) > 6) 
                    {
                        ClientLogic.TargetSetPlayerPosition(MyNetworkManager.clientObjects[(i - 1) * (_sizeMatrixPlatform - 1) + j - 1].GetComponent<NetworkIdentity>().connectionToClient, new Vector3(i * 3, 8, j * 3));
                        allPlayersInPosition = false;
                    } 
                } else
                {
                    break;
                }
            }
        }
        if (!allPlayersInPosition)
        {
            Invoke(nameof(SetPlayersPositions), 0.018f);
        }
    }

    private void CountdownToDestroy()
    {
        if (_timeLeft < 0)
        {
            DestroyPlatformsByColor();
            Invoke(nameof(StartNewRound), 4f);
            /*            yield return new WaitForSeconds(4);
                        StartNewRound();*/
        }
        else
        {
            float seconds = Mathf.FloorToInt(_timeLeft % 60);
            ClientLogic.textTimer = string.Format("{0:0}", seconds);
            _timeLeft -= 1;
            Invoke(nameof(CountdownToDestroy), 1f);
            /*            yield return new WaitForSeconds(1);
                        StartCoroutine(CountdownToDestroy()); */
        }
    }



    private void CreatePlatforms()
    {
        for (int i = 0; i < _sizeMatrixPlatform; i++)
        {
            for (int j = 0; j < _sizeMatrixPlatform; j++)
            {
                GameObject platform = Instantiate(_prefabBlock, new Vector3(i * 3, 3, j * 3), Quaternion.Euler(0, 0, 0));
                ClientLogic._syncListColors.Add(_materials[Random.Range(0, _materials.Length)].color);
                NetworkServer.Spawn(platform);
                ClientLogic._platforms.Add(platform);
            }

        }
    }



    private void StartNewRound()
    {
        DestroyAllPlatforms();
        _timeLeft = _timeBeforeDestroy;
        if (_timeBeforeDestroy > 2) _timeBeforeDestroy--;
        ClientLogic._platforms.Clear();
        ClientLogic._syncListColors.Clear();
        CreatePlatforms();
        ChooseRightColor();
        CountdownToDestroy();

    }

    private void ChooseRightColor()
    {
        ClientLogic._rightColor = ClientLogic._syncListColors[Random.Range(0, ClientLogic._syncListColors.Count)];
    }

    private void DestroyPlatformsByColor()
    {
        for (int i = 0; i < _sizeMatrixPlatform; i++)
        {
            for (int j = 0; j < _sizeMatrixPlatform; j++)
            {
                Debug.Log(ClientLogic._syncListColors[i * (_sizeMatrixPlatform) + j] + "   " + ClientLogic._rightColor);
                if (ClientLogic._syncListColors[i * (_sizeMatrixPlatform) + j] != ClientLogic._rightColor)
                {
                    Destroy(ClientLogic._platforms[i * (_sizeMatrixPlatform) + j]);
                }
            }
        }
    }

    private void DestroyAllPlatforms()
    {
        if (ClientLogic._platforms != null ) {
            if (ClientLogic._platforms.Count != 0)
            {
                {
                    for (int i = 0; i < _sizeMatrixPlatform; i++)
                    {
                        for (int j = 0; j < _sizeMatrixPlatform; j++)
                        {
                            if (ClientLogic._platforms[i * (_sizeMatrixPlatform) + j] != null)
                            {
                                Destroy(ClientLogic._platforms[i * (_sizeMatrixPlatform) + j]);
                            }
                        }
                    }
                }
            }
        }
    }

    private int inactivePlayers = 0;

    private void CheckPositionAndKill()
    {
        for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
        {
            if (MyNetworkManager.clientObjects[i] != null && MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>().playerActive)
            {
                if (MyNetworkManager.clientObjects[i].transform.position.y <= 2.8) 
                { 
                    inactivePlayers++;
                    ClientLogic.TargetCameraObservation(MyNetworkManager.clientObjects[i].GetComponent<NetworkIdentity>().connectionToClient);
                    MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>().playerActive = false;
                    /*                        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
                                            virtualCamera.Follow = _observationPlace.transform;*/
                }
            }

            if (inactivePlayers >= NetworkServer.connections.Count)
            {
                MyNetworkManager.singleton.ServerChangeScene("Lobby");
                return;
            }

            if (inactivePlayers >= NetworkServer.connections.Count - 1 && NetworkServer.connections.Count != 1 && MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>().playerActive)
            {
                //ClientLogicTargetPlayerWin(MyNetworkManager.clientObjects[i]);
                PlayerProperties playerProperties = MyNetworkManager.clientObjects[i].GetComponent<PlayerProperties>();
                playerProperties.countOfWins++;
                playerProperties.UpdatePlayerNameFromServer();
                NetworkManager.singleton.ServerChangeScene("Lobby");
                return;
            } 
        }
        Invoke(nameof(CheckPositionAndKill), 2f);
    }

}
