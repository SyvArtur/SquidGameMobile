using Cinemachine;
using Mirror;
using StarterAssets;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerProperties : NetworkBehaviour
{
    [SerializeField] private GameObject _nickname;
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject cinemachineCameraTarget;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [SyncVar]
    public int countOfWins;



    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar]
    private string computerName;

    [SyncVar(hook = nameof(OnActiveChanged))]
    public bool playerActive;


    void OnNameChanged(string oldName, string newName)
    {
        _nickname.GetComponent<TextMeshPro>().text = newName;
        if (isLocalPlayer)
        {
            _nickname.SetActive(false);
        }
    }

    void OnActiveChanged(bool oldActive, bool newActive)
    {
        _nickname.GetComponent<TextMeshPro>().enabled = newActive;
        gameObject.GetComponent<ThirdPersonController>().enabled = newActive;
        meshRenderer.enabled = newActive;
    }

    private IEnumerator WaitAllClient(Scene scene)
    {
        while (!MyNetworkManager.allClientsReady)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);

        if (!playerActive)
        {
            playerActive = true;
        }

        if (scene.name.Equals("Lobby"))
        {
            //Debug.Log(UnityEngine.Random.Range(12, 16) + "  " + gameObject.transform.position);
            //Vector3 myPosition = new Vector3(UnityEngine.Random.Range(-8, 17), 2, UnityEngine.Random.Range(-10, 7));
            SetPlayersPositions();
            //TargetSetPoisionForLobby(connectionToClient, scene, myPosition);
            //Debug.Log(gameObject.transform.position);
        }
    }

    private void SetPlayersPositions()
    {
        bool allPlayersInPosition = true;
        for (int i = 0; i <= 10; i++)
        {
            for (int j = 0; j <= 7; j++)
            {
                Vector3 playerPosition = new Vector3(-5 + i * 2, 2, -9 + j * 2);
                if (i * 16 + j < MyNetworkManager.clientObjects.Count)
                {
                    if (Vector3.Distance(MyNetworkManager.clientObjects[i * 16 + j].transform.position, playerPosition) > 7)
                    {
                        TargetSetPlayerPosition(MyNetworkManager.clientObjects[i * 16 + j].GetComponent<NetworkIdentity>().connectionToClient, playerPosition);
                        allPlayersInPosition = false;
                    }
                }
                else
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

    [TargetRpc]
    public void TargetSetPlayerPosition(NetworkConnectionToClient conn, Vector3 newPosition)
    {
        conn.identity.gameObject.transform.position = newPosition;
    }

    /*    [TargetRpc]
        private void TargetSetPoisionForLobby(NetworkConnectionToClient conn, Scene scene, Vector3 myPosition)
        {
            StartCoroutine(SetPositionForLobbyScene(scene, myPosition));
        }*/

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*        if (scene.name.Equals("BlockParty"))
                {
                    //Debug.Log(UnityEngine.Random.Range(12, 16) + "  " + gameObject.transform.position);
                    gameObject.transform.position = new Vector3 (UnityEngine.Random.Range(10, 18), UnityEngine.Random.Range(6, 20), UnityEngine.Random.Range(10, 18));
                    //Debug.Log(gameObject.transform.position);
                }*/

        StartCoroutine(WaitAllClient(scene));
        TargetBoundCameraToPlayer(gameObject.GetComponent<NetworkIdentity>().connectionToClient);
        SetDefaultAnimation();
        //ClientLogic.TargetChangeAnimationDead(MyNetworkManager.clientObjects[i].GetComponent<NetworkIdentity>().connectionToClient, _animIDDeath, false);

        /*        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
                virtualCamera.Follow = cinemachineCameraTarget.transform;*/

        //NetworkServer.SpawnObjects();
    }

    private void SetDefaultAnimation()
    {
        int animIDDeath = Animator.StringToHash("Kill");
        gameObject.GetComponent<Animator>().SetBool(animIDDeath, false);
    }

    /*    private IEnumerator SetPositionForLobbyScene(Scene scene, Vector3 myPosition)
        {   
            //Debug.Log(UnityEngine.Random.Range(12, 16) + "  " + gameObject.transform.position);
            gameObject.transform.position = myPosition;
            yield return new WaitForSeconds(0.5f);
            //Debug.Log(gameObject.transform.position);
            if (Vector3.Distance(gameObject.transform.position, myPosition) > 5) 
            {
                SetPositionForLobbyScene(scene, myPosition);
            } else
            {
                yield break;
            }

        }*/


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        countOfWins = 0;
        playerActive = true;
        /*        if (isServer)
                {
                    countOfWins = countOfWins + Random.Range(1, 10);
                    //playerName = computerName + " (" + countOfWins + ")";
                }*/

        if (isLocalPlayer)
        {
            CmdSetComputerNameForLocalPlayer(Environment.MachineName);
            //CmdUpdatePlayerName();
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            virtualCamera.Follow = cinemachineCameraTarget.transform;
            //CmdChangePlayerActive(false);
        }
        if (isServer)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            //UpdatePlayerNameFromServer();
        }
        //OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }


    /*    [Command]
        public void CmdUpdatePlayerName()
        {
            playerName = computerName + " (" + countOfWins + ")";
        }*/

    public void UpdatePlayerNameFromServer()
    {
        playerName = computerName + " (" + countOfWins + ")";
    }

    [Command]
    public void CmdSetComputerNameForLocalPlayer(string compName)
    {
        computerName = compName;
        playerName = computerName + " (" + countOfWins + ")";
    }

    /*    [Command]
        public void CmdChangePlayerActive(bool active)
        {
            playerActive = active;
        }*/

    [TargetRpc]
    public void TargetBoundCameraToPlayer(NetworkConnectionToClient conn)
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = cinemachineCameraTarget.transform;
    }

    [TargetRpc]
    public void TargetDisconnectAndChangeScene(NetworkConnection conn, string nameScene)
    {
        NetworkClient.Disconnect();
        Destroy(MyNetworkManager.singleton.gameObject);
        for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
        {
            Destroy(MyNetworkManager.clientObjects[i]);
        }
        SceneManager.LoadScene(nameScene);
    }

    void Update()
    {
        if (isOwned /*|| isLocalPlayer*/) { return; }
        _nickname.transform.LookAt(2 * _nickname.transform.position - new Vector3(Camera.main.transform.position.x, _nickname.transform.position.y, Camera.main.transform.position.z));
    }
}

/*using Cinemachine;
using Mirror;
using StarterAssets;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerProperties : NetworkBehaviour
{
    [SerializeField] private GameObject _nickname;
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [SyncVar]
    public int countOfWins;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar]
    private string computerName;

    [SyncVar(hook = nameof(OnActiveChanged))]
    public bool playerActive;


    void OnNameChanged(string oldName, string newName)
    {
        _nickname.GetComponent<TextMeshPro>().text = newName;
        if (isLocalPlayer)
        {
            _nickname.SetActive(false);
        }
    }

    void OnActiveChanged(bool oldActive, bool newActive)
    {
        if (false)
        {
            _nickname.GetComponent<TextMeshPro>().enabled = newActive;
            gameObject.GetComponent<ThirdPersonController>().enabled = newActive;
            meshRenderer.enabled = newActive;
        }
    }

    private IEnumerator WaitAllClient(Scene scene)
    {
        while (!MyNetworkManager.allClientsReady)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);

        if (!playerActive)
        {
            playerActive = true;
        }

        if (scene.name.Equals("Lobby"))
        {
            //Debug.Log(UnityEngine.Random.Range(12, 16) + "  " + gameObject.transform.position);
            //Vector3 myPosition = new Vector3(UnityEngine.Random.Range(-8, 17), 2, UnityEngine.Random.Range(-10, 7));
            SetPlayersPositions();
            //TargetSetPoisionForLobby(connectionToClient, scene, myPosition);
            //Debug.Log(gameObject.transform.position);
        }
    }

    private void SetPlayersPositions()
    {
        bool allPlayersInPosition = true;
        for (int i = 0; i <= 10; i++)
        {
            for (int j = 0; j <= 7; j++)
            {
                Vector3 playerPosition = new Vector3(-5 + i * 2, 2, -9 + j * 2);
                if (i * 16 + j < MyNetworkManager.clientObjects.Count)
                {
                    if (Vector3.Distance(MyNetworkManager.clientObjects[i * 16 + j].transform.position, playerPosition) > 7)
                    {
                        TargetSetPlayerPosition(MyNetworkManager.clientObjects[i * 16 + j].GetComponent<NetworkIdentity>().connectionToClient, playerPosition);
                        allPlayersInPosition = false;
                    }
                }
                else
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

    [TargetRpc]
    public void TargetSetPlayerPosition(NetworkConnectionToClient conn, Vector3 newPosition)
    {
        conn.identity.gameObject.transform.position = newPosition;
    }

*//*    [TargetRpc]
    private void TargetSetPoisionForLobby(NetworkConnectionToClient conn, Scene scene, Vector3 myPosition)
    {
        StartCoroutine(SetPositionForLobbyScene(scene, myPosition));
    }*//*

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        *//*        if (scene.name.Equals("BlockParty"))
                {
                    //Debug.Log(UnityEngine.Random.Range(12, 16) + "  " + gameObject.transform.position);
                    gameObject.transform.position = new Vector3 (UnityEngine.Random.Range(10, 18), UnityEngine.Random.Range(6, 20), UnityEngine.Random.Range(10, 18));
                    //Debug.Log(gameObject.transform.position);
                }*//*

        StartCoroutine(WaitAllClient(scene));
        TargetBoundCameraToPlayer(gameObject.GetComponent<NetworkIdentity>().connectionToClient);
        SetDefaultAnimation();
        //ClientLogic.TargetChangeAnimationDead(MyNetworkManager.clientObjects[i].GetComponent<NetworkIdentity>().connectionToClient, _animIDDeath, false);

        *//*        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
                virtualCamera.Follow = cinemachineCameraTarget.transform;*//*

        //NetworkServer.SpawnObjects();
    }

    private void SetDefaultAnimation()
    {
        int animIDDeath = Animator.StringToHash("Kill");
        gameObject.GetComponent<Animator>().SetBool(animIDDeath, false);
    }

    *//*    private IEnumerator SetPositionForLobbyScene(Scene scene, Vector3 myPosition)
        {   
            //Debug.Log(UnityEngine.Random.Range(12, 16) + "  " + gameObject.transform.position);
            gameObject.transform.position = myPosition;
            yield return new WaitForSeconds(0.5f);
            //Debug.Log(gameObject.transform.position);
            if (Vector3.Distance(gameObject.transform.position, myPosition) > 5) 
            {
                SetPositionForLobbyScene(scene, myPosition);
            } else
            {
                yield break;
            }

        }*//*


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        countOfWins = 0;
        playerActive = true;
        *//*        if (isServer)
                {
                    countOfWins = countOfWins + Random.Range(1, 10);
                    //playerName = computerName + " (" + countOfWins + ")";
                }*//*

        if (isLocalPlayer)
        {
            CmdSetComputerNameForLocalPlayer(Environment.MachineName);
            //CmdUpdatePlayerName();
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            //CmdChangePlayerActive(false);
        }
        if (isServer)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            //UpdatePlayerNameFromServer();
        }
        //OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }


*//*    [Command]
    public void CmdUpdatePlayerName()
    {
        playerName = computerName + " (" + countOfWins + ")";
    }*//*

    public void UpdatePlayerNameFromServer()
    {
        playerName = computerName + " (" + countOfWins + ")";
    }

    [Command]
    public void CmdSetComputerNameForLocalPlayer(string compName)
    {
        computerName = compName;
        playerName = computerName + " (" + countOfWins + ")";
    }

    [TargetRpc]
    public void TargetBoundCameraToPlayer(NetworkConnectionToClient conn)
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        //virtualCamera.Follow = cinemachineCameraTarget.transform;
    }

    [TargetRpc]
    public void TargetDisconnectAndChangeScene(NetworkConnection conn, string nameScene)
    {
        NetworkClient.Disconnect();
        Destroy(MyNetworkManager.singleton.gameObject);
        for (int i = 0; i < MyNetworkManager.clientObjects.Count; i++)
        {
            Destroy(MyNetworkManager.clientObjects[i]);
        }
        SceneManager.LoadScene(nameScene);
    }

    void Update()
    {
        if (isOwned *//*|| isLocalPlayer*//*) { return; }
        _nickname.transform.LookAt(2* _nickname.transform.position - new Vector3(Camera.main.transform.position.x, _nickname.transform.position.y, Camera.main.transform.position.z));
    }
}
*/